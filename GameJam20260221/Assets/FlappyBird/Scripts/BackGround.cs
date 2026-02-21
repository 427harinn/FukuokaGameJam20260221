using UnityEngine;

public class BackGround : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float moveX = 3.0f;
    [SerializeField] private MoveCamera moveCamera;
    [SerializeField] private float cameraFollowRate = 0.3f;
    [SerializeField] private bool moveWithCamera = true;
    [SerializeField] private float loopWidth = 19.2f;
    [SerializeField] private bool useLoop = true;
    private float startX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float currentMoveX = Mathf.Max(0.0f, moveX);
        bool toRight = false;

        if (moveCamera != null)
        {
            currentMoveX = Mathf.Max(0.0f, moveCamera.CurrentMoveX * cameraFollowRate);
            toRight = moveWithCamera;
        }
        
        Vector3 addPos = new Vector3(currentMoveX, 0, 0);

        if (toRight)
        {
            transform.position += addPos * (speed * Time.deltaTime);
        }
        else
        {
            transform.position -= addPos * (speed * Time.deltaTime);
        }

        if (!useLoop)
        {
            return;
        }

        if (toRight && transform.position.x >= startX + loopWidth)
        {
            transform.position -= new Vector3(loopWidth * 2.0f, 0.0f, 0.0f);
        }
        else if (!toRight && transform.position.x <= startX - loopWidth)
        {
            transform.position += new Vector3(loopWidth * 2.0f, 0.0f, 0.0f);
        }
    }
}
