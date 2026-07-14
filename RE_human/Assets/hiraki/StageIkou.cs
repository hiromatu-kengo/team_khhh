using UnityEngine;
using UnityEngine.SceneManagement;

public class StageIkou : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            FadeManager.Instance.LoadSceneWithFade("Boss1Room");
        }
    }
}
