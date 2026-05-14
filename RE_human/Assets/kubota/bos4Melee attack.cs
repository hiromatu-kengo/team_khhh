using UnityEngine;

public class bos4Melleattack : MonoBehaviour
{
    public int bos4attack = 3;
    public float bos4Melleintarval = 1f;

    private Transform player;
    private float timer;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth hp = other.GetComponentInParent<PlayerHealth>();

        if (hp == null) return;

        Vector3 dir = (player.position - transform.position).normalized;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= 4f)
        {
            timer += Time.deltaTime;

            if (timer >= bos4Melleintarval)
            {
                hp.TakeDamage(bos4attack);
                timer = 0f;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            timer = 0f;
        }
    }
}
