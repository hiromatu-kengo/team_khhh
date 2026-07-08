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

    // ★追加：アニメーションの移動フラグを切り替えるため
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        meleeAttack = GetComponent<Boss4MeleeAttack>();
        rangeAttack = GetComponent<Boss4RangeAttack>();
        grabAttack = GetComponent<Boss4GrabAttack>();
        guard = GetComponent<Boss4Guard>();
        anim = GetComponent<Animator>(); // ★追加

        // ★追加：ゲームが始まった瞬間（0秒目）にも一度プレイヤーを向かせる
        LookAtPlayer();
    }

    void Update()
    {
        if (playerTransform == null) return;

        // ★修正1：攻撃中やガード中であっても、常にプレイヤーの方を向かせるために【一番上】に引っ越し！
        LookAtPlayer();

        // どれかのアクション中（攻撃またはガード中）なら、移動せずに処理をスキップ
        if (meleeAttack.isAttacking || rangeAttack.isAttacking || grabAttack.isAttacking || guard.isGuarding)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (anim != null) anim.SetBool("isMoving", false); // アクション中は移動アニメOFF
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
                if (anim != null) anim.SetBool("isMoving", false);
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
            if (anim != null) anim.SetBool("isMoving", false);
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
                if (anim != null) anim.SetBool("isMoving", false);
                meleeAttack.Execute();
            }
            else
            {
                MoveToPlayer();
            }
            UpdateAnimation();
        }
        else if (distance <= rangeAttackRange)
        {
            // 【遠距離間合い】
            if (rangeAttack.CanAttack())
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                if (anim != null) anim.SetBool("isMoving", false);
                rangeAttack.Execute(playerTransform);
            }
            else
            {
                MoveToPlayer();
            }
        }
        else
        {
            // 【範囲外】ひたすら追いかける
            MoveToPlayer();
        }
    }

    // ★修正3：移動の処理をシンプルにまとめました
    void MoveToPlayer()
    {
        // 向きは一番上の LookAtPlayer() が決めてくれているので、ここでは「進む方向」だけを計算します
        float moveDirection = 1f;

        if (playerTransform.position.x > transform.position.x)
        {
            moveDirection = 1f;  // プレイヤーが右にいるなら、右（プラス方向）に進む
        }
        else
        {
            moveDirection = -1f; // プレイヤーが左にいるなら、左（マイナス方向）に進む
        }

        // 実際にボスを歩かせる速度を設定（Unity 6仕様の linearVelocity）
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);

        // 移動アニメーションを再生する
        if (anim != null) anim.SetBool("isMoving", true);
    }

    // ★修正2：元々のコードだと MoveToPlayer とプラスマイナスが逆になっていたので統一しました！
    void LookAtPlayer()
    {
        if (playerTransform.position.x > transform.position.x)
        {
            // プレイヤーが右にいる時は、-1 にして右を向かせる
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            // プレイヤーが左にいる時は、1 にして左を向かせる
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void UpdateAnimation()
    {
        if (anim == null) return;

        // もし横方向の速度（絶対値）が 0.1 より大きければ「歩いている（true）」、そうでなければ「止まっている（false）」
        bool IsChasing = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        anim.SetBool("IsChasing", IsChasing);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, bulletDetectRadius);
    }
}