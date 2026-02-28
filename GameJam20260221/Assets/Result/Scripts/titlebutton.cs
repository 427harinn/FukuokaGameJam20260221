using UnityEngine;

public class titlebutton : MonoBehaviour
{
    [SerializeField] private GameObject FadeIn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onClicked_title()
    {
        GManager.instance.flappyScore = 0;
        GManager.instance.funeScore = 0;
        GManager.instance.lineScore = 0;
        GManager.instance.killScore = 0;

        GameObject fade = Instantiate(FadeIn, Vector3.zero, Quaternion.identity);
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            fade.transform.SetParent(canvas.transform, false);
        }

        Invoke("sceneload", 2.5f);
    }
    
    public void sceneload()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
    }
}
