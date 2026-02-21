using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System.IO.IsolatedStorage;

public class PlayerFlappy : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float moveY = 0.0f;
    [SerializeField] private float jumpPower = 5.8f;
    [SerializeField] private float gravity = 22.0f;
    [SerializeField] private float playerPosX = 0.0f;
    [SerializeField] private float DamageCount = 0.6f;
    [SerializeField] GameObject mainCamera;
    [SerializeField] private AudioClip jumpSe;
    [SerializeField] private AudioClip missSe;
    public bool isDamage;
    public bool isGoal;

    private float startTime = 0.0f;
    private float elapsedTime = 0.0f;
    private UnityEngine.UI.Image playerImage;
    private AudioSource audioSource;
    private MoveCamera moveCameraScript;
    private bool jumpRequested = false;
    private int collisionCount = 0;

    [SerializeField] GameObject fadeOut;

    public int CollisionCount
    {
        get { return collisionCount; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerImage = GetComponent<UnityEngine.UI.Image>();
        audioSource = GetComponent<AudioSource>();
        if (mainCamera != null)
        {
            moveCameraScript = mainCamera.GetComponent<MoveCamera>();
        }
        isDamage = false;
        isGoal = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeOut != null && isGoal) 
        {
            return; 
        }
        // クリック入力はUpdateで受ける
        if (Input.GetMouseButtonDown(0))
        {
            jumpRequested = true;
        }

        OnDamageTimeCount();

        //��Q���ɓ�����ƈ�莞�ԃf�����b�g
        if (isDamage)
        {
            ColorAlphaChange();
        }

    }

    void FixedUpdate()
    {
        if (fadeOut != null && isGoal)
        {
            return;
        }
        if (jumpRequested)
        {
            OnJump();
            jumpRequested = false;
        }

        Gravity();

        Vector3 addPos = new Vector3(0.0f, moveY, 0.0f);
        transform.position += addPos * (speed * Time.fixedDeltaTime);
    }

    void Gravity()
    {
        const float kMaxGravity = -15.0f;
        moveY -= gravity * Time.fixedDeltaTime;

        if (moveY < kMaxGravity)
        {
            moveY = kMaxGravity;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollisionObject(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollisionObject(collision.gameObject);
    }

    private void HandleCollisionObject(GameObject hitObject)
    {
        if (hitObject.name == "topCollision")
        {
            OnTopScreenOut();
            return;
        }

        if (hitObject.name == "bottomCollision")
        {
            OnJump();
            return;
        }

        if (hitObject.name == "Goal")
        {
            isGoal = true;
            return;
        }

        StartDamage();

        // トゲに当たったらそのトゲだけ消す
        if (hitObject.GetComponent<Thorm>() != null)
        {
            Destroy(hitObject);
        }
    }

    /// <summary>
    /// �L�����N�^�[��_��
    /// </summary>
    private void ColorAlphaChange()
    {
        float alpha = Mathf.PingPong(Time.time * 10.0f, 1.0f);
        playerImage.color = new Color(1.0f, 1.0f, 1.0f, alpha);
    }

    private void OnDamageTimeCount()
    {
        if (isDamage)
        {
            elapsedTime = Time.time - startTime;

            if (elapsedTime >= DamageCount)
            {
                isDamage = false;
                elapsedTime = 0.0f;
                playerImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
    }

    private void OnJump()
    {
        moveY = 0.0f;
        moveY += jumpPower;
        PlaySe(jumpSe);
    }

    private void OnTopScreenOut()
    {
        moveY = 0.0f;
        moveY -= gravity * 0.3f;
    }

    private void StartDamage()
    {
        if (!isDamage)
        {
            collisionCount++;
            PlaySe(missSe);
            if (moveCameraScript != null)
            {
                moveCameraScript.TriggerDamageShake();
            }
        }
        isDamage = true;
        startTime = Time.time;
    }

    private void PlaySe(AudioClip clip)
    {
        if (audioSource == null || clip == null)
        {
            return;
        }
        audioSource.PlayOneShot(clip);
    }
}
