using UnityEngine;
using System.Collections;


public class Boss4RangeAttack : MonoBehaviour
{
    [Header("--- 遠距離攻撃の設定 ---")]
    public GameObject bulletPrefab;     // 弾のプレハブ
    public Transform firePoint;         // 弾が出る位置
    public float cooldown = 3.0f;       // クールタイム（秒）

    private float timer = 0f;           // クールタイムを数えるタイマー


    public bool isAttacking = false;

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

        Debug.Log("【遠距離】魔力を溜めている…（予兆）");
        yield return new WaitForSeconds(1.85f);

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

        yield return new WaitForSeconds(1.18f); // 後隙

        isAttacking = false;
        timer = cooldown; // クールタイム開始
    }
}