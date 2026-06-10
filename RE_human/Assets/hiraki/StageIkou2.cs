using UnityEngine;
using UnityEngine.SceneManagement;

public class StageIkou1 : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("Boss3Room");
        }
    }
}
