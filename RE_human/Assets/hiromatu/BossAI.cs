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

    bool canDash = true;

    bool isAttacking = false;

    // プレイヤー発見距離
    public float detectRange = 6f;

    float idleTimer;

    Vector2 targetPosition;

    Rigidbody2D rb;

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
        rb.linearVelocity = Vector2.zero;

        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0)
        {
            float randomX =
                Random.Range(-moveRange, moveRange);

            targetPosition = new Vector2(
                transform.position.x + randomX,
                transform.position.y
            );

            ChangeState(State.Move);
        }
    }

    void Move()
    {
        float direction =
            Mathf.Sign(targetPosition.x - transform.position.x);

        rb.linearVelocity =
            new Vector2(direction * moveSpeed, 0);

        float distance =
            Mathf.Abs(targetPosition.x - transform.position.x);

        if (distance < 0.1f)
        {
            ChangeState(State.Idle);
        }
    }

    void Chase()
    {
        float direction =
            Mathf.Sign(player.position.x - transform.position.x);

        rb.linearVelocity =
            new Vector2(direction * chaseSpeed, 0);
    }

    void ChangeState(State newState)
    {
        currentState = newState;

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