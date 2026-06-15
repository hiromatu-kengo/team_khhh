using UnityEngine;

public class ZakoTama : MonoBehaviour
{
    [Header("攻撃設定")]
    public GameObject EnemyLong;     // 発射する弾のプレハブ
    public Transform firePoint;         // 弾を発射する位置（銃口のエンプティオブジェクト）
    public Vector2 shotDirection = new Vector2(-1f, 0f); // 弾を飛ばす方向（初期値は左）
    public float shotSpeed = 5f;        // 弾の飛ぶ速度
    public float shotInterval = 2f;     // 何秒ごとに弾を撃つか

    private float shotTimer;

    void Start()
    {
        shotTimer = 0f;
    }

    void Update()
    {
        // 定期的に弾を撃つタイマー処理
        shotTimer += Time.deltaTime;
        if (shotTimer >= shotInterval)
        {
            Shot();
            shotTimer = 0f;
        }
    }

    // 💡弾を発射する関数
    void Shot()
    {
        // 弾のプレハブと発射位置（firePoint）が設定されているかチェック
        if (EnemyLong != null && firePoint != null)
        {
            // ① 弾を生成する（インスタンス化）
            GameObject newBullet = Instantiate(EnemyLong, firePoint.position, Quaternion.identity);

            // ② 生成した弾のRigidbody2Dを取得して、指定した方向に速度を与える
            Rigidbody2D bulletRb = newBullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                // 方向を正規化（長さを1に）してから、速度を掛ける
                bulletRb.linearVelocity = shotDirection.normalized * shotSpeed;
            }
        }
    }

    // 💡プレイヤーからの攻撃判定（前回のコードと同じ共通処理だ！）
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack") || collision.CompareTag("LongAttack"))
        {
            Destroy(gameObject);
        }
    }
}