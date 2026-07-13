using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using NUnit.Framework;



public class TitleManager : MonoBehaviour
{



    // インスペクターから、ゲーム本編のシーン名を指定できるようにする
    [Header("--- 移動先のシーン名 ---")]
    public string gameSceneName = "Stage1";

    [Header("点滅させるボタンのCanvasGroup")]
    public CanvasGroup startButtonCanvas;
    public CanvasGroup exitButtonCanvas;

    [Header("点滅の設定")]
    [Tooltip("点滅の周期（秒）")]
    public float blinkSpeed = 2.0f;

    //[Range(0f, 1f)]
    [Tooltip("点滅の最小透明度")]
    public float minAlpha = 0.3f;

    private void Update()
    {
        //ゆっくり点滅させる計算
        float wave = (Mathf.Sin(Time.time * blinkSpeed) + 1.0f) / 2.0f; // 0～1の値を作る
        //設定した一番薄い透明度からくっきりの間を滑らかに変化させる
        float currentAlpha = Mathf.Lerp(minAlpha, 1.0f, wave);
        //それぞれのボタンの透明度を舞フレーム更新する
        if (startButtonCanvas != null)
        {
            startButtonCanvas.alpha = currentAlpha;
        }
        if (exitButtonCanvas != null)
        {
            exitButtonCanvas.alpha = currentAlpha;
        }
    }

    /// <summary>
    /// スタートボタンが押されたときに実行する関数
    /// </summary>
    public void OnStartButton()
    {
        Debug.Log("ゲームを開始します！");
        // もしFadeManagerがシーン内に見つかれば、綺麗にフェードして遷移します
        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.LoadSceneWithFade(gameSceneName);
        }
        else
        {
            // 🌟もしFadeManagerがどこにもいなくても、エラーで止まらずにステージ1へ直接進みます！
            Debug.LogWarning("FadeManagerがシーン内に見つからないため、通常ロードでシーンを切り替えます。");
            SceneManager.LoadScene(gameSceneName);
        }
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
