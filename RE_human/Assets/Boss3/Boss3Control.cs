using UnityEngine;

// 物理演算を使うため、Rigidbody2Dコンポーネントを必須にする
[RequireComponent(typeof(Rigidbody2D))]
public class Boss3Control : MonoBehaviour
{
    public Transform playerTransform;
    private Boss3RangeAttack RangeAttack;

    [Header("移動設定")]
    public float moveSpeed = 5.0f; // 物理移動用に少し値を上げると良いです
    public float rangeAttackRange = 10.0f;

    [Header("離れるしきい値")]
    public float escapeRange = 5.0f;

    [Header("移動範囲の制限")]
    public float minX; // 移動可能な最小X座標
    public float maxX; // 移動可能な最大X座標

    [Header("ジャンプ設定（プレイヤーを超える）")]
    public float jumpForceX = 8f;  // プレイヤーを超えるための横方向の力
    public float jumpForceY = 12f; // 飛び上がる縦方向の力

    [Header("接地判定設定")]
    public Transform groundCheck; // 足元に置いたGroundCheckオブジェクトのTransform
    public LayerMask groundLayer; // 地面として扱うレイヤー
    public float groundCheckRadius = 0.2f; // 接地判定の円の半径

    private Rigidbody2D rb;
    private bool isGrounded; // 地面にいるか
    private bool isJumping;   // ジャンプ中か

    // Start is called once before the first execution of Update
    void Start()
    {
        RangeAttack = GetComponent<Boss3RangeAttack>();
        rb = GetComponent<Rigidbody2D>();

        // 必要に応じて、物理演算で回転しないようにフリーズさせる
        // rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform == null) return;

        // 1. 接地判定を行う
        CheckGrounded();

        // 2. 常にプレイヤーの方を向く（既存）
        LookAtPlayer();

        // 3. プレイヤーとの距離を計算（既存）
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // 4. ジャンプ中でないなら、行動を決定する
        if (!isJumping)
        {
            if (distance < escapeRange)
            {
                // 近づきすぎなので、離れるかジャンプするかを判断
                TryEscapeOrJump();
            }
            else
            {
                // 遠距離攻撃の適正距離にいる場合
                // 逃げる移動を止める（y軸の速度は維持）
                if (isGrounded)
                {
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                }
            }
        }
    }

    // 物理演算系の Clamp (位置制限) はFixedUpdateで行うと安定する
    void FixedUpdate()
    {
        // 移動範囲の Clamp（ボスのX位置をminXとmaxXの間に収める）
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);

        // ジャンプ中に着地した時の処理
        if (isJumping && isGrounded && rb.linearVelocity.y <= 0)
        {
            isJumping = false; // ジャンプ終了
        }
    }

    void LookAtPlayer()
    {
        if (playerTransform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    // 地面に着いているか確認する
    void CheckGrounded()
    {
        if (groundCheck == null) return;
        // 足元のgroundCheckの位置を中心に、小さな円で地面レイヤーと衝突しているか見る
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // プレイヤーから逃げるか、追い詰められてジャンプするかを決める
    void TryEscapeOrJump()
    {
        // 地面にいるときだけ逃走とジャンプができる
        if (!isGrounded) return;

        // プレイヤーとは「逆」の方向（X軸）を計算 (前回のロジック)
        float escapeDirectionX = transform.position.x - playerTransform.position.x;
        float escapeMoveDir = Mathf.Sign(escapeDirectionX);

        // プレイヤーがいる方向（ジャンプ用）
        float playerDir = Mathf.Sign(playerTransform.position.x - transform.position.x);

        // --- 追い詰められた判定 ---
        // 「左に逃げようとしていて、かつminXに到達している」
        // または 「右に逃げようとしていて、かつmaxXに到達している」
        bool isCornered = (escapeMoveDir == -1 && transform.position.x <= minX + 0.1f) ||
                          (escapeMoveDir == 1 && transform.position.x >= maxX - 0.1f);

        if (isCornered)
        {
            // 追い詰められた！プレイヤーを超えるジャンプを実行
            JumpOverPlayer(playerDir);
        }
        else
        {
            // 追い詰められていない。通常の逃げ移動。
            // Rigidbody2Dに速度を与える
            rb.linearVelocity = new Vector2(escapeMoveDir * moveSpeed, rb.linearVelocity.y);
        }
    }

    // プレイヤーがいる方向へ向かって、大きくジャンプする
    void JumpOverPlayer(float jumpXDir)
    {
        isJumping = true; // ジャンプ開始
        rb.linearVelocity = Vector2.zero; // 一度現在の移動速度をリセット

        // X（プレイヤー方向）とY（上）に力を加える (Impulseは瞬間的な力)
        rb.AddForce(new Vector2(jumpXDir * jumpForceX, jumpForceY), ForceMode2D.Impulse);
    }

    // インスペクターで接地判定の円を確認できるようにする（デバッグ用）
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}