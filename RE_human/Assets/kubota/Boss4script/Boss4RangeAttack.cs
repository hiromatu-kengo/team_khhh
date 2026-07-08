using UnityEngine;
using System.Collections;


public class Boss4RangeAttack : MonoBehaviour
{
    [Header("--- 遠距離攻撃の設定 ---")]
    public GameObject bulletPrefab;     // 弾のプレハブ
    public Transform firePoint;         // 弾が出る位置
    public float cooldown = 3.0f;       // クールタイム（秒）

    private float timer = 0f;           // クールタイムを数えるタイマー

    [Header("--- タイミング調整（インスペクターで秒数を設定） ---")]
    [Tooltip("アニメーションが始まってから、実際に弾が出るまでの時間（溜め）")]
    public float chargeTime = 1.15f;
    [Tooltip("弾が出たあと、次の行動に移れるようになるまでの時間（後隙）")]
    public float recoveryTime = 1.17f;

    [Header("アニメーション（任意）")]
    private Animator animator;

    public bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 毎フレームタイマーを減らす
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }


    public bool CanAttack()
    {
        if (isAttacking == false && timer <= 0)
        {
            return true; // 攻撃できるよ！
        }
        else
        {
            return false; // 今は無理！
        }
    }


    public void Execute(Transform player)
    {
        StartCoroutine(AttackRoutine(player));
    }

    // コルーチン(IEnumerator AttackRoutine)コルーチンとは中断と再開可能な関数の一種。
    // フレームをまたいで処理を継続することができる
    IEnumerator AttackRoutine(Transform player)
    {
        isAttacking = true;

        // 攻撃の瞬間は足を止める
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        //【まず最初に】アニメーションをLongAttack（溜めポーズ）に切り替える！
        if (animator != null)
        {
            animator.SetTrigger("Boss4LongAttack");
        }

        Debug.Log("① 【予兆】溜め開始. アニメーションが動き出します。 Time: " + Time.time);

        // 溜めポーズのアニメーションが再生されながら、ここで1.83秒待つ
        yield return new WaitForSeconds(chargeTime);

        Debug.Log("② 【溜め完了】弾を発射します。 Time: " + Time.time);

        if (bulletPrefab != null && firePoint != null)
        {
            // 弾を生成して飛ばす
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Boss4Bullet bulletScript = bullet.GetComponent<Boss4Bullet>();
            if (bulletScript != null)
            {
                // プレイヤーへの方向を計算
                Vector2 direction = (player.position - firePoint.position).normalized;
                bulletScript.Setup(direction);
            }
        }

        // 弾を撃ったあとの後隙（1.17秒）アニメーションが流れる
        yield return new WaitForSeconds(recoveryTime);

        Debug.Log("③ 【後隙完了】攻撃終了。Idleに戻ります。 Time: " + Time.time);

        isAttacking = false;
        timer = cooldown; // クールタイム開始
    }
}