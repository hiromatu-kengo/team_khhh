using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class TitleManager : MonoBehaviour
{



    // インスペクターから、ゲーム本編のシーン名を指定できるようにする
    [Header("--- 移動先のシーン名 ---")]
    public string gameSceneName = "Stage1";

    /// <summary>
    /// スタートボタンが押されたときに実行する関数
    /// </summary>
    public void OnStartButton()
    {
        Debug.Log("ゲームを開始します！");
        // 指定した名前のシーンへジャンプする
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// 終了ボタンが押されたときに実行する関数
    /// </summary>
    public void OnExitButton()
    {
        Debug.Log("ゲームを終了します！");
        // ゲームを完全に終了する（※PCなどでビルドしたあとに有効になります）
        Application.Quit();
    }
}
