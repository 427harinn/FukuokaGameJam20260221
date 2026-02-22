using UnityEngine;

public class EnemyFallMover : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 420.0f;
    [SerializeField] private float destroyY = -700.0f;
    [SerializeField] private float enemyHitboxShrinkX = 6.0f;
    [SerializeField] private float enemyHitboxShrinkY = 6.0f;

    private RectTransform rectTransform;
    private RectTransform playerRect;
    private EnemySpawner spawner;
    private readonly Vector3[] enemyCorners = new Vector3[4];
    private readonly Vector3[] playerCorners = new Vector3[4];

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (rectTransform == null)
        {
            return;
        }

        Vector2 pos = rectTransform.anchoredPosition;
        pos.y -= fallSpeed * Time.deltaTime;
        rectTransform.anchoredPosition = pos;

        if (playerRect != null && IsOverlapping(rectTransform, playerRect))
        {
            if (spawner != null)
            {
                spawner.NotifyPlayerHit();
            }
            Destroy(gameObject);
            return;
        }

        if (pos.y <= destroyY)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(EnemySpawner owner, RectTransform player)
    {
        spawner = owner;
        playerRect = player;
    }

    public void DetachSpawner()
    {
        spawner = null;
    }

    void OnDestroy()
    {
        if (spawner != null)
        {
            try
            {
                spawner.NotifyEnemyDestroyed();
            }
            catch (MissingReferenceException)
            {
                // Spawner already destroyed during scene transition.
            }
        }
    }

    private bool IsOverlapping(RectTransform a, RectTransform b)
    {
        a.GetWorldCorners(enemyCorners);
        b.GetWorldCorners(playerCorners);

        float enemyWidth = enemyCorners[2].x - enemyCorners[0].x;
        float enemyHeight = enemyCorners[2].y - enemyCorners[0].y;
        float safeShrinkX = Mathf.Clamp(enemyHitboxShrinkX, 0f, Mathf.Max(0f, enemyWidth * 0.49f));
        float safeShrinkY = Mathf.Clamp(enemyHitboxShrinkY, 0f, Mathf.Max(0f, enemyHeight * 0.49f));

        float aMinX = enemyCorners[0].x + safeShrinkX;
        float aMaxX = enemyCorners[2].x - safeShrinkX;
        float aMinY = enemyCorners[0].y + safeShrinkY;
        float aMaxY = enemyCorners[2].y - safeShrinkY;

        float bMinX = playerCorners[0].x;
        float bMaxX = playerCorners[2].x;
        float bMinY = playerCorners[0].y;
        float bMaxY = playerCorners[2].y;

        return aMinX < bMaxX && aMaxX > bMinX && aMinY < bMaxY && aMaxY > bMinY;
    }
}
