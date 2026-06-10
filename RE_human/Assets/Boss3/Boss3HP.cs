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

    void Start()
    {
        Boss3HP = maxHP;
    }

    void Update()
    {
        // もしボスが死んでいたら、ストップウォッチをスタートする
        if (isDead)
        {
            // 毎フレーム、流れた時間（秒）をタイマーに足していく
            deathTimer += Time.deltaTime;

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
            Boss3HP -= 10;
            Boss3HP = Mathf.Clamp(Boss3HP, 0, maxHP);
        }

        // --- ②遠距離攻撃が当たったとき ---
        if (collision.gameObject.CompareTag("LongAttack"))
        {
            Debug.Log("5ダメージ");
            Boss3HP -= 5;
            Boss3HP = Mathf.Clamp(Boss3HP, 0, maxHP);
        }

        // --- ③死亡判定 ---
        if (Boss3HP <= 0)
        {
            isDead = true;

            Debug.Log("ボスを撃破した！");

        }
    }
}