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
    }

    void Update()
    {
        if (playerTransform == null) return;

        // どれかのアクション中（攻撃またはガード中）なら、移動せずに処理をスキップ
        if (meleeAttack.isAttacking || rangeAttack.isAttacking || grabAttack.isAttacking || guard.isGuarding)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        LookAtPlayer();

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
                return;
            }
        }

        // ====================================================
        // ② それ以外：プレイヤーとの距離に応じた行動
        // ====================================================
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= grabRange)
        {
            // 【つかみ間合い】
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (grabAttack.CanAttack())
            {
                grabAttack.Execute();
            }
        }
        else if (distance <= meleeRange)
        {
            // 【近接間合い】
            if (meleeAttack.CanAttack())
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                meleeAttack.Execute();
            }
            else
            {
                // 近接が打てないなら近づく
                MoveToPlayer();
            }
        }
        else if (distance <= rangeAttackRange)
        {
            // 【遠距離間合い】
            // ★修正：遠距離攻撃ができるなら、その場で足を止めて実行！移動させない！
            if (rangeAttack.CanAttack())
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                rangeAttack.Execute(playerTransform);
            }
            else
            {
                // 攻撃がクールタイム中（無理な時）だけ近づく！
                MoveToPlayer();
            }
        }
        else
        {
            // 【範囲外】ひたすら追いかける
            MoveToPlayer();
        }
    }

    void MoveToPlayer()
    {
        if (playerTransform.position.x > transform.position.x)
        {
            // プレイヤーが右にいる時は、-1 にして右を向かせる
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            // プレイヤーが左にいる時は、1（元々の向き）にして左を向かせる
            transform.localScale = new Vector3(1, 1, 1);
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, bulletDetectRadius);
    }
}