using UnityEngine;
using System.Collections;

public class Boss4MeleeAttack : MonoBehaviour
{
    public Transform closeAttackPoint;
    public float closeAttackRadius = 1.5f;
    public LayerMask playerLayer;
    public int closeDamage = 10;
    public float cooldown = 2.0f;

    private float timer = 0f;
    public bool IsAttacking { get; private set; } = false;

    void Update()
    {
        if (timer > 0) timer -= Time.deltaTime;
    }

    public bool CanAttack() => !IsAttacking && timer <= 0;

    public void Execute()
    {
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        IsAttacking = true;
        Debug.Log("【近接】構え");
        yield return new WaitForSeconds(0.5f);

        Debug.Log("【近接】攻撃判定！");
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(closeAttackPoint.position, closeAttackRadius, playerLayer);
        foreach (Collider2D player in hitPlayers)
        {
            Debug.Log("近接ヒット！");
        }

        yield return new WaitForSeconds(0.3f);
        IsAttacking = false;
        timer = cooldown;
    }
}