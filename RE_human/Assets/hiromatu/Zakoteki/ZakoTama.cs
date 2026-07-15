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
            // 生成した弾に、このファイルの下部で定義している「衝突時に消えるコンポーネント」を自動で貼り付けます。
            BulletCollisionBehavior behavior = newBullet.AddComponent<BulletCollisionBehavior>();
            behavior.Initialize(gameObject); // 弾に「撃った本人（このエネミー）」を教えて、自分への衝突を無視させます。

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
    public class BulletCollisionBehavior : MonoBehaviour
    {
        private GameObject creator; // 弾を撃った本人（エネミー）

        // 弾を撃った本人を登録する関数
        public void Initialize(GameObject creatorObj)
        {
            creator = creatorObj;

            // 🌟安全対策：もし何にもぶつからずに画面外へ飛んでいった場合、
            // メモリ節約のために5秒後に自動で弾を消滅させます。
            Destroy(gameObject, 5.0f);
        }

        // パターンA：トリガー（すり抜ける壁やキャラ）に当たったとき
        private void OnTriggerEnter2D(Collider2D collision)
        {
            HandleCollision(collision.gameObject);
        }

        // パターンB：物理的な衝突（すり抜けない固い壁やキャラ）に当たったとき
        private void OnCollisionEnter2D(Collision2D collision)
        {
            HandleCollision(collision.gameObject);
        }

        // 共通の衝突判定処理
        private void HandleCollision(GameObject hitObject)
        {
            // 1. 自分を撃ったエネミー本人（creator）に当たった場合は、何もせず無視する
            // 2. 弾自身（gameObject）に当たった場合も無視する
            if (hitObject == creator || hitObject == gameObject)
            {
                return;
            }

            // それ以外のもの（ステージの壁、地面、プレイヤーなど）に当たったら、弾を消滅させる
            Destroy(gameObject);
        }
    }
}