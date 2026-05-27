using UnityEngine;

public class Boss4Controller : MonoBehaviour
{
    [Header("--- 必要な参照 ---")]
    public Transform playerTransform;   // プレイヤーの位置

    private Boss4MeleeAttack meleeAttack;
    private Boss4RangeAttack rangeAttack;
    private Boss4GrabAttack grabAttack;
    private Boss4Guard boss4Guard;
    private Rigidbody2D rb;

    [Header("--- ボスの基本ステータス ---")]
    public int bossHP = 100;            // ボスの体力
    public float moveSpeed = 2.0f;       // 移動速度

    [Header("--- 距離の設定（近い順） ---")]
    public float grabRange = 2.0f;        // つかみ攻撃の範囲 (2m以内)
    public float meleeRange = 5.0f;       // 近接攻撃の範囲 (5m以内)
    public float rangeAttackRange = 10.0f;// 遠距離攻撃の範囲 (10m以内)

    [Header("--- ガード（弾感知）の設定 ---")]
    public float bulletDetectRadius = 3.0f; // プレイヤーの弾に気づく範囲（レーダーの広さ）
    public LayerMask playerBulletLayer;     // ★プレイヤーの弾のレイヤーを指定


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        meleeAttack = GetComponent<Boss4MeleeAttack>();
        rangeAttack = GetComponent<Boss4RangeAttack>();
        grabAttack = GetComponent<Boss4GrabAttack>();
        boss4Guard = GetComponent<Boss4Guard>();
    }

    void Update()
    {
        // プレイヤーがいなければ何もしない
        if (playerTransform == null) return;

        // 「近接攻撃中」または「遠距離攻撃中」のどちらか一方でも true なら、
        // その場で速度を 0 にして、このフレームの処理（移動や次の攻撃）をすべてスキップする！
        if (meleeAttack.isAttacking == true || rangeAttack.isAttacking == true || grabAttack.isAttacking == true)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // 攻撃中じゃなければ、常にプレイヤーの方を向く
        LookAtPlayer();

        Collider2D incomingBullet = Physics2D.OverlapCircle(transform.position, bulletDetectRadius, playerBulletLayer);

        if (incomingBullet != null)
        {
            // 弾を発見した！
            if (boss4Guard.CanGuard() == true)
            {
                // 足を止めてガードを実行
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                boss4Guard.Execute();
                return; // ガードに入ったら、下の「攻撃・移動」の処理は今回はスキップ！
            }
        }



        // プレイヤーとの距離を計算する
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // --- 距離を最優先にしたAI判断 ---
        if (distance <= grabRange)
        {
            // 【2m以内：つかみ間合い】
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
                // ① 近接攻撃が打てるなら、その場で足を止めて殴る！
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                meleeAttack.Execute();
            }
            else
            {
                // ② 近接攻撃がクールタイム中なら、さらに内側の「つかみ範囲（2m）」を目指して近づく！
                MoveToPlayer();
            }
        }
        else if (distance <= rangeAttackRange)
        {
            // 【10m以内：遠距離間合い】近づきながら行動する
            MoveToPlayer();

            // 遠距離攻撃のクールタイムが明けていれば撃つ
            if (rangeAttack.CanAttack())
            {
                rangeAttack.Execute(playerTransform);
            }
        }
        else
        {
            // 【10m以上：範囲外】ひたすら追いかけるだけ
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
    // 弾を感知するレーダーの範囲をUnityの画面上に青い線で表示する
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, bulletDetectRadius);
    }
}
