using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    // [任意] もし時間経過でも一応消したい場合は残しておこう（念のためバグ対策）
    // 不要なら0にしてインスペクターで調整してくれ
    [Header("念のための自動消滅時間（秒）")]
    public float autoDestroyTime = 10f;

    void Start()
    {
        // 念のため、10秒経ったら絶対に消えるようにしておく（保険）
        if (autoDestroyTime > 0f)
        {
            Destroy(gameObject, autoDestroyTime);
        }
    }

    // ★★★ここが本命！何かにぶつかった瞬間の処理★★★
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 💡重要：何にぶつかったかによって処理を分ける

        // ① もしプレイヤーの攻撃（PlayerAttackタグ）にぶつかったら
        if (collision.CompareTag("PlayerAttack"))
        {
            // 弾は消滅する（プレイヤーの攻撃で弾が相殺されるイメージ）
            BulletDefeat();
            return; // 処理を抜ける
        }

        // ② もしプレイヤー本人（Playerタグ）にぶつかったら
        if (collision.CompareTag("Player"))
        {
            // 💡ここにプレイヤーにダメージを与える処理を入れる！（今はまだないので、弾が消えるだけ）
            Debug.Log("プレイヤーに弾が当たった！ダメージ！");

            // 弾は消滅する
            BulletDefeat();
            return;
        }

        // ③ もしステージ（Groundタグ）や壁などにぶつかったら
        if (collision.CompareTag("Ground")) // ステージに「Ground」タグをつけておくこと
        {
            // 弾は消滅する
            BulletDefeat();
            return;
        }
    }

    // 💡一応OnCollisionEnter2D側も対応（相手のColliderがTriggerじゃない場合用）
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Collision2Dの場合は collision.gameObject.CompareTag になるぞ！
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ground"))
        {
            BulletDefeat();
        }
    }

    // 弾が消滅するときの共通処理
    void BulletDefeat()
    {
        // ここに「ピシッ」というSEやエフェクトを入れるとクオリティが上がる！
        Destroy(gameObject);
    }
}