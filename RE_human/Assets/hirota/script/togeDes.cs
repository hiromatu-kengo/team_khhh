using UnityEngine;

public class togeDes : MonoBehaviour
{

    [SerializeField] private GameObject crystalEffectPrefab;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision. gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Player"))
        {
            Instantiate(crystalEffectPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}

