using UnityEngine;

public class Boss4Controller : MonoBehaviour
{
    [Header("--- 必要な参照 ---")]
    public Transform playerTransform;   // プレイヤーの位置

    private Boss4MeleeAttack meleeAttack;
    private Boss4RangeAttack rangeAttack;
    private Boss4GrabAttack gradAttack;
    private Rigidbody2D rb;

    [Header("--- ボスの基本ステータス ---")]
    public int bossHP = 100;            // ボスの体力
    public float moveSpeed = 2.0f;       // 移動速度
    public float attackRange = 5.0f;     // 近接攻撃に切り替わる距離

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        meleeAttack = GetComponent<Boss4MeleeAttack>();
        rangeAttack = GetComponent<Boss4RangeAttack>();
        gradAttack = GetComponent<Boss4GrabAttack>();
    }

    void Update()
    {
        // プレイヤーがいなければ何もしない
        if (playerTransform == null) return;

        // 「近接攻撃中」または「遠距離攻撃中」のどちらか一方でも true なら、
        // その場で速度を 0 にして、このフレームの処理（移動や次の攻撃）をすべてスキップする！
        if (meleeAttack.isAttacking == true || rangeAttack.isAttacking == true || gradAttack.IsAttacking == true)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // 攻撃中じゃなければ、常にプレイヤーの方を向く
        LookAtPlayer();

        // プレイヤーとの距離を計算する
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // --- 距離を最優先にしたAI判断 ---
        if (distance > attackRange)
        {
            // 【攻撃範囲外】クールタイム中はプレイヤーを追いかける
            MoveToPlayer();

            // 追いかけながら、もし遠距離攻撃のクールタイムが明ければその場で（次のフレームから足を止めて）発動！
            if (rangeAttack.CanAttack() == true)
            {
                rangeAttack.Execute(playerTransform);
            }
        }
        else
        {
            // 【攻撃範囲内（近接の間合い）】めり込まないように足を止める
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            // 近接攻撃のクールタイムが明けていれば殴る！
            if (meleeAttack.CanAttack() == true)
            {
                meleeAttack.Execute();
            }
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
}