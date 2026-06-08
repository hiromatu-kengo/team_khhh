using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss4Hp : MonoBehaviour
{
    // --- [ 設定項目 ] --------------------------------------------------
    // インスペクター（Unityの画面）から、ボスの最大HPを数字で設定できるようにする
    [Header("ステータス設定")]
    [SerializeField] private int maxHP = 100;



    [Header("クリア後の移動先シーン名")]
    [SerializeField] private string nextSceneName;

    // --- [ 内部の変数 ] ------------------------------------------------
    // ボスの「現在のHP」を記憶しておくための箱（整数を入れる int 型）
    private int Boss4HP;
    private bool isDead = false; // ★【追加】2回以上死亡処理が走らないためのガード

    // --- [ ゲーム開始時の処理 ] ----------------------------------------
    // ゲームが始まった瞬間（最初の1フレーム目）に、Unityが自動で1回だけ実行する場所
    void Start()
    {
        // ゲーム開始時は、現在のHPを最大HP（満タン）と同じにする
        Boss4HP = maxHP;
;
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (isDead) return; // すでに死んでいたらダメージ計算をしない

        // --- ①近接攻撃が当たったとき ---
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            Debug.Log("10ダメージ");
            Boss4HP -= 10;
            Boss4HP = Mathf.Clamp(Boss4HP, 0, maxHP); // マイナスにいかないお守り

        }

        // --- ②遠距離攻撃が当たったとき ---
        if (collision.gameObject.CompareTag("LongAttack"))
        {
            Debug.Log("10ダメージ");
            Boss4HP -= 50;
            Boss4HP = Mathf.Clamp(Boss4HP, 0, maxHP); // マイナスにいかないお守り

        }

        // --- ③死亡判定 ---
        if (Boss4HP <= 0)
        {
            Die();
            
        }

    }
    void Die()
    {
        isDead = true;
        Debug.Log("ボスを撃破した！");

        // ★【プロの工夫】死んだら移動スクリプト（Boss3Control）をOFFにしてワープを止める！
        Boss3Control movement = GetComponent<Boss3Control>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        // ★【プロの工夫】死んだら物理挙動を止めて、その場に崩れ落ちるようにする
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; // 物理の影響を受けなくする
        }

        // 2秒後にシーンを切り替える予約（自分自身がまだ生きているので、ちゃんと動くよ！）
        Invoke("GoToNextScene", 2.0f);
    }
    void GoToNextScene()
    {
        // 次のシーンへ移行
        SceneManager.LoadScene(nextSceneName);

        // シーンが切り替わるのでここで自分を消し去る（あるいは自動で消えるので書かなくてもOK）
        Destroy(gameObject);
    }
}
