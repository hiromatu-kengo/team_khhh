using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearSceneManager : MonoBehaviour
{
    public void NextScene()
    {
        SceneManager.LoadScene("GameStart");
    }
}