using UnityEngine;

public class Boss4Bullet : MonoBehaviour
{
    [Header("--- 弾の設定 ---")]
    public float speed = 5.0f;       // 弾の飛ぶ速度
    public int damage = 5;           // プレイヤーに与えるダメージ量
    public float lifeTime = 4.0f;    // 弾の寿命（秒）。画面外に逃げた弾を消す用

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // 念のため、生成されてから指定秒数（lifeTime）が経ったら自動消滅させる
        // これを入れ忘れると、画面外に無限に弾が溜まってゲームが重くなるぞ！
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// ボス（Boss4RangeAttack）から呼び出されて、弾を飛ばす方向を設定する関数
    /// </summary>
    public void Setup(Vector2 direction)
    {
        // Unity 6の最新仕様「linearVelocity」を使って、指定された方向に等速直線運動させる
        rb.linearVelocity = direction * speed;

        // 弾の画像の向きを、飛んでいる方向（ベクトル）に合わせて回転させる処理
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    /// <summary>
    /// 何かに当たった瞬間に自動で呼ばれる判定関数
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 【重要】プレイヤーに当たった場合
        if (collision.CompareTag("Player"))
        {
            Debug.Log("【弾】プレイヤーに命中！");

            // ★チーム連携ポイント：プレイヤー側のスクリプト（例：PlayerController）を取得してダメージを与える
            // ※ プレイヤー担当の子が作っているダメージ関数の名前に合わせてね！
            // var player = collision.GetComponent<PlayerController>();
            // if (player != null)
            // {
            //     player.TakeDamage(damage);
            // }

            // プレイヤーに当たったら弾は消える
            Destroy(gameObject);
        }

        // 地形（床や壁）に当たった場合
        else if (collision.CompareTag("Ground"))
        {
            Debug.Log("【弾】壁か床に当たって消滅。");
            Destroy(gameObject);
        }
    }
}