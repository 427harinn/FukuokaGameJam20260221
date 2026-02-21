using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance = null;
    public int lineScore = 0;
    public float elapsedTime = 0f;
    [SerializeField] private int penaltyPerEnemyHit = 500;
    private float startTime = 0f;
    private bool isGameCleared = false;
    private int enemyHitCount = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameCleared)
        {
            elapsedTime = Time.time - startTime;
        }
    }

    public void ApplyGoalScore()
    {
        if (isGameCleared)
        {
            return;
        }
        isGameCleared = true;
        elapsedTime = Time.time - startTime;
        int baseScore = Mathf.RoundToInt(CalculateScore(elapsedTime));
        int penaltyScore = enemyHitCount * penaltyPerEnemyHit;
        lineScore = Mathf.Max(0, baseScore - penaltyScore);
        if (GManager.instance != null)
        {
            GManager.instance.lineScore = lineScore;
        }
    }

    public void AddEnemyHit()
    {
        enemyHitCount += 1;
    }

    public float CalculateScore(float timeSeconds)
    {
        // score = 20250 * exp(-k * t) + 1000
        // k = ln(1.5) / 10 so that t=20,30,40 map to ~10000,7000,5000
        float k = Mathf.Log(1.5f) / 10f;
        return 20250f * Mathf.Exp(-k * timeSeconds) + 1000f;
    }
}
