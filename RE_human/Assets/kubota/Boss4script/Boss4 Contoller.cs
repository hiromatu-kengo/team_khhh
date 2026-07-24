using UnityEngine;

public class Boss4Controller : MonoBehaviour
{
    [Header("--- 必要な参照 ---")]
    public Transform playerTransform;   // プレイヤーの位置

    private Boss4MeleeAttack meleeAttack;
    private Boss4RangeAttack rangeAttack;
    private Boss4GrabAttack grabAttack;
    private Boss4Guard guard;
    private Rigidbody2D rb;
    private Animator anim;
    private bos4HP bossHp;

    [Header("--- ボスの基本ステータス ---")]
    public float moveSpeed = 2.0f;

    [Header("--- 距離の設定（近い順） ---")]
    public float grabRange = 3.5f;        // つかみ間合い
    public float meleeRange = 5.0f;       // 近接攻撃の範囲
    public float rangeAttackRange = 10.0f;// 遠距離攻撃の範囲

    [Header("--- ガード（弾感知）の設定 ---")]
    public float bulletDetectRadius = 4.0f;
    public LayerMask playerBulletLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        meleeAttack = GetComponent<Boss4MeleeAttack>();
        rangeAttack = GetComponent<Boss4RangeAttack>();
        grabAttack = GetComponent<Boss4GrabAttack>();
        guard = GetComponent<Boss4Guard>();
        anim = GetComponent<Animator>();
        bossHp = GetComponent<bos4HP>();

        LookAtPlayer();
    }

    void Update()
    {

        if (bossHp != null && bossHp.isDead)
        {
            rb.linearVelocity = Vector2.zero; // 動きを完全に止める
            return; // 死亡中はUpdateの処理をここでおしまいにする
        }

        if (playerTransform == null) return;

        // 常にプレイヤーの方を向かせる
        LookAtPlayer();

        // どれかのアクション中（攻撃またはガード中）なら、移動せずに処理をスキップ
        if ((meleeAttack != null && meleeAttack.isAttacking) ||
            (rangeAttack != null && rangeAttack.isAttacking) ||
            (grabAttack != null && grabAttack.isAttacking) ||
            (guard != null && guard.isGuarding))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            UpdateAnimation();
            return;
        }

        // ====================================================
        // ① 最優先：ガード処理
        // ====================================================
        Collider2D incomingBullet = Physics2D.OverlapCircle(transform.position, bulletDetectRadius, playerBulletLayer);

        if (incomingBullet != null)
        {
            if (guard.CanGuard() == true)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                guard.Execute();
                UpdateAnimation();
                return;
            }
        }

        // ====================================================
        // ② それ以外：プレイヤーとの距離に応じた行動
        // ====================================================
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= grabRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            // 1. つかみ攻撃ができるなら「つかみ」
            if (grabAttack != null && grabAttack.CanAttack())
            {
                grabAttack.Execute();
            }
            // 2. つかみが無理で、近接攻撃ができるなら「近接攻撃」
            else if (meleeAttack != null && meleeAttack.CanAttack())
            {
                meleeAttack.Execute();
            }
        }
        else if (distance <= meleeRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            if (meleeAttack != null && meleeAttack.CanAttack())
            {
                meleeAttack.Execute();
            }
        }
        else if (distance <= rangeAttackRange)
        {
            if (rangeAttack != null && rangeAttack.CanAttack())
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                rangeAttack.Execute(playerTransform);
            }
            else
            {
                MoveToPlayer();
            }
        }
        else
        {
            MoveToPlayer();
        }

        // 最後にアニメーションを自動切り替え
        UpdateAnimation();
    }

    void MoveToPlayer()
    {
        float moveDirection = (playerTransform.position.x > transform.position.x) ? 1f : -1f;
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
    }

    void LookAtPlayer()
    {
        if (playerTransform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void UpdateAnimation()
    {
        if (anim == null) return;

        if ((meleeAttack != null && meleeAttack.isAttacking) ||
            (rangeAttack != null && rangeAttack.isAttacking) ||
            (grabAttack != null && grabAttack.isAttacking) ||
            (guard != null && guard.isGuarding))
        {
            anim.SetBool("IsChasing", false);
            return;
        }

        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        anim.SetBool("IsChasing", isMoving);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, bulletDetectRadius);
    }
}