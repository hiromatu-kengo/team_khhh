using UnityEngine;
using UnityEngine.SceneManagement; // シーン切り替えに必須のライブラリ

public class GameOverManager : MonoBehaviour
{
    [Header("無効にする時間1秒")]
    [SerializeField] private float ignoreTime = 1.0f;

    static string m_stageName = "TitleScene";

    private float timer = 0.0f;//時間を数えるためのタイマー

    void Update()
    {
        //シーンが始まってから経過時間をタイマーに足していく
        timer += Time.unscaledDeltaTime;
        //設定した時間がたつまではこれより下の処理に進まない
        if (timer < ignoreTime)
        {
            return;
        }
        // 画面のどこかが左クリック（または画面タップ）された瞬間を検知
        if (Input.GetMouseButtonDown(0))
        {
            Time.timeScale = 1.0f;
            RestartGame();
        }
    }

    void RestartGame()
    {
        // 現在開いているシーン（ゲームオーバーになったステージ）の名前を取得
        string currentSceneName = m_stageName;

        // そのシーンを最初から読み込み直す（リトライ）
        FadeManager.Instance.LoadSceneWithFade(currentSceneName);

        // もし「常に最初のステージ1に戻したい」という場合は、以下のように名前を直接指定もできます
        // SceneManager.LoadScene("Stage1");
    }

    public static void SetSceneName(string sceneName)
    {
        m_stageName = sceneName;
    }
}