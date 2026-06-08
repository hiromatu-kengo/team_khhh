using UnityEngine;
using UnityEngine.SceneManagement; // シーン切り替えに必須のライブラリ

public class GameOverManager : MonoBehaviour
{
    void Update()
    {
        // 画面のどこかが左クリック（または画面タップ）された瞬間を検知
        if (Input.GetMouseButtonDown(0))
        {
            RestartGame();
        }
    }

    void RestartGame()
    {
        // 現在開いているシーン（ゲームオーバーになったステージ）の名前を取得
        string currentSceneName = SceneManager.GetActiveScene().name;

        // そのシーンを最初から読み込み直す（リトライ）
        SceneManager.LoadScene(currentSceneName);

        // もし「常に最初のステージ1に戻したい」という場合は、以下のように名前を直接指定もできます
        // SceneManager.LoadScene("Stage1");
    }
}