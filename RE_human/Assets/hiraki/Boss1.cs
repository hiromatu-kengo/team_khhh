using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("HP")]
    public int maxHP = 100;

    private int currentHP;

    [Header("通常移動")]
    public float moveSpeed = 2f;

    [Header("突進")]
    public float chargeSpeed = 10f;
    public float chargeDistance = 5f;
    public float chargeTime = 1f;

    [Header("ため時間")]
    public float prepareTime = 2f;

    [Header("硬直時間")]
    public float stunTime = 1f;

    [Header("突進クールタイム")]
    public float cooldownTime = 10f;

    [Header("突進攻撃")]
    public int chargeDamage = 20;

    [Header("近接攻撃")]
    public float attackDistance = 1f;
    public int attackDamage = 10;
    public float attackCooldown = 1f;

    private float cooldownTimer = 0f;
    private float attackTimer = 0f;

    private Transform player;

    private bool isPreparing = false;
    private bool isCharging = false;
    private bool isStunned = false;

    private float chargeTimer = 0f;
    private float prepareTimer = 0f;
    private float stunTimer = 0f;

    private Vector2 chargeDirection;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // HP初期化
        currentHP = maxHP;
    }

    void Update()
    {
        // 突進クールタイム減少
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // 近接攻撃クールタイム減少
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        // 硬直中
        if (isStunned)
        {
            Stun();
            return;
        }

        // 突進中
        if (isCharging)
        {
            Charge();
            return;
        }

        // ため中
        if (isPreparing)
        {
            PrepareCharge();
            return;
        }

        // プレイヤーとの距離
        float distance =
            Vector2.Distance(transform.position, player.position);

        // 近接攻撃
        if (distance <= attackDistance)
        {
            MeleeAttack();
        }

        // 突進
        else if (distance <= chargeDistance && cooldownTimer <= 0f)
        {
            StartPrepare();
        }

        // 通常追跡
        else
        {
            MoveToPlayer();
        }
    }

    // プレイヤーへ歩く
    void MoveToPlayer()
    {
        Vector2 direction =
            (player.position - transform.position).normalized;

        transform.position +=
            (Vector3)(direction * moveSpeed * Time.deltaTime);
    }

    // ため開始
    void StartPrepare()
    {
        isPreparing = true;
        prepareTimer = prepareTime;

        // 突進方向保存
        chargeDirection =
            (player.position - transform.position).normalized;

        Debug.Log("ため開始");
    }

    // ため処理
    void PrepareCharge()
    {
        prepareTimer -= Time.deltaTime;

        if (prepareTimer <= 0f)
        {
            isPreparing = false;
            isCharging = true;

            chargeTimer = chargeTime;

            Debug.Log("突進！");
        }
    }

    // 突進
    void Charge()
    {
        transform.position +=
            (Vector3)(chargeDirection * chargeSpeed * Time.deltaTime);

        chargeTimer -= Time.deltaTime;

        if (chargeTimer <= 0f)
        {
            isCharging = false;

            // 硬直開始
            isStunned = true;
            stunTimer = stunTime;

            // クールタイム開始
            cooldownTimer = cooldownTime;

            Debug.Log("硬直");
        }
    }

    // 硬直処理
    void Stun()
    {
        stunTimer -= Time.deltaTime;

        if (stunTimer <= 0f)
        {
            isStunned = false;

            Debug.Log("行動再開");
        }
    }

    // 近接攻撃
    void MeleeAttack()
    {
        // クールタイム中
        if (attackTimer > 0f)
        {
            return;
        }

        Debug.Log("近接攻撃！");

        PlayerHP playerHP =
            player.GetComponent<PlayerHP>();

        if (playerHP != null)
        {
            playerHP.TakeDamage(attackDamage);
        }

        // 攻撃クールタイム
        attackTimer = attackCooldown;
    }

    // ダメージを受ける
    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        Debug.Log("ボスHP : " + currentHP);

        // HP0以下で死亡
        if (currentHP <= 0)
        {
            Die();
        }
    }

    // 死亡処理
    void Die()
    {
        Debug.Log("ボス撃破！");

        Destroy(gameObject);
    }

    // 突進ヒット
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 突進中のみ
        if (!isCharging)
        {
            return;
        }

        // プレイヤー判定
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHP playerHP =
                collision.gameObject.GetComponent<PlayerHP>();

            if (playerHP != null)
            {
                playerHP.TakeDamage(chargeDamage);
            }

            Debug.Log("プレイヤーに突進ヒット！");
        }
    }
}