using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    //どこからでも「FadeManager.Instance」で呼べるようにする(シングルトン)
    public static FadeManager Instance { get; private set; }

    [UnitHeaderInspectable("フェード用のCanvasGroup")]
    public CanvasGroup fadeCanvasGroup;
    [Header("フェードにかかる時間")]
    public float fadeDuration = 0.5f;
    private bool isFading = false;

    private void Awake()
    {
        //最初の１つだけを残し、シーンが切り替わっても消えないようにする（元の形をキープ！）
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // すでに前のシーンから来たManagerがいるなら、新しい方は消える（正しい動きです）
            Destroy(gameObject);
            return;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //ゲーム起動時や、最初のシーンに入った時はフェードインから始める
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 1f;
            StartCoroutine(FadeIn());
        }
    }

    public void LoadSceneWithFade(string sceneName)
    {
        if (isFading) return;

        // 🌟 追加した安全対策①：「TitleScene」と呼ばれたら「title」に直してあげる
        if (sceneName == "TitleScene")
        {
            sceneName = "title";
        }

        StartCoroutine(TransitionCoroutine(sceneName, -1));
    }

    public void LoadSceneWithFade(int sceneIndex)
    {
        if (isFading) return;
        StartCoroutine(TransitionCoroutine("", sceneIndex));
    }

    private IEnumerator TransitionCoroutine(string sceneName, int sceneIndex)
    {
        isFading = true;

        //1.画面を暗くする
        yield return StartCoroutine(FadeOut());

        //2.シーンを切り替える
        if (sceneIndex >= 0)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }

        //シーンが完全に読み込まれるのを待つ
        yield return null;

        //3.画面を明るくする
        yield return StartCoroutine(FadeIn());

        isFading = false;
    }

    private IEnumerator FadeOut()
    {
        // 🌟 追加した安全対策②：Canvasがセットされていなかったらエラーを防ぐ
        if (fadeCanvasGroup == null) yield break;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            //Time.unscaledDeltaTimeを使うことで、
            //ボス戦のヒットストップ(Time.timeScale = 0)中でもフェードが進むようにする
            timer += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;
    }

    private IEnumerator FadeIn()
    {
        if (fadeCanvasGroup == null) yield break;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
    }
}
