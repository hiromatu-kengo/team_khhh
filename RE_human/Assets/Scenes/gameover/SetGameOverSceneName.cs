using UnityEngine;
using UnityEngine.SceneManagement;

public class SetGameOverSceneName : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameOverManager.SetSceneName(SceneManager.GetActiveScene().name);
    }

}
