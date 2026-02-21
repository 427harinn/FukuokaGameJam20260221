using UnityEngine;
using System.Collections; 

public class PowerEnemyHealth : MonoBehaviour
{
    public int hp = 3; // 敵ごとの体力
    [SerializeField] private GameObject deathImage; // 死亡時に出す画像
    [SerializeField] private float displayTime = 1.5f; // 画像を表示しておく時間

    bool deathImageFlag = false;
    float deathImageTimer = 0.5f;

    void Update()
    {
        if(deathImageFlag)
        {
            deathImageTimer -= Time.deltaTime;
            if(deathImageTimer<=0)
            {
                Destroy(gameObject);
            }
        }
    }

    // ダメージを受ける関数
    public void TakeDamage()
    {
        if (hp <= 0) return;
        hp--;
        Debug.Log(gameObject.name + "の残りHP: " + hp);

        if (hp <= 0)
        {
            deathImageFlag = true;
            if (deathImage != null)
            {
                GameObject tmp = Instantiate(deathImage);
                Destroy(tmp, deathImageTimer);
            }
            else
            {
                Debug.LogWarning("deathImage is not assigned on PowerEnemyHealth.", this);
            }
            //deathImage.SetActive(true); // 画像を表示！
           //StartCoroutine(ShowAndHideDeathImage());
        }
    }
}
