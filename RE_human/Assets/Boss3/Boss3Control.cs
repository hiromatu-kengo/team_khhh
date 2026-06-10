using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Boss3Control : MonoBehaviour
{
    public Transform playerTransform;
    private Boss3RangeAttack RangeAttack;

    [Header("射程・間合い設定")]
    public float rangeAttackRange = 10.0f;
    public float escapeRange = 5.0f;

    [Header("ワープ設定")]
    public Transform[] warpPoints;
    public float warpCoolTime = 2.0f;     // ワープした後の「殴られ時間（秒）」
    private float warpCoolTimer = 0f;     // 時間を測るためのタイマー変数

    private Rigidbody2D rb;
    private bool isWarping; // ワープ中かどうかのハタ

    void Start()
    {
        RangeAttack = GetComponent<Boss3RangeAttack>();
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // 最初はすぐにワープできるように、タイマーを満タンにしておく
        warpCoolTimer = warpCoolTime;
    }

    void Update()
    {
        if (playerTransform == null) return;

        LookAtPlayer();

        // 時間を進める
        warpCoolTimer += Time.deltaTime;

        // プレイヤーとの距離を測る
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (!isWarping)
        {
            // 近づかれた、かつクールタイムが終わっていたらワープ！（接地判定は無視！）
            if (distance < escapeRange && warpCoolTimer >= warpCoolTime)
            {
                WarpToSafePoint();
            }
            else if (distance <= rangeAttackRange)
            {
                // 攻撃範囲内にいれば反撃を試みる
                RangeAttack.TryAttack();
            }
        }
    }

    void FixedUpdate()
    {
        // ★接地判定を使わずに、ワープ後にボスの落ちる速度（y軸の速度）が落ち着いたらワープ状態を解除する
        if (isWarping && rb.linearVelocity.y <= 0.1f)
        {
            isWarping = false;
        }
    }

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

    void WarpToSafePoint()
    {
        if (warpPoints == null || warpPoints.Length == 0) return;
        // ★「地面にいないとワープできない」という制限を消したよ！

        List<Transform> safePoints = new List<Transform>();

        // プレイヤーから離れている安全なワープ先を探す
        foreach (Transform point in warpPoints)
        {
            if (point == null) continue;
            float distToPlayer = Vector3.Distance(point.position, playerTransform.position);

            if (distToPlayer >= escapeRange)
            {
                safePoints.Add(point);
            }
        }

        Transform targetPoint = null;

        if (safePoints.Count > 0)
        {
            int randomIndex = Random.Range(0, safePoints.Count);
            targetPoint = safePoints[randomIndex];
        }
        else
        {
            // 安全な場所がなければ、一番プレイヤーから遠い場所を選ぶ
            float maxDist = -1f;
            foreach (Transform point in warpPoints)
            {
                if (point == null) continue;
                float dist = Vector3.Distance(point.position, playerTransform.position);
                if (dist > maxDist)
                {
                    maxDist = dist;
                    targetPoint = point;
                }
            }
        }

        if (targetPoint != null)
        {
            transform.position = targetPoint.position;
            rb.linearVelocity = Vector2.zero; // ワープ直後の勢いをリセット
            isWarping = true;

            // ワープタイマーリセット
            warpCoolTimer = 0f;
        }
    }
}