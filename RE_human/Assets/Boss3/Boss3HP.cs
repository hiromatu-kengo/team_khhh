using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss3Hp : MonoBehaviour
{
    [Header("ステータス設定")]
    [SerializeField] private int maxHP = 100;

    [Header("クリアしたときの移動先シーン名")]
    public string nextSceneName = "Stage4";

    private int Boss3HP;
    private bool isDead = false;
    private float deathTimer = 0.0f;

    // ★追加：アニメーションを制御するための変数
    private Animator animator;

    void Start()
    {
        Boss3HP = maxHP;

        // ★追加：ボスについているAnimatorコンポーネントを自動で取得する
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // もしボスが死んでいたら、ストップウォッチをスタートする
        if (isDead)
        {
            deathTimer += Time.deltaTime;

            // 2秒経ったら、シーンを切り替える！
            if (deathTimer >= 0.93f)
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // すでに死んでいたらこれ以降のダメージ計算をしない（死体蹴り防止）
        if (isDead) return;

        bool isHit = false; // ★追加：攻撃が当たったかどうかのフラグ

        // --- ①近接攻撃が当たったとき ---
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            Debug.Log("10ダメージ");
            Boss3HP -= 10;
            Boss3HP = Mathf.Clamp(Boss3HP, 0, maxHP);
            isHit = true; // 当たったよ！
        }

        // --- ②遠距離攻撃が当たったとき ---
        if (collision.gameObject.CompareTag("LongAttack"))
        {
            Debug.Log("5ダメージ");
            Boss3HP -= 5;
            Boss3HP = Mathf.Clamp(Boss3HP, 0, maxHP);
            isHit = true; // 当たったよ！
        }

        // --- ★追加：攻撃が当たっていて、かつまだ生きていればダメージアニメーションを再生！ ---
        if (isHit && Boss3HP > 0)
        {
            if (animator != null)
            {
                animator.SetTrigger("Boss3Damage"); // さっき作った真っ赤になるやつを再生！
            }
        }

        // --- ③死亡判定 ---
        if (Boss3HP <= 0)
        {
            isDead = true;
            Debug.Log("ボスを撃破した！");

            // ★追加：死亡アニメーション（Die）を再生！
            if (animator != null)
            {
                animator.Play("Boss3Death");
            }

            // ★プロのひと工夫：死んだらボスの当たり判定を消す！
            // これでプレイヤーがボスをすり抜けられるようになり、理不尽なダメージを受けなくなります
            Collider2D myCollider = GetComponent<Collider2D>();
            if (myCollider != null)
            {
                myCollider.enabled = false;
            }

            // もしボスを物理（Rigidbody2D）で動かしているなら、ここで動きも止めるとGOOD！
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; // Unity 6仕様の速度停止（古いバージョンなら velocity = Vector2.zero）
                rb.bodyType = RigidbodyType2D.Kinematic; // 重力などで落ちないように固定
            }
        }
    }
}