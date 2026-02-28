using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private float radius = 5.0f;
    [SerializeField] public float speed = 2.0f;
    [SerializeField] private float initializeAngle = 0.0f;
    [SerializeField] private float addPosY = 0.0f;
    [SerializeField] AudioClip soundClip;

    private float currentAngle = 0.0f;
    private float initCirclePosY = 0.0f;
    public bool isRotate = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentAngle = 0.0f * Mathf.Deg2Rad;
        initCirclePosY = Mathf.Cos(currentAngle) * radius;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("addPosY:" + addPosY);
        Debug.Log("isRotate" + isRotate);
        Debug.Log("nowPosY" + transform.position.y);

        NormalUpdatePosition();

        if (isRotate)
        {
            currentAngle += speed * Time.deltaTime;
            UpdateRotatePosition();
        }
    }

    void UpdateRotatePosition()
    {
        float newPosX = Mathf.Sin(currentAngle) * radius;
        float newPosY = Mathf.Cos(currentAngle) * radius;

        transform.position = new Vector3(newPosX, newPosY, 0);
    }

    void NormalUpdatePosition()
    {
        if (transform.position.y > initCirclePosY)
        {
            Vector3 addPos = new Vector3(0, addPosY, 0);
            transform.position += addPos * Time.deltaTime * speed;

            Debug.Log("addPosY:" + addPosY);
        }
        else
        {
            if(!isRotate)
            {
                AudioSource.PlayClipAtPoint(soundClip, transform.position);
            }
            isRotate = true;
            Debug.Log("���n�_��艺�ɍs�����F" + transform.position.y);
        }
    }
}
