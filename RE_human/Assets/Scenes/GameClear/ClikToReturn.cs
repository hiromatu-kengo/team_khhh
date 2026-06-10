using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToReturn : MonoBehaviour
{
    [Header("title")]
    [SerializeField] private string startSceneName = "title";

    void Update()
    {
        // 画面のどこかがクリック（またはタップ）されたかを毎フレーム監視
        if (Input.GetMouseButtonDown(0))
        {
            // 指定したシーンに切り替える
            SceneManager.LoadScene(startSceneName);
        }
    }
}