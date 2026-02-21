using UnityEngine;

public class LanePlayerController : MonoBehaviour
{
    [SerializeField] private RectTransform[] lanePoints;
    [SerializeField] private float moveSpeed = 900f;
    [SerializeField] private float hitShakeDuration = 0.08f;
    [SerializeField] private float hitShakePower = 3.5f;
    [SerializeField] private float hitShakeFrequency = 30f;
    [SerializeField] private AudioClip moveSe;

    private RectTransform rectTransform;
    private AudioSource audioSource;
    private int currentLaneIndex = 0;
    private float shakeTimer = 0f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (lanePoints == null || lanePoints.Length == 0)
        {
            return;
        }

        currentLaneIndex = FindNearestLaneIndex();
        SetXToCurrentLane();
    }

    void Update()
    {
        if (lanePoints == null || lanePoints.Length == 0 || rectTransform == null)
        {
            return;
        }

        float targetX = lanePoints[currentLaneIndex].anchoredPosition.x;
        Vector2 current = rectTransform.anchoredPosition;
        float nextX = Mathf.MoveTowards(current.x, targetX, moveSpeed * Time.deltaTime);
        float shakeX = 0f;
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            float t = Mathf.Clamp01(shakeTimer / Mathf.Max(0.001f, hitShakeDuration));
            float amp = hitShakePower * t;
            shakeX = Mathf.Sin(Time.time * hitShakeFrequency) * amp;
        }

        rectTransform.anchoredPosition = new Vector2(nextX + shakeX, current.y);
    }

    public void MoveLeft()
    {
        if (lanePoints == null || lanePoints.Length == 0)
        {
            return;
        }

        int next = Mathf.Max(0, currentLaneIndex - 1);
        if (next != currentLaneIndex)
        {
            currentLaneIndex = next;
            PlaySe(moveSe);
        }
    }

    public void MoveRight()
    {
        if (lanePoints == null || lanePoints.Length == 0)
        {
            return;
        }

        int next = Mathf.Min(lanePoints.Length - 1, currentLaneIndex + 1);
        if (next != currentLaneIndex)
        {
            currentLaneIndex = next;
            PlaySe(moveSe);
        }
    }

    public void TriggerHitShake()
    {
        shakeTimer = Mathf.Max(shakeTimer, hitShakeDuration);
    }

    private void PlaySe(AudioClip clip)
    {
        if (audioSource == null || clip == null)
        {
            return;
        }
        audioSource.PlayOneShot(clip);
    }

    private int FindNearestLaneIndex()
    {
        if (rectTransform == null)
        {
            return 0;
        }

        float playerX = rectTransform.anchoredPosition.x;
        int nearest = 0;
        float best = Mathf.Abs(playerX - lanePoints[0].anchoredPosition.x);

        for (int i = 1; i < lanePoints.Length; i++)
        {
            float diff = Mathf.Abs(playerX - lanePoints[i].anchoredPosition.x);
            if (diff < best)
            {
                best = diff;
                nearest = i;
            }
        }

        return nearest;
    }

    private void SetXToCurrentLane()
    {
        if (rectTransform == null || lanePoints == null || lanePoints.Length == 0)
        {
            return;
        }

        Vector2 current = rectTransform.anchoredPosition;
        float x = lanePoints[currentLaneIndex].anchoredPosition.x;
        rectTransform.anchoredPosition = new Vector2(x, current.y);
    }
}
