using UnityEngine;
using UnityEngine.SceneManagement; // シーン切り替えに必須のライブラリ

public class GameOverManager : MonoBehaviour
{
    [Header("無効にする時間1秒")]
    [SerializeField] private float ignoreTime = 1.0f;

    private float timer = 0.0f;//時間を数えるためのタイマー

    void Update()
    {
        //シーンが始まってから経過時間をタイマーに足していく
        timer += Time.deltaTime;
        //設定した時間がたつまではこれより下の処理に進まない
        if (timer < ignoreTime)
        {
            return;
        }
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