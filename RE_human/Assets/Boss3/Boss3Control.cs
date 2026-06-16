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

        // ★【ここを修正！】
        // 攻撃中（溜め・後隙）であっても、常にプレイヤーの方を向かせるために一番上に持ってきました。
        // これでプレイヤーが後ろに回り込んでも、ボスがクルッと振り向いて弾を撃ちます！
        LookAtPlayer();

        // 時間を進める
        warpCoolTimer += Time.deltaTime;

        // ★遠距離攻撃スクリプトが「今まさに攻撃中」なら、ワープ処理だけをスキップする
        if (RangeAttack != null && RangeAttack.isAttacking)
        {
            return; // ここから下の処理（ワープや新しい攻撃の命令）をスキップ
        }

        // プレイヤーとの距離を測る
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (!isWarping)
        {
            // 近づかれた、かつクールタイムが終わっていたらワープ！
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

        List<Transform> safePoints = new List<Transform>();

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
            rb.linearVelocity = Vector2.zero;
            isWarping = true;
            warpCoolTimer = 0f;
        }
    }
}