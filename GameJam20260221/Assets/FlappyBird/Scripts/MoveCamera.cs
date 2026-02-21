using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float normalMoveX = 0.5f;
    [SerializeField] PlayerFlappy player;
    [SerializeField] private float damageShakeDuration = 0.15f;
    [SerializeField] private float damageShakePower = 0.15f;
    public float CurrentMoveX { get; private set; }
    private float shakeTimer = 0.0f;
    private Vector3 shakeOffset = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        // Remove previous frame shake before moving camera to avoid drift.
        transform.position -= shakeOffset;

        float moveX = normalMoveX;

        if (player != null && (player.isDamage || player.isGoal))
        {
            moveX -= 5.0f;
            //Debug.Log("moveX" + moveX);
        }

        // Prevent stage/camera from moving backward.
        moveX = Mathf.Max(0.0f, moveX);
        CurrentMoveX = moveX * speed;

        Vector3 addPos = new Vector3(moveX, 0, 0);

        transform.position += addPos * (speed * Time.deltaTime);

        if (shakeTimer > 0.0f)
        {
            shakeTimer -= Time.deltaTime;
            float t = Mathf.Clamp01(shakeTimer / Mathf.Max(0.001f, damageShakeDuration));
            float power = damageShakePower * t;
            Vector2 random = Random.insideUnitCircle * power;
            shakeOffset = new Vector3(random.x, random.y, 0.0f);
            transform.position += shakeOffset;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }

    }

    public void TriggerDamageShake()
    {
        shakeTimer = Mathf.Max(shakeTimer, damageShakeDuration);
    }
}
