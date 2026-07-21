using UnityEngine;

public class togeDes : MonoBehaviour
{

    [SerializeField] private GameObject crystalEffectPrefab;

    [SerializeField] private AudioClip breakSound;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision. gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            if (breakSound != null)
            {
                AudioSource.PlayClipAtPoint(breakSound, transform.position);
            }
            Instantiate(crystalEffectPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}

