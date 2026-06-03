using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int hp = 5;   // 3回当たると死亡

    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log("ダメージを受けた");
        Debug.Log("残りHP : " + hp);

        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("プレイヤー死亡");
        gameObject.SetActive(false);
    }
}
