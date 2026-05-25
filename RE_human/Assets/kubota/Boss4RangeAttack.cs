using UnityEngine;
using System.Collections;

public class Boss4RangeAttack : MonoBehaviour // クラス名がこれになっているね！
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float cooldown = 3.0f;

    private float timer = 0f;

    // ★★★ これが抜けているか、privateになっていないか確認！ ★★★
    public bool IsAttacking { get; private set; } = false;

    void Update()
    {
        if (timer > 0) timer -= Time.deltaTime;
    }

    public bool CanAttack() => !IsAttacking && timer <= 0;

    public void Execute(Transform player)
    {
        StartCoroutine(AttackRoutine(player));
    }

    IEnumerator AttackRoutine(Transform player)
    {
        IsAttacking = true; // ★ここでも使っているよ
        Debug.Log("【遠距離】パワーを溜めている…");
        yield return new WaitForSeconds(0.7f);

        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Boss4Bullet bulletScript = bullet.GetComponent<Boss4Bullet>();
            if (bulletScript != null)
            {
                Vector2 direction = (player.position - firePoint.position).normalized;
                bulletScript.Setup(direction);
            }
            
        }

        yield return new WaitForSeconds(0.4f);
        IsAttacking = false; // ★ここでも使っているよ
        timer = cooldown;
    }
}