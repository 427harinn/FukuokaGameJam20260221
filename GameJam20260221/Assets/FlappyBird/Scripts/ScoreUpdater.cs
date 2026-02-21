using UnityEngine;

public class ScoreUpdater : MonoBehaviour
{
    private float startTime = 0.0f;
    private float elapsedTime = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime = Time.time - startTime;

    }
}
