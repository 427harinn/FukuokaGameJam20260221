using UnityEngine;

public class Thorm : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float destroyMarginX = 1.0f;
    private ThormGenerator generator;
    private Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;

        Collider2D[] thormColliders = GetComponentsInChildren<Collider2D>(true);
        for (int i = 0; i < thormColliders.Length; i++)
        {
            Collider2D thormCollider = thormColliders[i];
            if (thormCollider == null)
            {
                continue;
            }

            thormCollider.isTrigger = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera == null)
        {
            return;
        }

        float leftEdgeX = GetCameraLeftEdgeX(mainCamera);
        if (transform.position.x < leftEdgeX - destroyMarginX)
        {
            Destroy(this.gameObject);
        }
    }

    private float GetCameraLeftEdgeX(Camera cam)
    {
        if (cam.orthographic)
        {
            return cam.transform.position.x - (cam.orthographicSize * cam.aspect);
        }

        // Fallback for perspective camera
        Vector3 viewPoint = cam.ViewportToWorldPoint(new Vector3(0.0f, 0.5f, Mathf.Abs(cam.transform.position.z)));
        return viewPoint.x;
    }

    public void Initialize(PlayerFlappy playerFlappy, ThormGenerator thormGenerator)
    {
        if (playerFlappy != null)
        {
            player = playerFlappy.gameObject;
        }
        generator = thormGenerator;
    }

    void OnDestroy()
    {
        if (generator != null)
        {
            generator.OnThormPassed();
        }
    }
}
