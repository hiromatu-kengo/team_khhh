using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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
    public float warpCoolTime = 2.0f;
    private float warpCoolTimer = 0f;

    [Header("--- ワープアニメーション設定（秒数） ---")]
    [Tooltip("WarpOutアニメーションが始まってから、完全に消え去るまでの時間")]
    public float warpOutTime = 0.58f;
    [Tooltip("移動先でWarpInアニメーションが始まってから、完全に現れきるまでの時間")]
    public float warpInTime = 0.58f;

    private Rigidbody2D rb;
    private Animator animator;
    private float originalGravity;
    private bool isWarping;

    void Start()
    {
        RangeAttack = GetComponent<Boss3RangeAttack>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        originalGravity = rb.gravityScale;
        warpCoolTimer = warpCoolTime;
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 攻撃中・ワープ中も常にプレイヤーを睨みつける
        LookAtPlayer();
        warpCoolTimer += Time.deltaTime;

        // 遠距離攻撃中、またはワープ演出中は次の行動をさせない
        if ((RangeAttack != null && RangeAttack.isAttacking) || isWarping)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance < escapeRange && warpCoolTimer >= warpCoolTime)
        {
            WarpToSafePoint();
        }
        else if (distance <= rangeAttackRange)
        {
            RangeAttack.TryAttack();
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
            StartCoroutine(WarpRoutine(targetPoint));
        }
    }

    // スマートに修正されたワープ管理コルーチン
    IEnumerator WarpRoutine(Transform targetPoint)
    {
        isWarping = true;
        warpCoolTimer = 0f;

        // 1. 位置固定（重力としがらみをストップ）
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;

        // 2. 「消えるアニメーション（WarpOut）」を再生！
        if (animator != null)
        {
            animator.SetTrigger("WarpOut");
        }

        // 完全にスーッと消える（WarpOutの再生が終わる）のを待つ
        yield return new WaitForSeconds(warpOutTime);

        // 3. 完全に透明になった瞬間に、裏で座標を瞬間移動！
        // （※このタイミングでAnimator側も自動的に「WarpIn」に切り替わっています）
        transform.position = targetPoint.position;

        // 4. フワッと実体化し終わる（WarpInの再生が終わる）のを待つ
        yield return new WaitForSeconds(warpInTime);

        // 5. 重力を元に戻して物理落下を再開
        rb.gravityScale = originalGravity;

        yield return null;

        // 6. 着地を待つ
        while (rb.linearVelocity.y < -0.1f)
        {
            yield return null;
        }

        isWarping = false;
    }
}