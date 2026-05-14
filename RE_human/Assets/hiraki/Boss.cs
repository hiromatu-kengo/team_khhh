using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    // 通常移動
    public float moveSpeed = 2f;

    public float minX = -10f;
    public float maxX = 10f;

    public float groundY = -3f;

    //public float walkBobSpeed = 6f;
    //public float walkBobHeight = 0.1f;

    // ゆっくり加速
    public float acceleration = 0.2f;

    private float currentSpeed = 0f;

    // =========================
    // 突進
    // =========================

    // 突進速度
    public float rushSpeed = 18f;

    // プレイヤー感知距離
    public float detectRange = 4f;

    // 突進距離
    public float rushDistance = 8f;

    // 突進前の溜め時間
    public float rushChargeTime = 0.3f;

    // =========================
    // 状態
    // =========================
    private bool isRushing = false;

    private bool isCharging = false;

    // =========================
    // その他
    // =========================
    private Vector2 targetPos;

    private Transform player;

    private Rigidbody2D rb;

    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        sr = GetComponent<SpriteRenderer>();

        player =
            GameObject.FindGameObjectWithTag("Player").transform;

        SetNewTarget();
    }

    void Update()
    {
        // プレイヤーとの距離
        float distance =
            Vector2.Distance(transform.position, player.position);

        // 感知したら突進準備
        if (
            distance <= detectRange
            && !isRushing
            && !isCharging
        )
        {
            StartCoroutine(StartRush());
        }

        // =========================
        // 左右反転
        // =========================
        if (targetPos.x > transform.position.x)
        {
            transform.localScale =
                new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale =
                new Vector3(-1, 1, 1);
        }
    }

    void FixedUpdate()
    {
        // =========================
        // 溜め中は停止
        // =========================
        if (isCharging)
        {
            return;
        }

        // =========================
        // 目標速度
        // =========================
        float targetSpeed =
            isRushing ? rushSpeed : moveSpeed;

        // =========================
        // ゆっくり加速
        // =========================
        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            acceleration * Time.fixedDeltaTime
        );

        // =========================
        // 基本移動
        // =========================
        Vector2 nextPos =
            Vector2.MoveTowards(
                rb.position,
                targetPos,
                currentSpeed * Time.fixedDeltaTime
            );

        // =========================
        // 通常時だけ上下に揺れる
        // =========================
        if (!isRushing)
        {
            //float walkOffset =
            //Mathf.Sin(Time.time * walkBobSpeed)
            //* walkBobHeight;

            //nextPos.y = groundY + walkOffset;
        }
        else
        {
            nextPos.y = groundY;
        }

        // =========================
        // 移動
        // =========================
        rb.MovePosition(nextPos);

        // =========================
        // 到着判定
        // =========================
        if (Vector2.Distance(rb.position, targetPos) < 0.1f)
        {
            // 突進終了
            if (isRushing)
            {
                isRushing = false;

                // 色を戻す
                sr.color = Color.white;
            }

            // 次の移動先
            SetNewTarget();
        }
    }

    // =========================
    // 通常移動先
    // =========================
    void SetNewTarget()
    {
        float randomX =
            Random.Range(minX, maxX);

        targetPos =
            new Vector2(randomX, groundY);
    }

    // =========================
    // 突進処理
    // =========================
    IEnumerator StartRush()
    {
        // 溜め開始
        isCharging = true;

        // 赤くして威嚇
        sr.color = Color.red;

        // 少し待つ
        yield return new WaitForSeconds(rushChargeTime);

        // 溜め終了
        isCharging = false;

        // 突進開始
        isRushing = true;

        // プレイヤー方向
        Vector2 direction =
            (
                player.position
                - transform.position
            ).normalized;

        // 突進先
        Vector2 rushTarget =
            (Vector2)transform.position
            + direction * rushDistance;

        // 移動範囲制限
        float clampedX =
            Mathf.Clamp(
                rushTarget.x,
                minX,
                maxX
            );

        targetPos =
            new Vector2(clampedX, groundY);
    }
}