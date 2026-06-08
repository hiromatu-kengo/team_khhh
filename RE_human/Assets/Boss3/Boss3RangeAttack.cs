using UnityEngine;

public class Boss3RangeAttack : MonoBehaviour
{
    [Header("ターゲット設定")]
    public Transform playerTransform;

    [Header("攻撃設定")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float attackCooldown = 3.0f;

    [Header("アニメーション（任意）")]
    private Animator animator;

    private float cooldownTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    public void TryAttack()
    {
        if (cooldownTimer > 0) return;

        cooldownTimer = attackCooldown;

        if (animator != null)
        {
            animator.SetTrigger("RangeAttack");
        }

        FireProjectile();
    }

    void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null || playerTransform == null)
        {
            Debug.LogWarning("遠距離攻撃の設定、またはプレイヤーの設定が足りません！");
            return;
        }

        // 弾を生成
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // プレイヤーへの「方向（ベクトル）」を計算する
        Vector2 targetDirection = (playerTransform.position - firePoint.position);
        targetDirection.Normalize();

        // --- 【ここを修正！】実際の弾のクラス名「Boss3Bullet」と「Setup」に合わせました ---
        Boss3Bullet projScript = projectile.GetComponent<Boss3Bullet>();
        if (projScript != null)
        {
            // Launch ではなく、弾のスクリプトにある Setup メソッドを呼び出す
            projScript.Setup(targetDirection);
        }
    }
}