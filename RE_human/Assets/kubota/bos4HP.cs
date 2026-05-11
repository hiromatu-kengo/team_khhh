using UnityEngine;

public class bos4HP : MonoBehaviour
{
    public float maxHP = 1000f;
    float currentHP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float damage) // 🔥 ここ変更
    {
        currentHP -= damage;
        Debug.Log("攻撃をうけた");
        if (currentHP <= 0)
        {
            Die();
        }
    }
    // Update is called once per frame
    void Die()
    {
        Destroy(gameObject);
    }
}
