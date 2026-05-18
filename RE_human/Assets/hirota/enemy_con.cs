using UnityEngine;

public class enemy_con : MonoBehaviour
{
    int hp = 0;

    int maxHp = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Meleeattack"))
        {
            //HPが減る
            hp--;
            Debug.Log("ss");
        }

        if (hp <= 0)
        {
            Destroy(gameObject);
        }


    }
}
