using UnityEngine;
using UnityEngine.SceneManagement;

public class StageIkou2 : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            FadeManager.Instance.LoadSceneWithFade("Boss3Room");
        }
    }
}
