using UnityEngine;

public class toNextSceneScript : MonoBehaviour
{
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private GameObject fadeInPrefab;
    [SerializeField] private float clearDelay = 0.4f;
    [SerializeField] private float fadeDelayAfterClear = 1.6f;
    [SerializeField] private float sceneChangeDelayAfterFade = 2.5f;
    [SerializeField] private string nextSceneName = "Sengen";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FinishAnimation()
    {
        if (clearPanel != null)
        {
            clearPanel.SetActive(true);
        }
        if (fadeInPrefab != null)
        {
            Invoke(nameof(SpawnFadeIn), fadeDelayAfterClear);
        }
        Invoke(nameof(LoadNextScene), sceneChangeDelayAfterFade);
    }

    private void SpawnFadeIn()
    {
        //親オブジェクトをcanvas
        GameObject fadeInInstance = Instantiate(fadeInPrefab, Vector3.zero, Quaternion.identity);
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            fadeInInstance.transform.SetParent(canvas.transform, false);
        }
    }

    private void LoadNextScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }



}
