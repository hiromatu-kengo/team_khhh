using UnityEngine;
using System.Collections; // ★コルーチン（IEnumerator）を使うために追加！

public class Boss3RangeAttack : MonoBehaviour
{
    [Header("ターゲット設定")]
    public Transform playerTransform;

    [Header("攻撃設定")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float attackCooldown = 3.0f;

    [Header("--- タイミング調整（インスペクターで秒数を設定） ---")]
    [Tooltip("アニメーションが始まってから、実際に弾が出るまでの時間（溜め）")]
    public float chargeTime = 0.5f;
    [Tooltip("弾が出たあと、次の行動に移れるようになるまでの時間（後隙）")]
    public float recoveryTime = 0.5f;

    [Header("アニメーション（任意）")]
    private Animator animator;

    private float cooldownTimer = 0f;

    // ★追加：今攻撃中かどうかを管理するフラグ（移動スクリプトなどで使ってね）
    public bool isAttacking = false;

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

    /// <summary>
    /// 攻撃を試みる（メインAIなどから呼び出す）
    /// </summary>
    public void TryAttack()
    {
        // ★攻撃中、またはクールタイム中は次の攻撃を受け付けない
        if (isAttacking || cooldownTimer > 0) return;

        // コルーチンを起動して、一連の攻撃流れをスタート！
        StartCoroutine(AttackRoutine());
    }

    // ★追加：時間差の処理を行うコルーチン
    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // 1. 攻撃の瞬間は足を止める（Rigidbody2Dの速度を0にする）
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        // 2. アニメーションの再生を開始！
        if (animator != null)
        {
            animator.SetTrigger("RangeAttack");
        }

        // 3. 【溜め時間】弾が出るポーズになるまで、設定された秒数だけ待つ
        yield return new WaitForSeconds(chargeTime);

        // 4. 【弾の発射】ここで弾を生成して飛ばす！
        FireProjectile();

        // 5. 【後隙時間】弾を撃ち終わったあとの余韻ポーズの秒数だけ待つ
        yield return new WaitForSeconds(recoveryTime);

        // 6. 攻撃終了！次の行動ができるようになり、クールタイムが始まる
        isAttacking = false;
        cooldownTimer = attackCooldown;
    }

    // 弾を生成して飛ばす処理（中身は元のまま綺麗に残してあるよ！）
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

        // 実際の弾のクラス名「Boss3Bullet」と「Setup」に合わせました
        Boss3Bullet projScript = projectile.GetComponent<Boss3Bullet>();
        if (projScript != null)
        {
            projScript.Setup(targetDirection);
        }
    }
}