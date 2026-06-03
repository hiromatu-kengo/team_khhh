using UnityEngine;


// 物理演算を使うため、Rigidbody2Dコンポーネントを必須にする
[RequireComponent(typeof(Rigidbody2D))]
public class Boss3PlayerControl : MonoBehaviour
{
    [Header("移動・ジャンプ設定")]
    public float moveSpeed = 6.0f;
    public float jumpForce = 12.0f;

    [Header("接地判定設定")]
    public Transform groundCheck; // 足元のGroundCheckオブジェクト
    public LayerMask groundLayer; // 地面レイヤー
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 物理演算でプレイヤーが転倒しないように回転をフリーズ
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // 1. 左右の入力を取得 (A/Dキー、または左右の矢印キー)
        // GetAxisRawを使うことで、入力の遊び（慣性）をなくしキビキビ動かせます
        moveInput = Input.GetAxisRaw("Horizontal");

        // 2. 移動方向に合わせてプレイヤーの向き（左右）を変える
        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // 3. 足元が地面に触れているかチェック
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // 4. ジャンプ入力（Spaceキー）の検知
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Y軸（上方向）に瞬間的に速度を与える
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void FixedUpdate()
    {
        // 5. 物理移動の適用（左右の速度を更新、上下は現在の速度を維持）
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    // インスペクターで接地判定の円を確認用（デバッグ用）
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}