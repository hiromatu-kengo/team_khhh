using UnityEngine;
public class player_ : MonoBehaviour
{
    Rigidbody2D rigid2D;

    Vector2 move;

    //飛んでいくスピード
    float speed = 10.0f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        float direction = Mathf.Sign(transform.localScale.x);

        rigid2D.linearVelocity = new Vector2(speed * direction, rigid2D.linearVelocity.y);


    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }

    }
}
