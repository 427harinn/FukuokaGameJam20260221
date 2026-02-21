using UnityEngine;

public class Line : MonoBehaviour
{
    private Vector2[] positions = new Vector2[2];
    private LineRenderer renderer;

    bool isDrawing = false;

    private Camera mainCamera;

    private EdgeCollider2D edgeCollider;

    [SerializeField] private GameObject FadeOut;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderer = GetComponent<LineRenderer>();
        renderer.useWorldSpace = false;
        mainCamera = Camera.main;
        edgeCollider = GetComponent<EdgeCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(FadeOut != null)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("マウスを押しました");
            positions[0] = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("マウスを離しました");
            positions[1] = Input.mousePosition;
            isDrawing = true;
        }

        if (isDrawing)
        {
            drawLine(positions[0], positions[1]);
            isDrawing = false;
        }
    }

    void drawLine(Vector3 positionDown, Vector3 positionUp)
    {
        edgeCollider.enabled = true;
        
        positionDown.z = 10f; // カメラからの距離（シーンに合わせて調整）
        positionUp.z = 10f;

        var p0 = mainCamera.ScreenToWorldPoint(positionDown);
        var p1 = mainCamera.ScreenToWorldPoint(positionUp);

        Vector3 local0 = transform.InverseTransformPoint(p0);
        Vector3 local1 = transform.InverseTransformPoint(p1);

        renderer.startWidth = 0.3f;
        renderer.endWidth = 0.3f;
        renderer.positionCount = 2;
        renderer.SetPosition(0, local0);
        renderer.SetPosition(1, local1);
        edgeCollider.points = new Vector2[]
        {
            local0,
            local1
        };
    }
}
