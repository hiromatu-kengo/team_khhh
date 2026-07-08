using UnityEngine;
using System.Collections;

public class Boss4MeleeAttack : MonoBehaviour
{
    [Header("--- 近接攻撃の設定 ---")]
    public Transform closeAttackPoint;  // 攻撃判定の中心
    public float closeAttackRadius = 1.5f; // 攻撃の届く半径
    public LayerMask playerLayer;       // プレイヤーのレイヤー
    public float cooldown = 2.0f;       // クールタイム（秒）

    [Header("--- 見た目の設定 ---")]
    public GameObject meleeVisual;

    private Animator animator;

    private float timer = 0f;           // クールタイムを数えるタイマー
    public bool isAttacking = false;

    void Start()
    {
        // ★追加：ゲーム開始時は剣の画像を非表示（OFF）にしておく
        if (meleeVisual != null)
        {
            meleeVisual.SetActive(false);
        }

        animator = GetComponent<Animator>();
    }


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

        if (animator != null)
        {
            animator.SetTrigger("Boss4MelleAttack");
        }

        Debug.Log("【近接】振りかぶっている…（予兆）");
        yield return new WaitForSeconds(0.5f);

        Debug.Log("【近接】ドン！攻撃判定発生！");
        // 攻撃の瞬間に画像を表示（ON）する！
        if (meleeVisual != null) meleeVisual.SetActive(true);

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(closeAttackPoint.position, closeAttackRadius, playerLayer);
        foreach (Collider2D player in hitPlayers)
        {
            Debug.Log("プレイヤーに近接攻撃がヒット！");
        }

        yield return new WaitForSeconds(0.3f);

        // 攻撃が終わったら画像を隠す（OFF）！
        if (meleeVisual != null) meleeVisual.SetActive(false);

        isAttacking = false;
        timer = cooldown;
    }
}