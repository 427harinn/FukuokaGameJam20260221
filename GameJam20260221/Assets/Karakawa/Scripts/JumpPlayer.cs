using UnityEngine;
using UnityEngine.UI;

public class JumpPlayer : MonoBehaviour
{
    private Vector2 direction;
    private Vector2 normal;
    private Rigidbody2D rb;
    private Image playerimage;
    private Collider2D playerCollider;
    [SerializeField, Range(0f, 1f)] private float upBlend = 0.5f;
    [SerializeField] private float jumpSpeed = 2.0f;
    [SerializeField] private float initialDamagedTime = 3f;
    [SerializeField] private int lineLayer = 6;
    private int playerLayer;

    private float damagedTime = 0f;
    private bool isDamaged = false;
    AudioSource audioSource;
    [SerializeField] private AudioClip jumpSE;
    [SerializeField] private AudioClip damageSE;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerimage = GetComponent<Image>();
        playerCollider = GetComponent<Collider2D>();
        playerLayer = gameObject.layer;
        damagedTime = initialDamagedTime;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        direction = rb.linearVelocity;
        if (isDamaged)
        {
            // ダメージ状態の処理
            damagedTime -= Time.deltaTime;
            if (damagedTime <= 0f)
            {
                isDamaged = false;
                damagedTime = initialDamagedTime;
                playerimage.color = new Color(1f, 1f, 1f, 1f);
                SetLineCollisionEnabled(true);
            }
            // ダメージ状態のときに、プレイヤーの色を点滅させる
            float alpha = Mathf.PingPong(Time.time * 10f, 1f);
            playerimage.color = new Color(1f, 1f, 1f, alpha);
        }
        else
        {
            if (playerimage.color.a < 1f)
            {
                playerimage.color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Line"))
        {
            audioSource.PlayOneShot(jumpSE);
            normal = collision.contacts[0].normal;
            Vector2 reflected = Vector2.Reflect(direction.normalized, normal);
            Vector2 blended = Vector2.Lerp(reflected.normalized, Vector2.up, upBlend);
            if (blended.y <= 0f)
            {
                blended = Vector2.up;
            }
            rb.linearVelocity = blended.normalized * jumpSpeed;

            ResetLine(collision);

        }
        if(collision.gameObject.CompareTag("Enemy"))
        {
            audioSource.PlayOneShot(damageSE);
            isDamaged = true;
            damagedTime = initialDamagedTime;
            SetLineCollisionEnabled(false);
            direction = Vector2.zero;
            Destroy(collision.gameObject);
            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.AddEnemyHit();
            }
        }
    }

    private void ResetLine(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<EdgeCollider2D>() == null) { return; }
        direction = rb.linearVelocity;
        collision.gameObject.GetComponent<EdgeCollider2D>().enabled = false;
        LineRenderer lineRenderer = collision.gameObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    private void SetLineCollisionEnabled(bool enabled)
    {
        if (lineLayer < 0 || lineLayer > 31) { return; }
        Debug.Log(enabled ? "有効化" : "無効化");
        Physics2D.IgnoreLayerCollision(playerLayer, lineLayer, !enabled);
    }
}
