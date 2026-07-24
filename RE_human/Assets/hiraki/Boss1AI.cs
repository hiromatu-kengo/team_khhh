using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss1AI : MonoBehaviour
{
    enum State
    {
        Idle,           // 待機
        Move,           // 移動
        Chase,          // 追跡
        MeleeAttack,    // 近接攻撃
        DashAttack,     // ダッシュ攻撃
        Hit,            // 被ダメージ
        Die             // 死亡状態
    }

    State currentState; // 現在の状態
    SpriteRenderer spriteRenderer; // スプライトレンダラー
    Animator animator; // アニメーター
    Rigidbody2D rb; // 物理演算
    Transform player; // プレイヤーのTransform

    public float moveSpeed = 3f; // 移動速度
    public float chaseSpeed = 5f; // 追跡速度

    public float moveRange = 5f; // 待機中の移動範囲
    public float idleTime = 2f; // 待機時間

    public float meleeRange = 2.5f; // 近接攻撃の範囲
    public float dashRange = 5f; // ダッシュ攻撃の範囲
    public float dashSpeed = 15f; // ダッシュ攻撃の速度
    public float dashTime = 0.5f; // ダッシュ攻撃の持続時間

    public float dashCooldown = 3f; // ダッシュ攻撃のクールダウン時間

    public GameObject attackEffect;

    bool canDash = true; // ダッシュ攻撃が可能かどうか
    bool isAttacking = false; // 攻撃中かどうか
    bool isFacingRight = true; // ボスが右を向いているかどうか
    float dashDirection; // ダッシュ攻撃の方向
    public float detectRange = 12f; // プレイヤー発見距離
    float idleTimer; // 待機時間のタイマー
    public Transform attackPoint; // 近接攻撃の中心位置（※コライダー不要！空のオブジェクトでOK）
    public float attackRadius = 1.5f; // 近接攻撃の判定の大きさ
    public float meleeCooldown = 1f; // 近接攻撃クールダウン
    public LayerMask playerLayer; // プレイヤーのレイヤー
    public int maxHP = 100; // 最大HP
    int currentHP; // 現在のHP
    bool hasHit = false; // 今回の攻撃で既にヒットしたか

    Vector2 targetPosition; // 目標位置

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        currentHP = maxHP;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

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
        // 死んでいる、ヒット中はAI処理を停止
        if (currentState == State.Die || currentState == State.Hit)
            return;

        // 【修正】攻撃中は移動や思考を止めつつ、ヒット判定チェックだけを行う
        if (isAttacking)
        {
            if (currentState == State.MeleeAttack && !hasHit)
            {
                CheckMeleeHit();
            }
            return;
        }

        // プレイヤーとの距離
        float playerDistance = Vector2.Distance(transform.position, player.position);

        // 近距離なら近接攻撃
        if (playerDistance < meleeRange)
        {
            ChangeState(State.MeleeAttack);
        }
        // 中距離なら突進攻撃
        else if (playerDistance < dashRange && canDash)
        {
            ChangeState(State.DashAttack);
        }
        // プレイヤーを見つけたら追尾
        else if (playerDistance < detectRange)
        {
            ChangeState(State.Chase);
        }

        // 現在の状態に応じた移動・攻撃処理を実行
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
                MeleeAttack();
                break;
        }

        if (currentState == State.Move || currentState == State.Chase)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        }
        else if (currentState == State.Idle)
        {
            animator.SetFloat("Speed", 0);
        }
    }

    void Idle()
    {
        rb.linearVelocity = Vector2.zero;
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0)
        {
            float randomX = Random.Range(-moveRange, moveRange);
            targetPosition = new Vector2(
                transform.position.x + randomX,
                transform.position.y
            );

            ChangeState(State.Move);
        }
    }

    void Move()
    {
        float direction = Mathf.Sign(targetPosition.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * moveSpeed, 0);

        ChangeScaleDirection(direction);

        float distance = Mathf.Abs(targetPosition.x - transform.position.x);

        if (distance < 0.1f)
        {
            ChangeState(State.Idle);
        }
    }

    void Chase()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * chaseSpeed, 0);
        FacePlayer();
    }

    void ChangeState(State newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        if (newState == State.Idle)
        {
            idleTimer = idleTime;
        }

        UpdateAnimation(newState);
    }

    void MeleeAttack()
    {
        isAttacking = true;
        hasHit = false; // ヒット状態リセット
        rb.linearVelocity = Vector2.zero;

        if (attackEffect != null)
        {
            attackEffect.SetActive(true);
        }

        ChangeState(State.MeleeAttack);

        Invoke(nameof(HideAttackEffect), 0.2f);
        Invoke(nameof(EndMeleeAttack), meleeCooldown);
    }

    // 【追加】攻撃時間中に毎フレーム呼ばれる判定処理
    void CheckMeleeHit()
    {
        if (attackPoint == null) return;

        // AttackPointの位置を中心に、attackRadiusの大きさの円でプレイヤーを探す
        Collider2D hitPlayer = Physics2D.OverlapCircle(
            attackPoint.position,
            attackRadius,
            playerLayer
        );

        if (hitPlayer != null)
        {
            hasHit = true; // 1回の攻撃で1回だけヒットさせる
            Debug.Log("★近接攻撃がヒットしました！: " + hitPlayer.name);

            // プレイヤーにダメージを与えるスクリプトがある場合はここに記述します
            // hitPlayer.GetComponent<PlayerHealth>()?.TakeDamage(10);
        }
    }

    void EndMeleeAttack()
    {
        isAttacking = false;
        ChangeState(State.Idle);
    }

    void HideAttackEffect()
    {
        if (attackEffect != null)
        {
            attackEffect.SetActive(false);
        }
    }

    void DashAttack()
    {
        isAttacking = true;
        canDash = false;
        rb.linearVelocity = Vector2.zero;

        dashDirection = Mathf.Sign(player.position.x - transform.position.x);
        FacePlayer();

        spriteRenderer.color = Color.red;

        Invoke(nameof(StartDash), 1f);
        ChangeState(State.DashAttack);
    }

    void StartDash()
    {
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0);
        Invoke(nameof(StopDash), dashTime);
    }

    void StopDash()
    {
        rb.linearVelocity = Vector2.zero;
        spriteRenderer.color = Color.white;
        isAttacking = false;

        ChangeState(State.Idle);
        Invoke(nameof(ResetDash), dashCooldown);
    }

    void ResetDash()
    {
        canDash = true;
    }

    void FacePlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        ChangeScaleDirection(direction);
    }

    void OnDrawGizmos()
    {
        if (attackPoint == null) return;

        // 攻撃時以外でも常に範囲を赤丸で確認できるようにしています
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    void ChangeScaleDirection(float direction)
    {
        if (direction > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            isFacingRight = true;
        }
        else if (direction < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            isFacingRight = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            int damageValue = 10;
            TakeDamage(collision.transform.position, damageValue);
        }
    }

    public void TakeDamage(Vector2 attackerPosition, int damage)
    {
        if (currentState == State.Die)
            return;

        currentHP -= damage;

        spriteRenderer.color = Color.red;
        Invoke(nameof(ResetColorAfterDamage), 0.15f);

        if (currentHP <= 0)
        {
            currentHP = 0;
            ChangeState(State.Die);
            Die();
            return;
        }

        ChangeState(State.Hit);
    }

    void ResetColorAfterDamage()
    {
        if (currentState == State.Die) return;
        spriteRenderer.color = Color.white;
    }

    void Die()
    {
        rb.linearVelocity = Vector2.zero;
        Invoke(nameof(LoadNextScene), 3f);
        Destroy(gameObject, 3f);
    }

    void UpdateAnimation(State state)
    {
        if (animator == null) return;

        switch (state)
        {
            case State.Idle:
                animator.SetFloat("Speed", 0);
                break;

            case State.Move:
            case State.Chase:
                animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
                break;

            case State.MeleeAttack:
                animator.SetTrigger("Attack");
                break;

            case State.DashAttack:
                animator.SetTrigger("Dash");
                break;

            case State.Hit:
                animator.SetTrigger("Hit");
                break;

            case State.Die:
                animator.SetBool("Dead", true);
                break;
        }
    }

    void LoadNextScene()
    {
        FadeManager.Instance.LoadSceneWithFade(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void Hit()
    {
        rb.linearVelocity = Vector2.zero;
    }

    void EndHit()
    {
        ChangeState(State.Idle);
    }
}