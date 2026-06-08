using UnityEngine;
using UnityEngine.UI;

public class attackChangeManeger : MonoBehaviour
{
    // プレイヤーのスクリプトをインスペクターから紐付ける
    [SerializeField] private player_con playerController;

    [Header("UI要素")]
    [SerializeField] private GameObject meleeUI;  // 近接攻撃（kirikae = trueのとき）
    [SerializeField] private GameObject rangedUI; // 遠距離攻撃（kirikae = falseのとき）

    // 前フレームの状態を記憶しておくための変数（毎フレーム無駄にUIを更新しないため）
    private bool lastKirikaeState;

    void Start()
    {
        if (playerController != null)
        {
            // 初期状態を同期
            lastKirikaeState = playerController.kirikae;
            UpdateUI(lastKirikaeState);
        }
    }

    void Update()
    {
        if (playerController == null) return;

        // プレイヤーのkirikae状態に変化があったかチェック
        if (playerController.kirikae != lastKirikaeState)
        {
            // 状態が変化していればUIを更新
            lastKirikaeState = playerController.kirikae;
            UpdateUI(lastKirikaeState);
        }
    }

    void UpdateUI(bool isMelee)
    {
        // player_conの仕様に合わせて調整
        // コードを見ると「if (kirikae) { 近接攻撃 }」となっているので
        // kirikae = true のときが近接、false のときが遠距離（仮）になります
        meleeUI.SetActive(isMelee);
        rangedUI.SetActive(!isMelee);
    }
}