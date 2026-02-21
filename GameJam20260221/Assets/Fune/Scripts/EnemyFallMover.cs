using UnityEngine;

public class EnemyFallMover : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 420.0f;
    [SerializeField] private float destroyY = -700.0f;

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

        float aMinX = enemyCorners[0].x;
        float aMaxX = enemyCorners[2].x;
        float aMinY = enemyCorners[0].y;
        float aMaxY = enemyCorners[2].y;

        float bMinX = playerCorners[0].x;
        float bMaxX = playerCorners[2].x;
        float bMinY = playerCorners[0].y;
        float bMaxY = playerCorners[2].y;

        return aMinX < bMaxX && aMaxX > bMinX && aMinY < bMaxY && aMaxY > bMinY;
    }
}
