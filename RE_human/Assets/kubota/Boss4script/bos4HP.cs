using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss4Hp : MonoBehaviour
{
    [Header("ステータス設定")]
    [SerializeField] private int maxHP = 100;

    [Header("クリアしたときの移動先シーン名")]
    public string nextSceneName = "gameclear";

    [Header("アニメーション（任意）")]
    private Animator animator;

    private int Boss4HP;
    private bool isDead = false;

    private float deathTimer = 0.0f;

    void Start()
    {
        Boss4HP = maxHP;
    }

    void Update()
    {
        // もしボスが死んでいたら、ストップウォッチをスタートする
        if (isDead)
        {
            // 毎フレーム、流れた時間（秒）をタイマーに足していく（Unityの基本技！）
            deathTimer += Time.deltaTime;

            if (animator != null)
            {
                animator.SetTrigger("Boss4death");
            }

            // 2秒経ったら、シーンを切り替える！
            if (deathTimer >= 2.0f)
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // すでに死んでいたらこれ以降のダメージ計算をしない（死体蹴り防止）
        if (isDead) return;

        // --- ①近接攻撃が当たったとき ---
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            Debug.Log("10ダメージ");
            Boss4HP -= 10;
            Boss4HP = Mathf.Clamp(Boss4HP, 0, maxHP);
        }

        // --- ②遠距離攻撃が当たったとき ---
        if (collision.gameObject.CompareTag("LongAttack"))
        {
            Debug.Log("5ダメージ");
            Boss4HP -= 5; 
            Boss4HP = Mathf.Clamp(Boss4HP, 0, maxHP);
        }

        // --- ③死亡判定 ---
        if (Boss4HP <= 0)
        {
            isDead = true;

            Debug.Log("ボスを撃破した！");
            
        }
    }

}