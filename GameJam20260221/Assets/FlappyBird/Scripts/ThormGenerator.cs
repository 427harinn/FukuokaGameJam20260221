using UnityEngine;

public class ThormGenerator : MonoBehaviour
{
    [SerializeField] private GameObject PrefabThorm;
    [SerializeField] private GameObject screenOutRightPos;
    [SerializeField] private PlayerFlappy player;
    [SerializeField] private int totalThormCount = 20;
    [SerializeField] private float minSpawnGapX = 3.2f;
    [SerializeField] private float maxSpawnGapX = 5.2f;
    [SerializeField] private float safeSwitchGapX = 6.0f;
    [SerializeField] private int minSameSideCount = 2;
    [SerializeField] private float topY = 3.1f;
    [SerializeField] private float bottomY = -3.3f;
    [SerializeField] private float topOffsetMinY = -0.4f;
    [SerializeField] private float topOffsetMaxY = 1.0f;
    [SerializeField] private float bottomOffsetMinY = -1.0f;
    [SerializeField] private float bottomOffsetMaxY = 0.4f;

    private int passedThormCount = 0;

    void Start()
    {
        SpawnInitialThorms();
    }

    private void SpawnInitialThorms()
    {
        if (PrefabThorm == null || player == null)
        {
            return;
        }

        int count = Mathf.Max(1, totalThormCount);
        float basePosX = GetSpawnBaseX();
        float minGap = Mathf.Min(minSpawnGapX, maxSpawnGapX);
        float maxGap = Mathf.Max(minSpawnGapX, maxSpawnGapX);
        float currentX = basePosX;
        bool lastSpawnTop = Random.Range(0, 100) <= 50;
        int sameSideRemaining = Mathf.Max(0, minSameSideCount - 1);

        for (int i = 0; i < count; i++)
        {
            bool nextTop = lastSpawnTop;
            if (sameSideRemaining > 0)
            {
                sameSideRemaining--;
            }
            else if (Random.Range(0, 100) <= 50)
            {
                nextTop = !lastSpawnTop;
                sameSideRemaining = Mathf.Max(0, minSameSideCount - 1);
            }

            float gapX = Random.Range(minGap, maxGap);
            if (nextTop != lastSpawnTop)
            {
                gapX = Mathf.Max(gapX, safeSwitchGapX);
            }
            if (i > 0)
            {
                currentX += gapX;
            }

            float topMin = Mathf.Min(topOffsetMinY, topOffsetMaxY);
            float topMax = Mathf.Max(topOffsetMinY, topOffsetMaxY);
            float bottomMin = Mathf.Min(bottomOffsetMinY, bottomOffsetMaxY);
            float bottomMax = Mathf.Max(bottomOffsetMinY, bottomOffsetMaxY);

            float posY = nextTop
                ? (topY + Random.Range(topMin, topMax))
                : (bottomY + Random.Range(bottomMin, bottomMax));
            Vector3 euler = nextTop ? new Vector3(0.0f, 0.0f, 180.0f) : Vector3.zero;

            GameObject thormObj = Instantiate(PrefabThorm, new Vector3(currentX, posY, 0.0f), Quaternion.Euler(euler));
            Thorm thorm = thormObj.GetComponent<Thorm>();
            if (thorm != null)
            {
                thorm.Initialize(player, this);
            }

            lastSpawnTop = nextTop;
        }
    }

    private float GetSpawnBaseX()
    {
        if (screenOutRightPos != null)
        {
            return screenOutRightPos.transform.position.x;
        }
        return transform.position.x + 8.0f;
    }

    public void OnThormPassed()
    {
        passedThormCount++;
        if (player != null && passedThormCount >= Mathf.Max(1, totalThormCount))
        {
            player.isGoal = true;
        }
    }
}
