using UnityEngine;
using System.Collections;

public class ZakoJump : MonoBehaviour
{
    public float speed = -2f;
    private Rigidbody2D rb;
    public float jumpForce = 5f;
    public float jumpInterval = 3f;
    private float jumpTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        jumpTimer += Time.deltaTime;

        // 設定した間隔（秒）が経ったらジャンプする
        if (jumpTimer >= jumpInterval)
        {
            Jump();
            jumpTimer = 0f; // タイマーをゼロに戻す
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
    }
    void Jump()
    {
        // 瞬間的な力を上方向（Vector2.up）に加える
        // ForceMode2D.Impulse を使うと、質量に関係なくピョンと跳ねてくれるぞ
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            Destroy(gameObject);
        }
        if (collision.CompareTag("LongAttack"))
        {
            Destroy(gameObject);
        }
    }
}
