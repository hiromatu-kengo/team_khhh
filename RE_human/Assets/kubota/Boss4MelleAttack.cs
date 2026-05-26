using UnityEngine;
using System.Collections;

public class Boss4MeleeAttack : MonoBehaviour
{
    [Header("--- 近接攻撃の設定 ---")]
    public Transform closeAttackPoint;  // 攻撃判定の中心
    public float closeAttackRadius = 1.5f; // 攻撃の届く半径
    public LayerMask playerLayer;       // プレイヤーのレイヤー
    public float cooldown = 2.0f;       // クールタイム（秒）

    private float timer = 0f;           // クールタイムを数えるタイマー

    public bool isAttacking = false;

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public bool CanAttack()
    {
        if (isAttacking == false && timer <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Execute()
    {
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        Debug.Log("【近接】振りかぶっている…（予兆）");
        yield return new WaitForSeconds(0.5f);

        Debug.Log("【近接】ドン！攻撃判定発生！");
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(closeAttackPoint.position, closeAttackRadius, playerLayer);
        foreach (Collider2D player in hitPlayers)
        {
            Debug.Log("プレイヤーに近接攻撃がヒット！");
        }

        yield return new WaitForSeconds(0.3f);

        isAttacking = false;
        timer = cooldown;
    }

    void OnDrawGizmosSelected()
    {
        // 判定の中心（closeAttackPoint）がインスペクターに設定されているときだけ処理する
        if (closeAttackPoint != null)
        {
            // 線の色を「赤」にする（お好みの色に変えてもOK！）
            Gizmos.color = Color.red;

            // 実際の攻撃判定（OverlapCircleAll）と全く同じ位置・同じ半径の「ワイヤーフレームの円」を描く
            Gizmos.DrawWireSphere(closeAttackPoint.position, closeAttackRadius);
        }
    }
}