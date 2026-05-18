using UnityEngine;

public class Boss4Guard : MonoBehaviour
{
    public bool canGuard = true;

    public float guardCooldown = 5f;

    

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            if (canGuard)
            {
                StartCoroutine(GuardCoroutine());
            }
        }
    }

    System.Collections.IEnumerator GuardCoroutine()
    {
        canGuard = false;
        

        Debug.Log("ガード");

        yield return new WaitForSeconds(1f);

        

        yield return new WaitForSeconds(guardCooldown);

        canGuard = true;
    }
}