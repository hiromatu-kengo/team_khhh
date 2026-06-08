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
    public float warpCoolTime = 2.0f;     // 【新規】ワープした後の「殴られ時間（秒）」
    private float warpCoolTimer = 0f;     // 【新規】時間を測るためのタイマー変数

    [Header("接地判定設定")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isWarping;

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

        CheckGrounded();
        LookAtPlayer();

        // --- 【新規】時間を進める（毎フレーム、経過時間を足していく） ---
        warpCoolTimer += Time.deltaTime;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (!isWarping)
        {
            // 【条件変更】近づかれた、かつ「クールタイムが終了している（タイマーが目標秒数を超えた）」ならワープ
            if (distance < escapeRange && warpCoolTimer >= warpCoolTime)
            {
                WarpToSafePoint();
            }
            else if (distance <= rangeAttackRange)
            {
                // クールタイム中（殴られ時間中）でも、射程内にいればボスは反撃（攻撃）を試みる
                // ※もし完全に無防備にしたければ、ここも条件を追加して制限できます
                RangeAttack.TryAttack();
            }
        }
    }

    void FixedUpdate()
    {
        if (isWarping && isGrounded && rb.linearVelocity.y <= 0)
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

    void CheckGrounded()
    {
        if (groundCheck == null) return;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void WarpToSafePoint()
    {
        if (warpPoints == null || warpPoints.Length == 0) return;
        if (!isGrounded) return;

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

            // --- 【新規】ワープに成功したら、タイマーを「0」にリセットする ---
            warpCoolTimer = 0f;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}