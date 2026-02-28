using UnityEngine;

public class OnClickTutorial : MonoBehaviour
{
    [SerializeField] GameObject tutorial;
    [SerializeField] GameObject credit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnButtonClick_tutorialclose()
    {
        tutorial.SetActive(false);
    }

    public void OnButtonClick_creditopen()
    {
        credit.SetActive(true);
    }

    public void OnButtonClick_creditclose()
    {
        credit.SetActive(false);
    }
    public void OnButtonClick_tutorialopen()
    {
        tutorial.SetActive(true);
    }
}