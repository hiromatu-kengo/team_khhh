using UnityEngine;

public class Boss4Controller : MonoBehaviour
{
    [Header("--- 参照 ---")]
    public Transform playerTransform;
    private Rigidbody2D rb; // ★物理移動用のコンポーネント

    // 分割した攻撃スクリプトたち
    private Boss4MeleeAttack meleeAttack;
    private Boss4RangeAttack rangedAttack;
    private Boss4GrabAttack grabAttack;

    [Header("--- 設定 ---")]
    public float attackRange = 3.0f;
    public float moveSpeed = 2.0f;   // ★ボスの歩くスピード

    // ガードの変数はそのまま
    private bool isGuarding = false;

    // 現在ボスが行動（攻撃やガード）中かどうか
    public bool IsBusy => isGuarding || meleeAttack.IsAttacking || rangedAttack.IsAttacking || grabAttack.IsAttacking;

    void Start()
    {
        // 同じGameObjectについているスクリプトを自動取得
        rb = GetComponent<Rigidbody2D>(); // ★追加
        meleeAttack = GetComponent<Boss4MeleeAttack>();
        rangedAttack = GetComponent<Boss4RangeAttack>();
        grabAttack = GetComponent<Boss4GrabAttack>();
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 1. 向きの制御：攻撃中やガード中でなければ、常にプレイヤーの方を向く
        if (!IsBusy)
        {
            LookAtPlayer();
        }

        // 2. 行動中（攻撃モーション中など）なら、ここから下の移動やAI判断はスキップ
        if (IsBusy) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // 「このフレームで攻撃を発動したか」を記録するフラグ
        bool didAttack = false;

        // 3. 攻撃の判定
        if (distance <= attackRange)
        {
            // 近距離の時：つかみクールのタイマーが0なら確率でつかみ、ダメなら近接
            if (grabAttack.CanAttack() && Random.Range(0, 100) < 50)
            {
                grabAttack.Execute(playerTransform);
                didAttack = true; // 攻撃した！
            }
            else if (meleeAttack.CanAttack())
            {
                meleeAttack.Execute();
                didAttack = true; // 攻撃した！
            }
        }
        else
        {
            // 遠距離の時
            if (rangedAttack.CanAttack())
            {
                rangedAttack.Execute(playerTransform);
                didAttack = true; // 攻撃した！
            }
        }

        // 4. ★ここが核心！ 攻撃しなかった（＝すべての攻撃がクールタイム中）なら移動する
        if (!didAttack)
        {
            MoveToPlayer();
        }
        else
        {
            // 攻撃の予兆（構え）に入った瞬間は、滑らないように足をピタッと止める
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    // ★追加：プレイヤーに向かって歩く処理
    void MoveToPlayer()
    {
        // プレイヤーが右にいるなら 1、左にいるなら -1
        float direction = playerTransform.position.x > transform.position.x ? 1 : -1;

        // Unity 6最新仕様の linearVelocity で横移動！
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    // ★追加：プレイヤーの方を向く処理
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