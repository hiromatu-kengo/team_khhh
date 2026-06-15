using UnityEngine;

public class ZakoYoko : MonoBehaviour
{
    public float speed = -2f;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("PlayerAttack")) 
        { 
            Destroy(gameObject);
        }
        if(collision.CompareTag("LongAttack"))
        {
            Destroy(gameObject);
        }
    }
}
