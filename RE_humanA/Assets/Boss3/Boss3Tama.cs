using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Boss3Projectile : MonoBehaviour
{
    [Header("弾の設定")]
    public float speed = 10f;
    public float lifeTime = 3f; // 画面外に出た時などのために自動消滅する時間
    public int damage = 1;      // プレイヤーに与えるダメージ量

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // 弾が重力型で落ちないようにする
        rb.gravityScale = 0f;
    }

    void Start()
    {
        // 一定時間後に自動で削除
        Destroy(gameObject, lifeTime);
    }

    // 発射時に方向を決める
    public void Launch(float direction)
    {
        // 進行方向に向けてスプライトの向きを反転させる
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;

        // Unity 6のlinearVelocityで移動力を与える
        rb.linearVelocity = new Vector2(direction * speed, 0f);
    }

    // 衝突判定
    void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤーに当たった場合
        if (collision.CompareTag("Player"))
        {
            // TODO: プレイヤーのダメージ処理をここに書く
            // collision.GetComponent<PlayerHealth>()?.TakeDamage(damage);

            Debug.Log("プレイヤーに手型攻撃がヒット！");
            Destroy(gameObject); // 弾を消す
        }

        // 地面や壁（インフラ）に当たったら消える（レイヤー名などは環境に合わせて調整）
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}