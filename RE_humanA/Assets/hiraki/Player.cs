using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移動")]
    public float moveSpeed = 5f;

    [Header("ジャンプ")]
    public float jumpPower = 10f;

    private Rigidbody2D rb;

    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
        Jump();
    }

    // 左右移動
    void Move()
    {
        float x = 0f;

        // Aキー
        if (Input.GetKey(KeyCode.A))
        {
            x = -1f;
        }

        // Dキー
        if (Input.GetKey(KeyCode.D))
        {
            x = 1f;
        }

        rb.linearVelocity = new Vector2(
            x * moveSpeed,
            rb.linearVelocity.y
        );
    }

    // ジャンプ
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpPower
            );
        }
    }

    // 地面に触れた
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // 地面から離れた
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}