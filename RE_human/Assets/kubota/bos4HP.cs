using UnityEngine;

public class bos4HP : MonoBehaviour
{
    public int maxHP = 100;

    public int currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        Debug.Log("Boss HP : " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Boss Dead");

        Destroy(gameObject);
    }
}
