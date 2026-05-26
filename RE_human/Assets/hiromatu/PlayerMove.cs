using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // A,Dキー or ← →
        float moveInput = Input.GetAxisRaw("Horizontal");

        // 横移動
        rb.linearVelocity =
            new Vector2(moveInput * moveSpeed, 0);

    }
}