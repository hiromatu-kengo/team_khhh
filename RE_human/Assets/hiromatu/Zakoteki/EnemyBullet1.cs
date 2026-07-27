using UnityEngine;

public class EnemyBullet1 : MonoBehaviour
{
    [Header("念のための自動消滅時間（秒）")]
    public float autoDestroyTime = 10f;

    private GameObject ownerEnemy; // この弾を撃った敵を記憶する変数

    // 💡 敵側から「誰が撃ったか」を受け取る関数
    public void Initialize(GameObject shooter)
    {
        ownerEnemy = shooter;
    }

    void Start()
    {
        if (autoDestroyTime > 0f)
        {
            Destroy(gameObject, autoDestroyTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 💡 自分を撃った本人（エネミー）に当たった場合は何もしない（すり抜ける）
        if (collision.gameObject == ownerEnemy)
        {
            return;
        }

        // ① プレイヤーの攻撃にぶつかったら
        if (collision.CompareTag("PlayerAttack"))
        {
            BulletDefeat();
            return;
        }

        // ② プレイヤー本人にぶつかったら
        if (collision.CompareTag("Player"))
        {
            Debug.Log("プレイヤーに弾が当たった！ダメージ！");
            BulletDefeat();
            return;
        }

        // ③ ステージ（Ground）などにぶつかったら
        if (collision.CompareTag("Ground"))
        {
            BulletDefeat();
            return;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 💡 自分を撃った本人なら無視
        if (collision.gameObject == ownerEnemy)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ground"))
        {
            BulletDefeat();
        }
    }

    void BulletDefeat()
    {
        Destroy(gameObject);
    }
}