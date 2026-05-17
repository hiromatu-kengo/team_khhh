using UnityEngine;

public class Boss4Guard : MonoBehaviour
{
    public bool canGuard = true;

    public float guardCooldown = 5f;

    private bool isGuarding = false;

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
        isGuarding = true;

        Debug.Log("ガード");

        yield return new WaitForSeconds(1f);

        isGuarding = false;

        yield return new WaitForSeconds(guardCooldown);

        canGuard = true;
    }
}