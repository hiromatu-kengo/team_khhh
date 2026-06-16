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
    public float grabRange = 3.5f;        // つかみ間合い（押し合いを考慮して3.5mに調整）
    public float meleeRange = 5.0f;       // 近接攻撃の範囲
    public float rangeAttackRange = 10.0f;// 遠距離攻撃の範囲

    [Header("--- ガード（弾感知）の設定 ---")]
    public float bulletDetectRadius = 4.0f; // プレイヤーの弾に気づく範囲（レーダーの広さ）
    public LayerMask playerBulletLayer;     // ★超重要：プレイヤーの弾のレイヤーだけを指定する

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
        // ① 最優先：自分の周りに「プレイヤーの弾」が来た時【だけ】ガード！
        // ====================================================
        Collider2D incomingBullet = Physics2D.OverlapCircle(transform.position, bulletDetectRadius, playerBulletLayer);

        if (incomingBullet != null)
        {
            if (guard.CanGuard() == true)
            {
                // 弾を見つけたので、足を止めてガードを展開！
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                guard.Execute();
                return; // ガードを始めたら、これ以降の移動や攻撃の処理は絶対にやらない！
            }
        }

        // ====================================================
        // ② それ以外（弾が来ていない時）：プレイヤーとの距離に応じた行動
        // ====================================================
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= grabRange)
        {
            // 【3.5m以内：つかみ間合い】
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (grabAttack.CanAttack())
            {
                grabAttack.Execute();
            }
        }
        else if (distance <= meleeRange)
        {
            // 【5m以内：近接間合い】
            if (meleeAttack.CanAttack())
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                meleeAttack.Execute();
            }
            else
            {
                // 近接攻撃がクールタイム中なら、つかみを目指してさらに近づく！
                MoveToPlayer();
            }
        }
        else if (distance <= rangeAttackRange)
        {
            // 【10m以内：遠距離間合い】近づきながら攻撃
            MoveToPlayer();

            if (rangeAttack.CanAttack())
            {
                rangeAttack.Execute(playerTransform);
            }
        }
        else
        {
            // 【10m以上：範囲外】ひたすら追いかける
            MoveToPlayer();
        }
    }

    void MoveToPlayer()
    {
        float direction = 1.0f;
        if (playerTransform.position.x < transform.position.x)
        {
            direction = -1.0f;
        }
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
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

    // 弾を感知するレーダーの範囲をUnityの画面上（Sceneビュー）に青い線で表示する
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, bulletDetectRadius);
    }
}