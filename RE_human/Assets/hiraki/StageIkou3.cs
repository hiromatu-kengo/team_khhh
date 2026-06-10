using UnityEngine;
using UnityEngine.SceneManagement;

public class StageIkou3 : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("Boss4Room");
        }
    }
}
