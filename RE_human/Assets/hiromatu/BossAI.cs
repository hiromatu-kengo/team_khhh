using UnityEngine;

public class BossAI : MonoBehaviour
{
    enum State
    {
        Idle,
        Move
    }

    State currentState;

    public float moveSpeed = 3f;
    public float moveRange = 5f;
    public float idleTime = 2f;

    float idleTimer;

    Vector2 targetPosition;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        ChangeState(State.Idle);
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;

            case State.Move:
                Move();
                break;
        }
    }

    void Idle()
    {
        rb.linearVelocity = Vector2.zero;

        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0)
        {
            // Xだけランダム
            float randomX =
                Random.Range(-moveRange, moveRange);

            // Yは固定
            targetPosition = new Vector2(
                transform.position.x + randomX,
                transform.position.y
            );

            ChangeState(State.Move);
        }
    }

    void Move()
    {
        // 方向
        float direction =
            Mathf.Sign(targetPosition.x - transform.position.x);

        // 横移動のみ
        rb.linearVelocity =
            new Vector2(direction * moveSpeed, 0);

        // 距離判定
        float distance =
            Mathf.Abs(targetPosition.x - transform.position.x);

        if (distance < 0.1f)
        {
            ChangeState(State.Idle);
        }
    }

    void ChangeState(State newState)
    {
        currentState = newState;

        if (newState == State.Idle)
        {
            idleTimer = idleTime;
        }
    }
}