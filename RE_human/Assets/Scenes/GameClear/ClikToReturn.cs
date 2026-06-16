using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToReturn : MonoBehaviour
{
    [Header("title")]
    [SerializeField] private string startSceneName = "title";

    [Header("無効にする時間1秒")]
    [SerializeField] private float ignoreTime = 1.0f;

    private float timer = 0.0f;//時間を数えるためのタイマー

    void Update()
    {
        //シーンが始まってから経過時間をタイマーに足していく
        timer += Time.deltaTime;
        //設定した時間がたつまではこれより下の処理に進まない
        if(timer < ignoreTime)
        {
            return;
        }
        // 画面のどこかがクリック（またはタップ）されたかを毎フレーム監視
        if (Input.GetMouseButtonDown(0))
        {
            // 指定したシーンに切り替える
            SceneManager.LoadScene(startSceneName);
        }
    }
}