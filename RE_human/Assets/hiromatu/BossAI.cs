using UnityEngine;

public class BossAI : MonoBehaviour
{
    enum State
    {
        Idle,
        Move,
        Chase,
        MeleeAttack,
        DashAttack
    }

    State currentState;

    public float moveSpeed = 3f;
    public float chaseSpeed = 5f;

    public float moveRange = 5f;
    public float idleTime = 2f;

    public float meleeRange = 2f;
    public float dashRange = 5f;

    public float dashCooldown = 3f;

    //bool : 「はい/いいえ」を払わす変数
    bool canDash = true;

    bool isAttacking = false;

    // プレイヤー発見距離
    public float detectRange = 6f;

    float idleTimer;

    //Vector2 : 「2Dの位置や方向
    Vector2 targetPosition;

    //Rigidbody2D : 「物理演算」重力として使っている
    Rigidbody2D rb;

    //Transform : 「位置・回転・大きさ」
    Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Playerタグを探す
        player = GameObject.FindGameObjectWithTag("Player").transform;

        ChangeState(State.Idle);
    }

    void Update()
    {
        if (isAttacking)
        {
            return;
        }
        // プレイヤーとの距離
        float playerDistance =
            Vector2.Distance(transform.position, player.position);

        // 発見したら追尾
        if (playerDistance < meleeRange)
        {
            ChangeState(State.MeleeAttack);
        }
        else if (playerDistance < dashRange && canDash)
        {
            ChangeState(State.DashAttack);
        }
        else if (playerDistance < detectRange)
        {
            ChangeState(State.Chase);
        }

        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;

            case State.Move:
                Move();
                break;

            case State.Chase:
                Chase();
                break;
            case State.MeleeAttack:
                MeleeAttack();
                break;

            case State.DashAttack:
                DashAttack();
                break;
        }
    }

    void Idle()
    {
        //敵の速度を0にする
        //linearVelocity : 今の速度
        rb.linearVelocity = Vector2.zero;

        //待機時間を少しずつ減らしています
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0)
        {
            //ランダムな移動距離を作成
            //float : 少数を入れる変数
            float randomX =
                Random.Range(-moveRange, moveRange);
            
            //移動先を決めている
            // new Vector2 : 2Dの座標を作っている
            targetPosition = new Vector2(
                transform.position.x + randomX,
                transform.position.y
            );

            ChangeState(State.Move);
        }
    }

    void Move()
    {
        //右に進むか左に進むか
        //目標地点から現在位置を引いて右にあるか左にあるか確認
        //Mathf.Sign() : 数値の「符号」を返す　正or負or0
        float direction =
            Mathf.Sign(targetPosition.x - transform.position.x);

        //実際に移動する
        rb.linearVelocity =
            new Vector2(direction * moveSpeed, 0);

        //目標地点までの距離を調べる
        //Mathif.Abs() : 絶対値　右でも左でも距離は正の数にする
        float distance =
            Mathf.Abs(targetPosition.x - transform.position.x);

        //目標地点にかなり近づいたら
        //ゲームではピッタリ0にならないので0.1未満にしてる
        if (distance < 0.1f)
        {
            ChangeState(State.Idle);
        }
    }

    void Chase()
    {
        //プレイヤーが左右どちらにいるか調べる
        float direction =
            Mathf.Sign(player.position.x - transform.position.x);

        //プレイヤーの方向へ移動
        rb.linearVelocity =
            new Vector2(direction * chaseSpeed, 0);
    }

    void ChangeState(State newState)
    {
        //状態を変更する関数
        //現在状態を変更
        currentState = newState;

        //新しい状態がIdleなら
        if (newState == State.Idle)
        {
            idleTimer = idleTime;
        }
    }
    void MeleeAttack()
    {
        isAttacking = true;

        rb.linearVelocity = Vector2.zero;

        Debug.Log("近接攻撃");

        Invoke(nameof(EndMeleeAttack), 1f);
    }

    void EndMeleeAttack()
    {
        isAttacking = false;

        ChangeState(State.Idle);
    }
    void DashAttack()
    {
        isAttacking = true;

        canDash = false;

        rb.linearVelocity = Vector2.zero;

        Debug.Log("ため開始");

        Invoke(nameof(StartDash), 1f);
    }

    void StartDash()
    {
        float direction =
            Mathf.Sign(player.position.x - transform.position.x);

        rb.linearVelocity =
            new Vector2(direction * 25f, 0);

        Debug.Log("突進！");

        Invoke(nameof(StopDash), 0.5f);
    }

    void StopDash()
    {
        rb.linearVelocity = Vector2.zero;

        isAttacking = false;

        ChangeState(State.Idle);

        Invoke(nameof(ResetDash), dashCooldown);
    }

    void ResetDash()
    {
        canDash = true;
    }
}