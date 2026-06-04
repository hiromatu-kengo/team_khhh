using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss4Hp : MonoBehaviour
{
    // --- [ 設定項目 ] --------------------------------------------------
    // インスペクター（Unityの画面）から、ボスの最大HPを数字で設定できるようにする
    [Header("ステータス設定")]
    [SerializeField] private int maxHP = 100;

   [Header("連動するUI設定")]
   [SerializeField] private Boss4HPBar bossHPBar;

    [Header("クリア後の移動先シーン名")]
    [SerializeField] private string nextSceneName;

    // --- [ 内部の変数 ] ------------------------------------------------
    // ボスの「現在のHP」を記憶しておくための箱（整数を入れる int 型）
    private int Boss4HP;


    // --- [ ゲーム開始時の処理 ] ----------------------------------------
    // ゲームが始まった瞬間（最初の1フレーム目）に、Unityが自動で1回だけ実行する場所
    void Start()
    {
        // ゲーム開始時は、現在のHPを最大HP（満タン）と同じにする
        Boss4HP = maxHP;

        if (bossHPBar != null)
        {
            bossHPBar.SetupBossHP(maxHP);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // --- ①近接攻撃が当たったとき ---
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            Boss4HP -= 10;
            Boss4HP = Mathf.Clamp(Boss4HP, 0, maxHP); // マイナスにいかないお守り

            if (bossHPBar != null) bossHPBar.UpdateHPBar(Boss4HP); // 💡ここで即UIを更新！
        }

        // --- ②遠距離攻撃が当たったとき ---
        if (collision.gameObject.CompareTag("LongAttack"))
        {
            Boss4HP -= 5;
            Boss4HP = Mathf.Clamp(Boss4HP, 0, maxHP); // マイナスにいかないお守り

            if (bossHPBar != null) bossHPBar.UpdateHPBar(Boss4HP); // 💡ここでも即UIを更新！
        }

        // --- ③死亡判定 ---
        if (Boss4HP <= 0)
        {
            Debug.Log("ボスを撃破した！");
            Destroy(gameObject);
            Invoke("GoToNextScene", 2.0f);
        }

    }
    void GoToNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
