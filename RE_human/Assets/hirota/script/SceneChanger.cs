using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ChangeScene(string title)
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("title");
    }
}
