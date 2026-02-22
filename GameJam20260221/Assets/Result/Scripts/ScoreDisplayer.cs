using UnityEngine;

public class ScoreDisplayer : MonoBehaviour
{
    [SerializeField] Rotate[] ball;
    [SerializeField] GameObject score;
    [SerializeField] private float maxTimer = 0.0f;

    private float startTime = 0.0f;
    private float elapsedTime = 0.0f;
    private bool isStart = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (ball[8].isRotate)
        {
            if (!isStart)
            {
                startTime = Time.time;
                isStart = true;
            }

            elapsedTime = Time.time - startTime;
            Debug.Log("Œo‰ßŽžŠÔF" + elapsedTime);
        }

        if (elapsedTime >= maxTimer)
        {
            score.gameObject.SetActive(true);
            for (int i = 0; i < ball.Length; i++)
            {
                ball[i].speed += 0.4f * Time.deltaTime;
            }
        }
    }
}
