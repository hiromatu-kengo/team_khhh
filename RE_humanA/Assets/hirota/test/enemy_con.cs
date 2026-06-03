using UnityEngine;

public class enemy_con : MonoBehaviour
{
    int hp;
    int maxHp = 5;

    void Start()
    {
        hp = maxHp;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("LongAttack"))
        {
            hp--;

            Debug.Log("敵HP : " + hp);

            if (hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}