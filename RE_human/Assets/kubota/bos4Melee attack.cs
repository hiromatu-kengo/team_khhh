using UnityEngine;

public class Bos4Attack : MonoBehaviour
{
    [Header("Damage")]
    public int meleeDamage = 10;
    public int rangeDamage = 5;
    public int grabDamage = 20;

    [Header("Projectile")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    private bool canAttack = true;

    public float attackCooldown = 2f;

    public void MeleeAttack()
    {
        if (!canAttack)
            return;

        StartCoroutine(MeleeCoroutine());
    }

    System.Collections.IEnumerator MeleeCoroutine()
    {
        canAttack = false;

        Debug.Log("近距離攻撃");

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    public void RangeAttack()
    {
        if (!canAttack)
            return;

        StartCoroutine(RangeCoroutine());
    }

    System.Collections.IEnumerator RangeCoroutine()
    {
        canAttack = false;

        Debug.Log("遠距離攻撃");

        Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
   }

    public void GrabAttack()
    {
        if (!canAttack)
            return;

        StartCoroutine(GrabCoroutine());
    }

    System.Collections.IEnumerator GrabCoroutine()
    {
        canAttack = false;

        Debug.Log("つかみ攻撃");

        yield return new WaitForSeconds(2f);

        canAttack = true;
    }
}