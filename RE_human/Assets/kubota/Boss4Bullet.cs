using UnityEngine;

public class Boss4Bullet : MonoBehaviour
{
    [Header("--- 弾の設定 ---")]
    public float speed = 5.0f;       // 弾の飛ぶ速度
    public int damage = 5;           // プレイヤーに与えるダメージ量
    public float lifeTime = 10.0f;    // 弾の寿命（秒）。画面外に逃げた弾を消す用

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // 念のため、生成されてから指定秒数（lifeTime）が経ったら自動消滅させる
        Destroy(gameObject, lifeTime);
    }

    public void Setup(Vector2 direction)
    {
        // Unity 6の最新仕様「linearVelocity」を使って、指定された方向に等速直線運動させる
        rb.linearVelocity = direction * speed;

        // 弾の画像の向きを、飛んでいる方向（ベクトル）に合わせて回転させる処理
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤーに当たった場合
        if (collision.CompareTag("Player"))
        {
            Debug.Log("【弾】プレイヤーに命中！");


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