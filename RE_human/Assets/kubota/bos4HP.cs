using UnityEngine;

// 🌟 このスクリプトは「ボスのゲームオブジェクト」に貼り付けて使います
public class BossHealthSimple : MonoBehaviour
{
    // --- [ 設定項目 ] --------------------------------------------------
    // インスペクター（Unityの画面）から、ボスの最大HPを数字で設定できるようにする
    [Header("ステータス設定")]
    [SerializeField] private int maxHP = 100;

    // --- [ 内部の変数 ] ------------------------------------------------
    // ボスの「現在のHP」を記憶しておくための箱（整数を入れる int 型）
    private int Boss4HP;


    // --- [ ゲーム開始時の処理 ] ----------------------------------------
    // ゲームが始まった瞬間（最初の1フレーム目）に、Unityが自動で1回だけ実行する場所
    void Start()
    {
        // ゲーム開始時は、現在のHPを最大HP（満タン）と同じにする
        Boss4HP = maxHP;
    }


    // --- [ ダメージを受ける処理 ] --------------------------------------
    // プレイヤーの攻撃（近接・遠距離）がボスに当たったとき、外部から呼び出される関数
    // ( ) の中にある「damage」には、当たった攻撃の攻撃力の数字（15とか10）が入ってきます
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            Boss4HP -= 10;
            Debug.Log("ボス-10ダメージ");
        }
        // プレイヤーに当たった場合
        else if (collision.CompareTag("LongAttack"))
        {
            Debug.Log("ボス-5ダメージ");
            Boss4HP -= 5;
        }

        if (Boss4HP == 0)
        {
            // コンソール画面に「ボスを撃破した！」と表示する
            Debug.Log("ボスを撃破した！");

            // ボス自身のゲームオブジェクト（gameObject）を、ゲームの世界から完全に消滅させる
            Destroy(gameObject);
        }
    }

}