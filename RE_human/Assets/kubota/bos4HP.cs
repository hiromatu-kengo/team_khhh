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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Groundタグならジャンプ回数をリセット
        if (collision.gameObject.CompareTag("LongAttack"))
        {
            Boss4HP -= 5;
        }

        if (Boss4HP <= 0)
        {
            // コンソール画面に「ボスを撃破した！」と表示する
            Debug.Log("ボスを撃破した！");

            // ボス自身のゲームオブジェクト（gameObject）を、ゲームの世界から完全に消滅させる
            Destroy(gameObject);
        }
    }
}