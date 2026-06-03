using UnityEngine;

public class Boss3RangeAttack : MonoBehaviour
{
    [Header("攻撃設定")]
    public GameObject projectilePrefab; // 手型の弾プレハブ
    public Transform firePoint;         // 弾の発射位置（ボスの手元など）
    public float attackCooldown = 3.0f; // 攻撃のクールダウン（秒）

    [Header("アニメーション（任意）")]
    private Animator animator;          // アニメーションを制御する場合

    private float cooldownTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // クールダウンのタイマーを進める
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    // Boss3Controlから呼ばれる攻撃命令
    public void TryAttack()
    {
        // クールダウン中なら何もしない
        if (cooldownTimer > 0) return;

        // クールダウン初期化
        cooldownTimer = attackCooldown;

        // アニメーションを再生（Animatorを使っている場合）
        if (animator != null)
        {
            animator.SetTrigger("RangeAttack");
        }

        // 弾を生成して発射
        FireProjectile();
    }

    void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("遠距離攻撃の設定が足りません！");
            return;
        }

        // 弾を生成
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // ボスの向き（localScale.x）に合わせて弾の飛ぶ方向を決める
        // Boss3ControlのLookAtPlayer()で右向きの時は1、左向きの時は-1になっている
        float direction = Mathf.Sign(transform.localScale.x);

        // 弾のスクリプトに方向を伝える
        Boss3Projectile projScript = projectile.GetComponent<Boss3Projectile>();
        if (projScript != null)
        {
            projScript.Launch(direction);
        }
    }
}