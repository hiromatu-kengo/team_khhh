using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss1AI : MonoBehaviour
{
    enum State
    {
        Idle,           // 待機
        Move,           // 移動
        Chase,          // 追跡
        DashAttack,     // ダッシュ攻撃
        Hit,            // 被ダメージ・硬直
        Die             // 死亡状態
    }

    State currentState;
    SpriteRenderer spriteRenderer;
    Animator animator;
    Rigidbody2D rb;
    Transform player;

    [Header("移動・追尾設定")]
    public float moveSpeed = 3f;
    public float chaseSpeed = 5f;
    public float moveRange = 5f;
    public float idleTime = 1.5f;
    public float detectRange = 12f;
    public float stopDistance = 1.5f;   // プレイヤーと密着時に歩くのを止める距離

    [Header("ダッシュ攻撃設定")]
    public float dashRange = 8f;        // ダッシュ攻撃を開始する距離
    public float dashSpeed = 15f;       // ダッシュ速度
    public float dashTime = 0.4f;       // 突進している時間
    public float dashCooldown = 3f;     // 次のダッシュまでの待ち時間
    public float postDashPause = 1.2f;  // ダッシュ攻撃後の「隙（止まる時間）」

    [Header("被ダメージ・硬直設定")]
    public float hitStunDuration = 0.5f; // 攻撃を受けた時の硬直時間（秒）
    public float knockbackForce = 4f;    // のけぞる力（ノックバック）
    public string playerAttackTag = "PlayerAttack"; // プレイヤー攻撃のタグ名

    [Header("オブジェクト参照")]
    public Transform attackPoint;       // 攻撃判定
    public GameObject attackEffect;     // 攻撃エフェクト（任意）

    [Header("ステータス")]
    public int maxHP = 100;
    int currentHP;

    bool canDash = true;
    bool isAttacking = false;
    bool isRecovering = false; // ダッシュ後の隙（後硬直）フラグ
    float dashDirection;
    float idleTimer;
    Vector2 targetPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        currentHP = maxHP;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        HideAttackEffect();
        FacePlayer();
        ChangeState(State.Idle);
    }

    void Update()
    {
        UpdateAnimationSpeed();

        // 隙（硬直）時間中は押し出されて滑らないように速度をゼロに固定
        if (isRecovering)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // 死亡・食らい中・攻撃中・プレイヤー不在時は思考ストップ
        if (currentState == State.Die || currentState == State.Hit || isAttacking || player == null)
            return;

        float playerDistance = Vector2.Distance(transform.position, player.position);

        // 1. ダッシュ攻撃可能であれば、距離内(dashRange)なら最優先で発動
        if (playerDistance <= dashRange && canDash)
        {
            ChangeState(State.DashAttack);
        }
        // 2. 近すぎる（stopDistance以下）場合は押し合わずに向きだけ合わせてIdle待機（クールタイム消化を待つ）
        else if (playerDistance <= stopDistance)
        {
            FacePlayer();
            ChangeState(State.Idle);
        }
        // 3. 索敵範囲内であれば追尾（Chase）
        else if (playerDistance < detectRange)
        {
            ChangeState(State.Chase);
        }
        // 4. 範囲外であれば通常の巡回/待機
        else if (currentState != State.Move)
        {
            ChangeState(State.Idle);
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
        }
    }

    void ChangeState(State newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        TriggerAnimationState(newState);

        switch (newState)
        {
            case State.Idle:
                idleTimer = idleTime;
                rb.linearVelocity = Vector2.zero;
                break;

            case State.DashAttack:
                ExecuteDashAttack();
                break;

            case State.Hit:
                Invoke(nameof(EndHit), hitStunDuration);
                break;
        }
    }

    void UpdateAnimationSpeed()
    {
        if (animator == null) return;

        if ((currentState == State.Move || currentState == State.Chase) && !isRecovering)
        {
            float speed = Mathf.Abs(rb.linearVelocity.x);
            if (speed < 0.1f) speed = (currentState == State.Chase) ? chaseSpeed : moveSpeed;
            animator.SetFloat("Speed", speed);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    void Idle()
    {
        // プレイヤーが近くにいる時はうろうろ移動（Move）に移らず、その場で待機する
        if (player != null && Vector2.Distance(transform.position, player.position) <= detectRange)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = Vector2.zero;
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0)
        {
            float randomX = Random.Range(-moveRange, moveRange);
            targetPosition = new Vector2(transform.position.x + randomX, transform.position.y);
            ChangeState(State.Move);
        }
    }

    void Move()
    {
        float direction = Mathf.Sign(targetPosition.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        ChangeScaleDirection(direction);

        if (Mathf.Abs(targetPosition.x - transform.position.x) < 0.2f)
        {
            ChangeState(State.Idle);
        }
    }

    void Chase()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);
        FacePlayer();
    }

    void ExecuteDashAttack()
    {
        isAttacking = true;
        canDash = false;
        rb.linearVelocity = Vector2.zero;

        dashDirection = Mathf.Sign(player.position.x - transform.position.x);
        FacePlayer();

        spriteRenderer.color = Color.red;
        Invoke(nameof(StartDash), 0.5f);
    }

    void StartDash()
    {
        if (attackPoint != null) attackPoint.gameObject.SetActive(true);
        if (attackEffect != null) attackEffect.SetActive(true);

        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);
        Invoke(nameof(StopDash), dashTime);
    }

    void StopDash()
    {
        rb.linearVelocity = Vector2.zero;
        spriteRenderer.color = Color.white;
        HideAttackEffect();

        isAttacking = false;
        isRecovering = true; // 隙（後硬直）をスタート
        ChangeState(State.Idle);

        Invoke(nameof(EndRecovery), postDashPause); // 指定秒数後に移動・思考を解禁
        Invoke(nameof(ResetDash), dashCooldown);
    }

    void EndRecovery()
    {
        isRecovering = false; // 隙が解除されて行動再開
    }

    void ResetDash()
    {
        canDash = true; // ★クールタイム終了（いつでも次のダッシュが可能になる）
    }

    void HideAttackEffect()
    {
        if (attackPoint != null) attackPoint.gameObject.SetActive(false);
        if (attackEffect != null) attackEffect.SetActive(false);
    }

    // --- プレイヤーの攻撃（Trigger判定）が当たった時の処理 ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerAttackTag))
        {
            TakeDamage(collision.transform.position, 10);
        }
    }

    // --- ダメージ受ける処理 ---
    public void TakeDamage(int damage)
    {
        Vector2 defaultPos = player != null ? player.position : transform.position;
        TakeDamage(defaultPos, damage);
    }

    public void TakeDamage(Vector2 attackerPosition, int damage)
    {
        if (currentState == State.Die) return;

        currentHP -= damage;
        Debug.Log($"[Boss1AI] ダメージを受けました！残りHP: {currentHP}");

        // アクションを中断
        CancelInvoke(nameof(StartDash));
        CancelInvoke(nameof(StopDash));
        CancelInvoke(nameof(EndRecovery));
        HideAttackEffect();
        isAttacking = false;
        isRecovering = false;

        if (currentHP <= 0)
        {
            currentHP = 0;
            ChangeState(State.Die);
            Die();
            return;
        }

        spriteRenderer.color = Color.red;
        Invoke(nameof(ResetColor), 0.15f);

        float knockbackDir = Mathf.Sign(transform.position.x - attackerPosition.x);
        rb.linearVelocity = new Vector2(knockbackDir * knockbackForce, rb.linearVelocity.y);

        ChangeState(State.Hit);
    }

    void ResetColor()
    {
        if (currentState == State.Die) return;
        spriteRenderer.color = Color.white;
    }

    void EndHit()
    {
        if (currentState == State.Die) return;
        rb.linearVelocity = Vector2.zero;
        ChangeState(State.Idle);
    }

    void Die()
    {
        rb.linearVelocity = Vector2.zero;

        // 死亡時に食らい判定と物理挙動をカット
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        rb.simulated = false;

        Invoke(nameof(LoadNextScene), 3f);
        Destroy(gameObject, 3f);
    }

    void FacePlayer()
    {
        if (player == null) return;
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        ChangeScaleDirection(direction);
    }

    void ChangeScaleDirection(float direction)
    {
        if (direction > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void TriggerAnimationState(State state)
    {
        if (animator == null) return;

        switch (state)
        {
            case State.DashAttack:
                animator.SetTrigger("Dash");
                break;
            case State.Die:
                animator.SetBool("die", true);
                break;
        }
    }

    void LoadNextScene()
    {
        FadeManager.Instance.LoadSceneWithFade(SceneManager.GetActiveScene().buildIndex + 1);
    }
}