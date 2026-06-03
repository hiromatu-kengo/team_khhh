using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    public float moveSpeed = 6f;   // 移動速度
    public float jumpForce = 12f;  // ジャンプの力

    Rigidbody2D rb;
    bool isGrounded; // 地面に足がついているか（無限ジャンプ防止フラグ）

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // ① 左右の移動入力を受け取る（A/Dキー、または矢印キー）
        float moveX = Input.GetAxisRaw("Horizontal");

        // 左右の移動速度を適用（Y方向の速度は現在の重力をそのまま維持）
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);

        // ② ジャンプの入力（スペースキー）があり、かつ地面にいるとき
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // 上方向への速度を与える
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            // 空中に浮いたのでフラグをfalseにする
            isGrounded = false;
            Debug.Log("jamp");
        }
    }

    // ③ 地面（コライダー）に触れた瞬間の判定
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 触れたオブジェクトのタグが「Ground」なら、地面に着地したとみなす
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}