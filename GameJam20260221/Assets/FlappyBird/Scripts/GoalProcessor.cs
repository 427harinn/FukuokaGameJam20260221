using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalProcessor : MonoBehaviour
{
    [SerializeField] PlayerFlappy player;
    [SerializeField] private int totalThormCount = 20;
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private GameObject fadeInPrefab;
    [SerializeField] private float fadeInDelay = 0.5f;
    [SerializeField] private float sceneChangeDelayAfterFade = 2.5f;
    [SerializeField] private string nextSceneName = "izumi";
    private bool hasLoggedClear = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null && player.isGoal && !hasLoggedClear)
        {
            int score = CalculateFlappyScore(player.CollisionCount, totalThormCount);
            player.isGoal = true;
            if (GManager.instance != null)
            {
                GManager.instance.flappyScore = score;
            }
            if (clearPanel != null)
            {
                clearPanel.SetActive(true);
            }
            if (fadeInPrefab != null)
            {
                Invoke(nameof(SpawnFadeIn), fadeInDelay);
            }
            Debug.Log($"Game Clear! collision={player.CollisionCount}, score={score}");
            hasLoggedClear = true;
        }
    }

    private void SpawnFadeIn()
    {
        //親オブジェクトをcanvas
        GameObject fadeInInstance = Instantiate(fadeInPrefab, Vector3.zero, Quaternion.identity);
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            fadeInInstance.transform.SetParent(canvas.transform, false);
        }
        Invoke(nameof(LoadNextScene), sceneChangeDelayAfterFade);
    }

    private void LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            return;
        }
        SceneManager.LoadScene(nextSceneName);
    }

    private int CalculateFlappyScore(int collisions, int totalCount)
    {
        int safeTotal = Mathf.Max(1, totalCount);
        int surviveCount = Mathf.Clamp(safeTotal - Mathf.Max(0, collisions), 0, safeTotal);
        float ratio = (float)surviveCount / safeTotal;
        float curved = Mathf.Pow(ratio, 1.4f);
        return Mathf.RoundToInt(10000.0f * curved);
    }
}
