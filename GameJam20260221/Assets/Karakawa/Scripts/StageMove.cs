using UnityEngine;

public class StageMove : MonoBehaviour
{
    GameObject moveStage;
    private Camera mainCamera;

    bool isMoving = false;
    float targetStageY = 0f;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float moveCooldown = 0.3f;
    private float cooldownTimer = 0f;
    [SerializeField] private float centerOffsetY = 2.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveStage = this.gameObject.transform.parent.gameObject;
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (isMoving)
        {
            MoveStageTowards(targetStageY);
            if (Mathf.Approximately(moveStage.transform.position.y, targetStageY))
            {
                isMoving = false;
                cooldownTimer = moveCooldown;
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isMoving)
        {
            return;
        }
        if (cooldownTimer > 0f)
        {
            return;
        }
        if (collision.gameObject.CompareTag("WallTop"))
        {
            StartMoveToCenter();
        }
    }

    private void StartMoveToCenter()
    {
        targetStageY = GetTargetStageY();
        isMoving = true;
    }

    public void CenterStageNow()
    {
        targetStageY = GetTargetStageY();
        isMoving = false;
        MoveStageTowards(targetStageY);
    }

    private float GetTargetStageY()
    {
        float deltaY = this.gameObject.transform.position.y - (mainCamera.transform.position.y + centerOffsetY);
        return moveStage.transform.position.y - deltaY;
    }

    private void MoveStageTowards(float targetY)
    {
        float newY = Mathf.MoveTowards(
            moveStage.transform.position.y,
            targetY,
            moveSpeed * Time.deltaTime
        );
        moveStage.transform.position = new Vector3(
            moveStage.transform.position.x,
            newY,
            moveStage.transform.position.z
        );
    }

}
