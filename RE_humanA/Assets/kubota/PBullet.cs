using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Header("--- 弾の設定 ---")]
    public float speed = 8.0f;       // 弾の速度
    public float lifeTime = 5.0f;    // 弾の寿命（秒）

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // 画面外に行って消えなかった時のために寿命で消す
        Destroy(gameObject, lifeTime);
    }

    public void Setup(Vector2 direction)
    {
        // 指定された方向にすっ飛んでいく
        rb.linearVelocity = direction * speed;

        // 左を向いて飛ぶときは画像の向きも反転させる
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 【超重要】ボスの「盾」に当たった場合
        if (collision.CompareTag("BossShield"))
        {
            Debug.Log("【ガード】プレイヤーの弾はボスの盾に防がれた！");
            Destroy(gameObject); // 弾だけが消滅する（ボスは無傷！）
        }

        // ボス本体（Enemy）に当たった場合
        else if (collision.CompareTag("Enemy"))
        {
            Debug.Log("ボス本体に命中！");

            // 💡ここでボスのHPを減らす処理を呼び出せます！
            // var boss = collision.GetComponent<Boss4Controller>();
            // if (boss != null) { boss.bossHP -= 10; }

            Destroy(gameObject); // 弾が消える
        }

        // 地形に当たった場合
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}  

