using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Image用
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public Sprite attackSprite;   // 攻撃時の画像
    private Sprite defaultSprite;  // 元の画像
    
    private SpriteRenderer sr;     // 2D用
    private Image uiImage;         // UI用

    public float timer;
    [SerializeField] private float missCooldown = 0.5f;
    private float cooldownTimer = 0f;
    private bool isInRange = false;
    [SerializeField] private float blinkSpeed = 8f;
    [SerializeField] private float postHitGrace = 0.2f;
    private float graceTimer = 0f;
    [SerializeField] private Image flashImage;
    [SerializeField] private float flashDuration = 0.12f;
    [SerializeField, Range(0f, 1f)] private float flashAlpha = 0.6f;
    private Coroutine flashRoutine;
    private readonly HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();
    [SerializeField] private AudioClip[] cutSfx;
    [SerializeField] private AudioClip hitSfx;
    private AudioSource audioSource;

    private int missCount = 0;
    [SerializeField] private GameObject FadeOut;
    [SerializeField] private GameObject clear; 
    [SerializeField] private GameObject FadeIn;
    [SerializeField] private float sceneChangeDelayAfterFade = 2.5f;
    [SerializeField] private string nextSceneName = "FlappyGame";

    void Start()
    {
        // 両方の可能性を考えて取得を試みる
        sr = GetComponent<SpriteRenderer>();
        uiImage = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();

        // 最初の画像を保存しておく
        if (sr != null) defaultSprite = sr.sprite;
        else if (uiImage != null) defaultSprite = uiImage.sprite;
    }

    void Update()
    {
        if(FadeOut != null)
        {
            return;
        }
        timer -= Time.deltaTime;
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            SetAlpha(alpha);
            return;
        }
        else
        {
            SetAlpha(1f);
        }
        if (graceTimer > 0f)
        {
            graceTimer -= Time.deltaTime;
        }
        // クリックした瞬間に画像を変更
        if (Input.GetMouseButtonDown(0))
        {
            if (!isInRange)
            {
                if (graceTimer <= 0f)
                {
                    cooldownTimer = missCooldown;
                }
                return;
            }
            timer = 0.06f;
            ChangeImage(attackSprite);
            // 0.2秒後に元の画像に戻す（おまけ機能）
            Invoke("ResetImage", 0.2f);
        }
    }

    void ChangeImage(Sprite nextSprite)
    {
        if (nextSprite == null) return;
        
        if (sr != null) sr.sprite = nextSprite;
        else if (uiImage != null) uiImage.sprite = nextSprite;
    }

    void ResetImage()
    {
        ChangeImage(defaultSprite);
    }

    void SetAlpha(float a)
    {
        if (sr != null)
        {
            Color c = sr.color;
            c.a = a;
            sr.color = c;
        }
        else if (uiImage != null)
        {
            Color c = uiImage.color;
            c.a = a;
            uiImage.color = c;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        isInRange = true;
        SpriteRenderer sp = other.gameObject.GetComponent<SpriteRenderer>();
        //sp.color = Color.red;
        if (timer>=0)
        {
            Debug.Log("Click!!!");
            // 強敵チェック
            PowerEnemyHealth pEnemy = other.GetComponent<PowerEnemyHealth>();
            if (pEnemy != null)
            {
               Debug.Log("nomal");

                pEnemy.TakeDamage();
                graceTimer = postHitGrace;
                hitEnemies.Add(other);
                PlayCutSfx();

                return;
            }

            // 普通の敵チェック
            EnemyHealth nEnemy = other.GetComponent<EnemyHealth>();
            if (nEnemy != null)
            {
            Debug.Log("power");

                nEnemy.TakeDamage();
                graceTimer = postHitGrace;
                hitEnemies.Add(other);
                PlayCutSfx();
                return;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Goal"))
        {
            GManager.instance.killScore = 10000 - missCount * 1000;
            Invoke("ClearImage", 0.3f);
            return;
        }
        isInRange = false;
        if (IsEnemy(other) && !hitEnemies.Contains(other))
        {
            missCount++;
            FlashOnce();
            PlayHitSfx();
        }
        hitEnemies.Remove(other);
    }

    private bool IsEnemy(Collider2D other)
    {
        return other.GetComponent<PowerEnemyHealth>() != null
            || other.GetComponent<EnemyHealth>() != null;
    }

    private void FlashOnce()
    {
        if (flashImage == null) return;
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashCoroutine());
    }

    private System.Collections.IEnumerator FlashCoroutine()
    {
        flashImage.gameObject.SetActive(true);
        Color c = flashImage.color;
        c.a = flashAlpha;
        flashImage.color = c;
        yield return new WaitForSeconds(flashDuration);
        c.a = 0f;
        flashImage.color = c;
        flashImage.gameObject.SetActive(false);
        flashRoutine = null;
    }

    private void PlayCutSfx()
    {
        if (audioSource == null || cutSfx == null || cutSfx.Length == 0) return;
        int index = Random.Range(0, cutSfx.Length);
        AudioClip clip = cutSfx[index];
        if (clip == null) return;
        audioSource.PlayOneShot(clip);
    }

    private void PlayHitSfx()
    {
        if (audioSource == null || hitSfx == null) return;
        audioSource.PlayOneShot(hitSfx);
    }
    /* void OnTriggerExit2D(Collider2D other) {
         SpriteRenderer sp = other.gameObject.GetComponent<SpriteRenderer>();
         sp.color = Color.white;
     }*/

    private void ClearImage()
    {
        clear.SetActive(true);
        Invoke("FadeInAnimation", 1.8f);
    }
    
    private void FadeInAnimation()
    {
        GameObject fadein = Instantiate(FadeIn, Vector3.zero, Quaternion.identity);
        //fadeinの親オブジェクトをcanvasに
        fadein.transform.SetParent(GameObject.Find("Canvas").transform, false);
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
}
