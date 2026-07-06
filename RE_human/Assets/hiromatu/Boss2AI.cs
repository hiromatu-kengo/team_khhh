using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Boss2AI : MonoBehaviour
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

    [Header("クリアしたときの移動先シーン名")]
    public string nextSceneName;

    private float deathTimer = 0.0f;

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

    public float dashCooldown = 10f; // ダッシュ攻撃のクールダウン時間
    public float meleePostWaitTime = 1.0f;//近接攻撃のあとのステイ時間
    public float dashPostWaitTime = 1.5f;//ダッシュ攻撃のあとのステイ時間

    //bool : 「はい/いいえ」を払わす変数

    bool canDash = true; // ダッシュ攻撃が可能かどうか
    bool isAttacking = false; // 攻撃中かどうか
    bool isFacingRight = true; // ボスが右を向いているかどうか
    bool isHitboxActive = false;
    float dashDirection; // ダッシュ攻撃の方向
    public float detectRange = 12f; // プレイヤー発見距離
    float idleTimer; // 待機時間のタイマー
    public Transform attackPoint; // 近接攻撃の中心位置
    public float attackRadius = 1f; // 近接攻撃の半径
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

    public AudioClip bossDamageSE;
    public AudioClip bossGuardSE;
    public AudioClip bossDashSE;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        //現在のHPを最大HPと同じにする
        currentHP = maxHP;

        // Playerタグを探す
        player = GameObject.FindGameObjectWithTag("Player").transform;

        FacePlayer();

        //ゲーム開始時は、アタックポイントのオブジェクトを完全に消しておく
        if (attackPoint != null)
        {
            attackPoint.gameObject.SetActive(false);
        }

        ChangeState(State.Idle);
    }

    void Update()
    {
        // もしボスが死んでいたら、ストップウォッチをスタートする
        /*  if (isDead)
          {
              // 毎フレーム、流れた時間（秒）をタイマーに足していく
              deathTimer += Time.deltaTime;

              // 2秒経ったら、シーンを切り替える！
              if (deathTimer >= 2.0f)
              {
                  SceneManager.LoadScene(nextSceneName);
              }
          }*/
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

        //待機状態でタイマーが残っている間は、プレイヤーを無視してしっかり止まる
        if (currentState == State.Idle && idleTimer > 0)
        {

        }
        else
        {
            //プレイヤーがボスの正面にいるかどうかを調べる
            bool isPlayerInFront = (isFacingRight && player.position.x > transform.position.x) ||
                                   (!isFacingRight && player.position.x < transform.position.x);
            //近距離なら近接攻撃
            if (playerDistance < meleeRange && isPlayerInFront)
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

        //HPが半分以下なら追跡スピードが1.4倍にアップ!
        float currentChaseSpeed = chaseSpeed;
        if(currentHP <= maxHP / 2)
        {
            currentChaseSpeed = chaseSpeed * 1.4f;
        }

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
        //攻撃中のフラグをONにする
        isAttacking = true;

        //攻撃中の移動を停止する
        rb.linearVelocity = Vector2.zero;

        Debug.Log("近接攻撃：振りかぶり開始（まだ当たらない）");



        //一秒後にEndMeleeAttackを実行する
        //時間差で実行
        Invoke(nameof(DoMeleeSwing), 0.4f);
    }

    void DoMeleeSwing()
    {


        if (attackPoint == null)
        {
            Debug.LogError("【設定ミス】ボスのインスペクターで Attack Point がセットされていません！");
            return;
        }

        //攻撃が当たるときに、オブジェクトを画面に出現させる
        attackPoint.gameObject.SetActive(true);

        //攻撃判定を出す
        Collider2D hitPlayer =
            Physics2D.OverlapCircle(
                attackPoint.position,
                attackRadius,
                playerLayer
                );
        isHitboxActive = true;

        Debug.Log("近接攻撃");

        //ボスの攻撃がプレイヤーにヒットしたらヒットストップをかける
        if(hitPlayer != null)
        {
            Debug.Log("近接攻撃ヒット");
            TriggerHitStop(0.12f);
        }

        // 0.2秒間だけ判定を出したあと、攻撃の終わり（後隙）の処理を呼び出す
        Invoke(nameof(EndMeleeAttack), 0.2f);
    }

    void EndMeleeAttack()
    {
        //攻撃が終わったので、判定フラグをOFFにする
        isHitboxActive = false;

        //攻撃が終わったら、アタックポイントのオブジェクトを消す
        if (attackPoint != null)
        {
            attackPoint.gameObject.SetActive(false);
        }
        //見た目だけ先に待機状態にもどしておく
        UpdateAnimation(State.Idle);
        Invoke(nameof(FinishMeleeStay), meleePostWaitTime);


    }
    void FinishMeleeStay()
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

        FacePlayer();

        //HPが半分以下なら、ため色がより危険「紫色」になる
        if (currentHP <= maxHP / 2)
        {
            spriteRenderer.color = new Color(0.7f, 0f, 0.7f);
        }
        else
        {
            //黄色にする
            spriteRenderer.color = Color.yellow;
        }

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

        //突進した瞬間にダッシュSEを鳴らす
        if(audioSource != null && bossDashSE != null)
        {
            audioSource.PlayOneShot(bossDashSE);
        }

        //0.5秒後に停止
        Invoke(nameof(StopDash), dashTime);
    }

    void StopDash()
    {
        //停止
        rb.linearVelocity = Vector2.zero;

        //白に戻す
        spriteRenderer.color = Color.white;

        //見た目を先に待機に戻しておく
        UpdateAnimation(State.Idle);

        //すぐ終了せず、ステイ時間を食んでから「完全にダッシュを終える関数」を呼ぶ
        Invoke(nameof(FinishDashStay), dashPostWaitTime);

        //クールタイム開始
        Invoke(nameof(ResetDash), dashCooldown);
    }

    void FinishDashStay()
    {
        //攻撃中のフラッグを終える
        isAttacking = false;

        //待機状態に戻る
        ChangeState(State.Idle);
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

        if (isHitboxActive)
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
    // プレイヤーの攻撃が当たったときに呼び出される関数
    // 引数として「攻撃が飛んできた位置（プレイヤーの位置）」を受け取る
    public void TakeDamage(Vector2 attackerPosition, int damage)
    {
        //すでに死んでいるならダメージ処理をしない
        if (currentState == State.Die) return;
        // 攻撃元が、ボスから見て右側にあるかどうかを調べる
        // プレイヤーのX座標がボスのX座標より大きければ、右側から攻撃されている
        bool isAttackedFromRight = attackerPosition.x > transform.position.x;

        // ボスが右を向いていて、右から攻撃された、もしくは、
        // ボスが左を向いていて、左から攻撃された場合（正面からの攻撃）
        if ((isFacingRight && isAttackedFromRight) || (!isFacingRight && !isAttackedFromRight))
        {
            //ガード成功
            Debug.Log("盾で防いだ！");

            //盾で弾いた時に、音を鳴らす
            if(audioSource != null && bossGuardSE != null)
            {
                audioSource.PlayOneShot(bossGuardSE);
            }

            //一瞬だけグレーにした弾いた感じを出す
            spriteRenderer.color = new Color(0.4f, 0.4f, 0.4f);
            Invoke(nameof(ResetColorAfterDamage), 0.1f);

            //正面ガードした時に、一瞬だけ止める
            TriggerHitStop(0.04f);

            return;// ダメージを与えずにここで処理を終了する
        }
        //実際にボスのHPを減らす引き算を追加
        currentHP -= damage;
        //ガード失敗
        Debug.Log($"背後からの攻撃ヒット!残りHP:{currentHP}/{maxHP}");

        //被弾したら赤くする
        spriteRenderer.color = Color.red;
        //0.15秒後に元の色に戻すタイマーを仕込む
        Invoke(nameof(ResetColorAfterDamage), 0.15f);

        //プレイヤーの攻撃を背後にくらった瞬間にヒットストップ
        TriggerHitStop(0.08f);

        //ボスの被ダメSEを鳴らす
        if (audioSource != null && bossDamageSE != null)
        {
            audioSource.PlayOneShot(bossDamageSE);
        }

        //HPが０以下になったら死亡処理を呼びだす
        if (currentHP <= 0)
        {
            ChangeState(State.Die);
            Die();
        }
    }
    //くらった後の赤色を白に戻す関数
    void ResetColorAfterDamage()
    {
        //すでに死んでいるなら、白に戻さずそのままにする
        if (currentState == State.Die) return;

        spriteRenderer.color = Color.white;
    }

    void Die()
    {
        Debug.Log("ボスを撃破した！");

        //これまでに仕込まれたすべてのタイマーを完全にキャンセルする
        CancelInvoke();

        //動きを完全に止める
        rb.linearVelocity = Vector2.zero;

        //物理演算のシミュレーションをオフにする
        rb.simulated = false;

        //物理的な当たり判定(コライダー)を消して、プレイヤーが通り抜けられるようにする
        //  GetComponent<Collider2D>().enabled = false;
        //ボスを少し半透明にする、などの演出(とりあえず１秒後に消滅させる)
        //３秒後にゲーム画面からボスを完全に削除する
        Invoke(nameof(LoadNextScene), 3f);
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
            case State.Chase:
                animator.SetInteger("AnimState", 1); // 通常移動も追跡も、どちらも「歩き」にする
                break;

            case State.MeleeAttack:
                animator.SetInteger("AnimState", 3);//近接攻撃
                break;

            case State.DashAttack:
                animator.SetInteger("AnimState", 2); // ダッシュ
                break;

            case State.Die:
                animator.SetInteger("AnimState", 4); // （おまけ）もし死亡モーションがあれば
                break;
        }
    }
    void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //ヒットストップを呼び出すための管理関数
    public void TriggerHitStop(float duration)
    {
        //すでに死んでいる場合はゲームをやめない
        if (currentState == State.Die) return;

        StartCoroutine(HitStopCoroutine(duration));
    }

    //実際に時間を一瞬だけスローにするコルーチン
    private IEnumerator HitStopCoroutine(float duration)
    {
        //Unity全体の時間の流れを「ほぼ停止(0.02倍速)」にする
        //0に完全停止させると不都合が起きることがあるため0.02位にする
        Time.timeScale = 0.02f;

        //Time.timeScaleの影響を受けない「現実世界の時間(Realtime)」で指定数秒松
        yield return new WaitForSeconds(duration);

        //時間の流れを等倍に戻す
        Time.timeScale = 1f;
    }

}