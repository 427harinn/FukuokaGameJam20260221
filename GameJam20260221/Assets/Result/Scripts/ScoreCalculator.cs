using UnityEngine;
using UnityEngine.UI;
using unityroom.Api;

public class ScoreCalculator : MonoBehaviour
{
    public Text scoreText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int sumScore = GManager.instance.flappyScore +
                    GManager.instance.funeScore +
                    GManager.instance.lineScore +
                    GManager.instance.killScore;

        scoreText.text = sumScore.ToString();
        UnityroomApiClient.Instance.SendScore(1, sumScore, ScoreboardWriteMode.HighScoreAsc);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
