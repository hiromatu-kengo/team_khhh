using UnityEngine;
using UnityEngine.InputSystem; 

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;

    private bool isPaused = false;

    void Update()
    {
        // Escキーが押された瞬間を検知
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame(); // すでにポーズ中なら再開
            }
            else
            {
                PauseGame(); // 動いているならポーズ
            }
        }
    }

    // ゲームを一時停止する処理
    public void PauseGame()
    {
        isPaused = true;

        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(true); // ポーズ画面を表示
        }

        Time.timeScale = 0f;
    }

    // ゲームを再開する処理
    public void ResumeGame()
    {
        isPaused = false;

        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(false); // ポーズ画面を非表示
        }

        Time.timeScale = 1f;
    }
}
