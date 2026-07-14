using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss1AI : MonoBehaviour
{
    enum State
    {
        Idle,   // 待機
        Move,   // 移動
        Chase,  // 追跡
        MeleeAttack, // 近接攻撃
        DashAttack, // ダッシュ攻撃
        Die        //死亡状態

    }

    State currentState; // 現在の状態
    SpriteRenderer spriteRenderer; // スプライトレンダラー

    public float moveSpeed = 3f; // 移動速度
    public float chaseSpeed = 5f; // 追跡速度

    public float moveRange = 5f; // 待機中の移動範囲
    public float idleTime = 2f; // 待機時間

    public float meleeRange = 2f; // 近接攻撃の範囲
    public float dashRange = 5f; // ダッシュ攻撃の範囲
    public float dashSpeed = 15f; // ダッシュ攻撃の速度
    public float dashTime = 0.5f; // ダッシュ攻撃の持続時間

    public float dashCooldown = 3f; // ダッシュ攻撃のクールダウン時間

    public GameObject attackEffect;

    //bool : 「はい/いいえ」を払わす変数

    bool canDash = true; // ダッシュ攻撃が可能かどうか
    bool isAttacking = false; // 攻撃中かどうか
    bool isFacingRight = true; // ボスが右を向いているかどうか
    float dashDirection; // ダッシュ攻撃の方向
    public float detectRange = 12f; // プレイヤー発見距離
    float idleTimer; // 待機時間のタイマー
    public Transform attackPoint; // 近接攻撃の中心位置
    public float attackRadius = 1f; // 近接攻撃の半径
    public float meleeCooldown = 1f;//近接攻撃クールダウン
    public LayerMask playerLayer; // プレイヤーのレイヤー
    public int maxHP = 100; //最大HP
    int currentHP;  //現在のHP
    bool isDead = false;//すでに死んでいるかどうかのフラグ

    //Vector2 : 「2Dの位置や方向
    Vector2 targetPosition; // 目標位置

    //Rigidbody2D : 「物理演算」重力として使っている
    Rigidbody2D rb;

    //Transform : 「位置・回転・大きさ」
    Transform player;

    Animator animator;


    [SerializeField] private string nextSceneName;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        currentHP = maxHP;

        GameObject playerObj =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObj == null)
        {
            Debug.LogError("Playerタグのオブジェクトが見つかりません");
            return;
        }

        player = playerObj.transform;

        if (attackEffect != null)
        {
            attackEffect.SetActive(false);
        }

        FacePlayer();
        ChangeState(State.Idle);
    }

    void Update()
    {
        //死んでいるなら、これ以降のAI処理を何もしない
        if (currentState == State.Die)
        {
            return;
        }
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

            case State.DashAttack:
                DashAttack();
                break;

            case State.MeleeAttack:
                DashAttack();
                break;

            default:
                Debug.Log("この処理は条件にありません");
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

        //１人で歩いているときは進む方向(direction)に合わせて向きを変える
        ChangeScaleDirection(direction);

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
        FacePlayer();
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
        else if ((newState == State.MeleeAttack))
        {
            MeleeAttack();
        }
        UpdateAnimation(newState);
    }
    void MeleeAttack()
    {
        Debug.Log("attackEffect = " + attackEffect);

        isAttacking = true;

        rb.linearVelocity = Vector2.zero;

       attackEffect.SetActive(true);

        isAttacking = true;

        rb.linearVelocity = Vector2.zero;

        // 攻撃エフェクト表示
        attackEffect.SetActive(true);

        Collider2D hitPlayer =
            Physics2D.OverlapCircle(
                attackPoint.position,
                attackRadius,
                playerLayer
            );

        Debug.Log("近接攻撃");

        if (hitPlayer != null)
        {
            Debug.Log("近接攻撃ヒット");
        }

        // 0.2秒後に消す
        Invoke(nameof(HideAttackEffect), 0.2f);

        Invoke(nameof(EndMeleeAttack), meleeCooldown);
    }

    void EndMeleeAttack()
    {
        //攻撃中のフラッグをOFFにする
        isAttacking = false;

        //待機状態へ戻す
        ChangeState(State.Idle);
    }

    void HideAttackEffect()
    {
        attackEffect.SetActive(false);
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

        FacePlayer();

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
        //プレイヤーが右にいるか左にいるかを調べて、向きを変える関数を呼び出す
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        ChangeScaleDirection(direction);
    }

    //Sceneビューで当たり判定を表示
    void OnDrawGizmos()
    {
        // attackPointがない場合は終了
        if (attackPoint == null)
        {
            return;
        }

        if (currentState == State.MeleeAttack)
        {
            // 赤色設定
            Gizmos.color = Color.red;

            // 攻撃範囲を円で表示
            Gizmos.DrawWireSphere(
                attackPoint.position,
                attackRadius
            );
        }
    }

    void ChangeScaleDirection(float direction)
    {
        if (direction > 0)
        {
            //左向き
            transform.localScale = new Vector3(1, 1, 1);
            //左向きの保存
            isFacingRight = true;
        }
        else if (direction < 0)
        {
            //右向き
            transform.localScale = new Vector3(-1, 1, 1);
            //右向きの保存
            isFacingRight = false;
        }
    }
    // プレイヤーの攻撃が当たったときに呼び出される関数
    // 引数として「攻撃が飛んできた位置（プレイヤーの位置）」を受け取る

    /*
    public void TakeDamage(Vector2 attackerPosition, int damage)
    {

        //HPが０以下になったら死亡処理を呼びだす
        if (currentHP <= 0)
        {
            ChangeState(State.Die);
            Die();
        }
    }
    */

    //プレイヤーの攻撃（トリガー）がボスに触れた瞬間に実行される関数
    void OnTriggerEnter2D(Collider2D collision)
    {
        //もしプレイヤーが「PlayerAttack」なら処理する
        if (collision.CompareTag("PlayerAttack"))
        {
            int damageValue = 10;

            //すでに作ってあったダメージ関数を呼び出す(対手の位置とダメージを渡す)
            TakeDamage(collision.transform.position, damageValue);
        }
    }


    public void TakeDamage(Vector2 attackerPosition, int damage)
    {
        // すでに死亡しているなら処理しない
        if (currentState == State.Die)
        {
            return;
        }

        // HPを減らす
        currentHP -= damage;

        Debug.Log($"ダメージ {damage} を受けた");
        Debug.Log($"残りHP : {currentHP}/{maxHP}");

        // HPが0以下なら死亡
        if (currentHP <= 0)
        {
            currentHP = 0;

            ChangeState(State.Die);
            Die();
        }
    }
    void Die()
    {
        Debug.Log("ボスを撃破した！");

        //動きを完全に止める
        rb.linearVelocity = Vector2.zero;
        //物理的な当たり判定(コライダー)を消して、プレイヤーが通り抜けられるようにする
        GetComponent<Collider2D>().enabled = false;
        //ボスを少し半透明にする、などの演出(とりあえず１秒後に消滅させる)
        //３秒後にゲーム画面からボスを完全に削除する
        Invoke(nameof(LoadNextScene), 3f);
        Destroy(gameObject, 3f);
      //  SceneManager.LoadScene("Stage2");

    }

    // 【新設】状態に合わせてAnimatorの番号を書き換える関数
    void UpdateAnimation(State state)
    {
        // アニメーターがついていない場合はエラー防止で何もしない
        if (animator == null) return;

        switch (state)
        {
            case State.Idle:
                animator.SetInteger("AnimState", 0); // 待機
                break;

            case State.Move:
                animator.SetInteger("AnimState", 1); // 歩き
                break;

            case State.Chase:
                animator.SetInteger("AnimState", 1); // 歩き
                break;

            case State.DashAttack:
                animator.SetInteger("AnimState", 2); // ダッシュ
                break;

            case State.MeleeAttack:
                animator.SetInteger("AnimState", 3); // 近接攻撃
                break;

            case State.Die:
                animator.SetInteger("AnimState", 4); // もし死亡モーションがあれば
                break;
        }
    }
    /*
      private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {


            Debug.Log("ボスを撃破した！");
            Destroy(gameObject);
            Invoke("GoToNextScene", 2.0f);
        }

    }
    */

    void LoadNextScene()
    {
        FadeManager.Instance.LoadSceneWithFade(SceneManager.GetActiveScene().buildIndex + 1);
    }

}