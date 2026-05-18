using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public int maxHP = 100;

    private int currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        Debug.Log("プレイヤーHP : " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("プレイヤー死亡");

        Destroy(gameObject);
    }
}