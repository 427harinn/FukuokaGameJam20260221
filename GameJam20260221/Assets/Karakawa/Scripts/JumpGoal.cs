using UnityEngine;
using UnityEngine.UI;

public class JumpGoal : MonoBehaviour
{
    public Collider2D collider;
    [SerializeField] private GameObject clear;
    [SerializeField] private GameObject FadeIn;
    private Collider2D pendingGoalCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
        {
            ScoreManager.instance.ApplyGoalScore();
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.up * 6f;
            }
            pendingGoalCollider = collision;
            this.gameObject.GetComponent<Image>().enabled = false;
            Invoke("Stop", 0.3f);
        }
    }

    private void Stop()
    {
        StageMove stageMove = GetComponent<StageMove>();
        if (stageMove != null)
        {
            stageMove.CenterStageNow();
        }
        Invoke("SENNAnimation", 0.8f);
    }

    private void SENNAnimation()
    {
        clear.SetActive(true);
        Invoke("FadeInAnimation", 0.8f);
    }

    private void FadeInAnimation()
    {
        GameObject fadein = Instantiate(FadeIn, Vector3.zero, Quaternion.identity);
        //fadeinの親オブジェクトをcanvasに
        fadein.transform.SetParent(GameObject.Find("Canvas").transform, false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (pendingGoalCollider != null && collision == pendingGoalCollider)
        {
            StartCoroutine(DisableGoalTriggerNextFrame(pendingGoalCollider));
            pendingGoalCollider = null;
        }
    }

    private System.Collections.IEnumerator DisableGoalTriggerNextFrame(Collider2D goalCollider)
    {
        yield return null;
        BoxCollider2D box = goalCollider.GetComponent<BoxCollider2D>();
        if (box != null)
        {
            box.isTrigger = false;
        }
    }
}
