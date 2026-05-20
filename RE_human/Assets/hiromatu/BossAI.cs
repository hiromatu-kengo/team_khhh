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
    SpriteRenderer spriteRenderer;

    public float moveSpeed = 3f;
    public float chaseSpeed = 5f;

    public float moveRange = 5f;
    public float idleTime = 2f;

    public float meleeRange = 2f;
    public float dashRange = 5f;
    public float dashSpeed = 15f;
    public float dashTime = 0.5f;

    public float dashCooldown = 3f;

    //bool : 「はい/いいえ」を払わす変数
    bool canDash = true;

    bool isAttacking = false;

    //ボスが右を向いているのか
    bool isFacingRight = true;

    //突進攻撃の方向を固定する
    float dashDirection;

    // プレイヤー発見距離
    public float detectRange = 6f;

    float idleTimer;

    //近接攻撃の中心位置
    public Transform attackPoint;

    //近接攻撃の半径
    public float attackRadius = 1f;

    //プレイヤーのレイヤー
    public LayerMask playerLayer;

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
        //攻撃中はAIを停止する
        if (isAttacking)
        {
            return;
        }
        // プレイヤーとの距離
        float playerDistance =
            Vector2.Distance(transform.position, player.position);

        //近距離なら近接攻撃
        if (playerDistance < meleeRange)
        {
            ChangeState(State.MeleeAttack);
        }
        //中距離なら突進攻撃
        else if (playerDistance < dashRange && canDash)
        {
            ChangeState(State.DashAttack);
        }
        //プレイヤーを見つけたら追尾
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
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        //目標地点にかなり近づいたら待機
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
            //待機時間のリセット
            idleTimer = idleTime;
        }
    }
    void MeleeAttack()
    {
        //攻撃中のフラグをONにする
        isAttacking = true;

        //攻撃中の移動を停止する
        rb.linearVelocity = Vector2.zero;

        //攻撃判定を出す
        Collider2D hitPlayer =
            Physics2D.OverlapCircle(
                attackPoint.position,
                attackRadius,
                playerLayer
                );

        Debug.Log("近接攻撃");

        if(hitPlayer != null)
        {
            Debug.Log("近接攻撃ヒット");
        }

        //一秒後にEndMeleeAttackを実行する
        //時間差で実行
        Invoke(nameof(EndMeleeAttack), 1f);
    }

    void EndMeleeAttack()
    {
        //攻撃中のフラッグをOFFにする
        isAttacking = false;

        //待機状態へ戻す
        ChangeState(State.Idle);
    }
    void DashAttack()
    {
        //攻撃中
        isAttacking = true;

        //再突進を禁止する
        canDash = false;

        //一旦停止（攻撃をためている）
        rb.linearVelocity = Vector2.zero;

        //プレイ屋の方向を固定する
        dashDirection =
            Mathf.Sign(player.position.x - transform.position.x);

        //赤くする
        spriteRenderer.color = Color.red;

        Debug.Log("ため開始");

        //一秒後に突進開始
        Invoke(nameof(StartDash), 1f);
    }

    void StartDash()
    {
        //高速で突進する
        rb.linearVelocity =
            new Vector2(dashDirection * dashSpeed, 0);

        Debug.Log("突進！");

        //0.5秒後に停止
        Invoke(nameof(StopDash), dashTime);
    }

    void StopDash()
    {
        //停止
        rb.linearVelocity = Vector2.zero;

        //白に戻す
        spriteRenderer.color = Color.white;

        //攻撃中のフラッグを終える
        isAttacking = false;

        //待機状態に戻る
        ChangeState(State.Idle);

        //クールタイム開始
        Invoke(nameof(ResetDash), dashCooldown);
    }

    void ResetDash()
    {
        //突進攻撃の再使用を可能にする
        canDash = true;
    }

    void FacePlayer()
    {
        //プレイヤーが右にいる場合
        if (player.position.x > transform.position.x)
        {
            //右向き
            transform.localScale = new Vector3(1, 1, 1);

            //右向きの保存
            isFacingRight = true;
        }
        else
        {
            //左向き
            transform.localScale = new Vector3(-1, 1, 1);

            //左向きの保存
            isFacingRight = false;
        }
    }
    
    //Sceneビューで当たり判定を表示
    void OnDrawGizmosSelected()
    {
        // attackPointがない場合は終了
        if (attackPoint == null)
        {
            return;
        }

        // 赤色設定
        Gizmos.color = Color.red;

        // 攻撃範囲を円で表示
        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius
        );
    }
}