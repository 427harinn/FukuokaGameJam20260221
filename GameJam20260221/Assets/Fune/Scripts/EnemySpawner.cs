using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    private const int kTotalSpawnCount = 30;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private RectTransform spawnParent;
    [SerializeField] private RectTransform playerRect;
    [SerializeField] private LanePlayerController playerController;
    [SerializeField] private Image damageFlashImage;
    [SerializeField] private GameObject damageFlashRoot;
    [SerializeField] private float damageFlashDuration = 0.12f;
    [SerializeField, Range(0f, 1f)] private float damageFlashAlpha = 0.35f;
    [SerializeField] private AudioClip hitSe;
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private GameObject fadeInPrefab;
    [SerializeField] private float clearDelay = 0.4f;
    [SerializeField] private float fadeDelayAfterClear = 1.6f;
    [SerializeField] private float sceneChangeDelayAfterFade = 2.5f;
    [SerializeField] private string nextSceneName = "FlyLine";
    [SerializeField] private RectTransform[] spawnPoints;
    [SerializeField] private float spawnIntervalMin = 0.6f;
    [SerializeField] private float spawnIntervalMax = 1.2f;
    [SerializeField] private int minSpawnPerWave = 1;
    [SerializeField] private int maxSpawnPerWave = 2;
    [SerializeField] private float warningDuration = 0.5f;
    [SerializeField] private int maxAliveCount = 25;
    [SerializeField] private int warningChildIndex = 0;
    [SerializeField] private GameObject clearObj;
    [SerializeField] private GameObject fadeIn;
    [SerializeField] private GameObject fadeOut;

    private float timer = 0.0f;
    private int aliveCount = 0;
    private int spawnedCount = 0;
    private int hitCount = 0;
    private bool isPreparingSpawn = false;
    private bool isGameEnded = false;
    private bool isSceneChangeScheduled = false;
    private float nextSpawnInterval = 1.0f;
    private Coroutine flashCoroutine;
    private AudioSource audioSource;
    private readonly List<int> lastWaveIndices = new List<int>();

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SetAllWarnings(false);
        SetDamageFlashVisible(false);
        PickNextSpawnInterval();
    }

    void OnDestroy()
    {
        EnemyFallMover[] movers = FindObjectsByType<EnemyFallMover>(FindObjectsSortMode.None);
        for (int i = 0; i < movers.Length; i++)
        {
            if (movers[i] != null)
            {
                movers[i].DetachSpawner();
            }
        }
    }

    void Update()
    {
        if(fadeOut != null)
        {
            return;
        }
        if (enemyPrefab == null || spawnParent == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }

        if (isGameEnded)
        {
            return;
        }

        if (spawnedCount >= kTotalSpawnCount)
        {
            return;
        }

        if (aliveCount >= maxAliveCount)
        {
            return;
        }

        if (isPreparingSpawn)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer < nextSpawnInterval)
        {
            return;
        }

        timer = 0.0f;
        StartCoroutine(PrepareAndSpawn());
    }

    private IEnumerator PrepareAndSpawn()
    {
        isPreparingSpawn = true;

        if (spawnedCount >= kTotalSpawnCount || isGameEnded)
        {
            isPreparingSpawn = false;
            yield break;
        }

        int remainingSpawn = kTotalSpawnCount - spawnedCount;
        int remainingAliveCap = Mathf.Max(0, maxAliveCount - aliveCount);
        int maxByPoints = spawnPoints.Length;
        int waveMax = Mathf.Min(Mathf.Max(1, maxSpawnPerWave), remainingSpawn, remainingAliveCap, maxByPoints);
        int waveMin = Mathf.Min(Mathf.Max(1, minSpawnPerWave), waveMax);
        int waveCount = Random.Range(waveMin, waveMax + 1);
        List<int> selectedIndices = SelectSpawnIndices(waveCount);

        if (selectedIndices.Count == 0)
        {
            isPreparingSpawn = false;
            yield break;
        }

        List<GameObject> warningObjects = new List<GameObject>();
        for (int i = 0; i < selectedIndices.Count; i++)
        {
            RectTransform point = spawnPoints[selectedIndices[i]];
            GameObject warningObj = GetWarningObject(point);
            if (warningObj != null)
            {
                warningObj.SetActive(true);
                warningObjects.Add(warningObj);
            }
        }

        if (warningObjects.Count > 0)
        {
            yield return new WaitForSeconds(warningDuration);
            for (int i = 0; i < warningObjects.Count; i++)
            {
                if (warningObjects[i] != null)
                {
                    warningObjects[i].SetActive(false);
                }
            }
        }

        if (isGameEnded)
        {
            isPreparingSpawn = false;
            yield break;
        }

        for (int i = 0; i < selectedIndices.Count; i++)
        {
            int index = selectedIndices[i];
            RectTransform point = spawnPoints[index];
            if (point == null)
            {
                continue;
            }

            if (aliveCount >= maxAliveCount || spawnedCount >= kTotalSpawnCount)
            {
                break;
            }

            GameObject enemyObj = Instantiate(enemyPrefab, spawnParent);
            RectTransform enemyRect = enemyObj.GetComponent<RectTransform>();
            if (enemyRect != null)
            {
                enemyRect.anchoredPosition = point.anchoredPosition;
            }
            else
            {
                enemyObj.transform.position = point.position;
            }

            EnemyFallMover mover = enemyObj.GetComponent<EnemyFallMover>();
            if (mover == null)
            {
                mover = enemyObj.AddComponent<EnemyFallMover>();
            }
            mover.Initialize(this, playerRect);

            aliveCount++;
            spawnedCount++;
        }

        lastWaveIndices.Clear();
        lastWaveIndices.AddRange(selectedIndices);
        PickNextSpawnInterval();
        isPreparingSpawn = false;
    }

    public void NotifyEnemyDestroyed()
    {
        if (isGameEnded)
        {
            return;
        }

        aliveCount = Mathf.Max(0, aliveCount - 1);

        if (spawnedCount >= kTotalSpawnCount && aliveCount == 0)
        {
            EndGame(true);
        }
    }

    public void NotifyPlayerHit()
    {
        if (isGameEnded)
        {
            return;
        }

        hitCount++;
        Debug.Log("Player Hit!");
        PlaySe(hitSe);
        if (playerController != null)
        {
            playerController.TriggerHitShake();
        }
        PlayDamageFlash();
        Debug.Log($"Hit Count: {hitCount}");
    }

    private GameObject GetWarningObject(RectTransform point)
    {
        if (point == null || point.childCount <= warningChildIndex || warningChildIndex < 0)
        {
            return null;
        }
        return point.GetChild(warningChildIndex).gameObject;
    }

    private void SetAllWarnings(bool active)
    {
        if (spawnPoints == null)
        {
            return;
        }

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject warningObj = GetWarningObject(spawnPoints[i]);
            if (warningObj != null)
            {
                warningObj.SetActive(active);
            }
        }
    }

    private void PickNextSpawnInterval()
    {
        float min = Mathf.Min(spawnIntervalMin, spawnIntervalMax);
        float max = Mathf.Max(spawnIntervalMin, spawnIntervalMax);
        nextSpawnInterval = Random.Range(min, max);
    }

    private void PlayDamageFlash()
    {
        if (damageFlashImage == null)
        {
            return;
        }

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(DamageFlashCoroutine());
    }

    private IEnumerator DamageFlashCoroutine()
    {
        SetDamageFlashVisible(true);
        SetDamageFlashAlpha(damageFlashAlpha);
        yield return new WaitForSeconds(damageFlashDuration);
        SetDamageFlashVisible(false);
        SetDamageFlashAlpha(0f);
        flashCoroutine = null;
    }

    private void SetDamageFlashAlpha(float alpha)
    {
        if (damageFlashImage == null)
        {
            return;
        }

        Color c = damageFlashImage.color;
        c.a = alpha;
        damageFlashImage.color = c;
    }

    private void SetDamageFlashVisible(bool visible)
    {
        if (damageFlashRoot != null)
        {
            damageFlashRoot.SetActive(visible);
            return;
        }

        if (damageFlashImage != null)
        {
            damageFlashImage.gameObject.SetActive(visible);
        }
    }

    private void PlaySe(AudioClip clip)
    {
        if (audioSource == null || clip == null)
        {
            return;
        }
        audioSource.PlayOneShot(clip);
    }

    private void EndGame(bool isClear)
    {
        isGameEnded = true;
        isPreparingSpawn = false;
        StopAllCoroutines();
        SetAllWarnings(false);

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        if (isClear)
        {
            int score = CalculateFuneScore(hitCount);
            if (GManager.instance != null)
            {
                GManager.instance.funeScore = score;
            }
            Debug.Log($"Game Clear! (hits: {hitCount})");
            Debug.Log($"Fune Score: {score}");
            Invoke(nameof(ShowClearPanel), Mathf.Max(0f, clearDelay));
        }
        else
        {
            Debug.Log("Game Over! (hit)");
        }
    }

    private int CalculateFuneScore(int hits)
    {
        int safeHits = Mathf.Max(0, hits);
        float ratio = 1.0f - ((float)safeHits / kTotalSpawnCount);
        ratio = Mathf.Clamp01(ratio);
        float curved = Mathf.Pow(ratio, 1.3f);
        return Mathf.RoundToInt(10000.0f * curved);
    }

    private void ShowClearPanel()
    {
        if (clearPanel != null)
        {
            clearPanel.SetActive(true);
        }
        Invoke(nameof(SpawnFadeIn), Mathf.Max(0f, fadeDelayAfterClear));
    }

    private void SpawnFadeIn()
    {
        if (fadeInPrefab != null)
        {
            //親オブジェクトをcanvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                GameObject fadeInInstance = Instantiate(fadeInPrefab, Vector3.zero, Quaternion.identity);
                fadeInInstance.transform.SetParent(canvas.transform, false);
            }
        }

        if (!isSceneChangeScheduled)
        {
            isSceneChangeScheduled = true;
            Invoke(nameof(LoadNextScene), Mathf.Max(0f, sceneChangeDelayAfterFade));
        }
    }

    private void LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            return;
        }
        SceneManager.LoadScene(nextSceneName);
    }

    private List<int> SelectSpawnIndices(int count)
    {
        List<int> result = new List<int>();
        List<int> primaryPool = new List<int>();
        List<int> fallbackPool = new List<int>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] == null)
            {
                continue;
            }

            if (lastWaveIndices.Contains(i))
            {
                fallbackPool.Add(i);
            }
            else
            {
                primaryPool.Add(i);
            }
        }

        while (result.Count < count && primaryPool.Count > 0)
        {
            int pick = Random.Range(0, primaryPool.Count);
            result.Add(primaryPool[pick]);
            primaryPool.RemoveAt(pick);
        }

        while (result.Count < count && fallbackPool.Count > 0)
        {
            int pick = Random.Range(0, fallbackPool.Count);
            result.Add(fallbackPool[pick]);
            fallbackPool.RemoveAt(pick);
        }

        return result;
    }
}
