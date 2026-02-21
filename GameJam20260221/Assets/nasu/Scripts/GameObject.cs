using UnityEngine;

public class ImageScroller : MonoBehaviour
{
    public float speed = 2.0f;

    [SerializeField] GameObject FadeOut;
    void Update()
    {
        if(FadeOut != null)
        {
            return;
        }
        transform.position += Vector3.left * speed * Time.deltaTime;
    }
    
}
