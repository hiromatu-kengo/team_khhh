using UnityEngine;
using System.Collections;

public class Boss4GrabAttack : MonoBehaviour
{
    public Transform closeAttackPoint;
    public float closeAttackRadius = 1.5f;
    public LayerMask playerLayer;

    public float cooldown = 8.0f;
    public float grabDashSpeed = 8.0f;
    public float grabDuration = 1.5f;

    private float timer = 0f;
    private Rigidbody2D rb;
    public bool IsAttacking { get; private set; } = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (timer > 0) timer -= Time.deltaTime;
    }

    public bool CanAttack() => !IsAttacking && timer <= 0;

    public void Execute(Transform player)
    {
        StartCoroutine(AttackRoutine(player));
    }

    IEnumerator AttackRoutine(Transform player)
    {
        IsAttacking = true;
        Debug.Log("【つかみ】予兆");
        yield return new WaitForSeconds(0.8f);

        float dashDirection = player.position.x > transform.position.x ? 1 : -1;
        float dashTimer = 0.5f;
        bool hasCaught = false;

        while (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
            rb.linearVelocity = new Vector2(dashDirection * grabDashSpeed, rb.linearVelocity.y);

            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(closeAttackPoint.position, closeAttackRadius, playerLayer);
            if (hitPlayers.Length > 0)
            {
                hasCaught = true;
                break;
            }
            yield return null;
        }

        if (!hasCaught)
        {
            Debug.Log("【つかみ】ミス！");
            rb.linearVelocity = Vector2.zero;
            yield return new WaitForSeconds(0.6f);
        }
        else
        {
            Debug.Log("【つかみ】キャッチ！");
            rb.linearVelocity = Vector2.zero;

            float grabTimer = 0f;
            while (grabTimer < grabDuration)
            {
                player.position = closeAttackPoint.position; // 拘束
                Debug.Log("ガシガシダメージ！");
                yield return new WaitForSeconds(0.5f);
                grabTimer += 0.5f;
            }
            Debug.Log("【つかみ】終了、吹き飛ばし");
        }

        IsAttacking = false;
        timer = cooldown;
    }
}
