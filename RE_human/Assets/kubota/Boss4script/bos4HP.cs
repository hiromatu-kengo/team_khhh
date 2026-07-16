using UnityEngine;
using UnityEngine.SceneManagement;

public class bos4HP : MonoBehaviour
{
    [Header("ステータス設定")]
    [SerializeField] private int maxHP = 100;
    public string nextSceneName = "gameclear";

    [Header("ボスのコントローラーを指定")]
    public Boss4Controller bossController;

    public Animator animator;
    private int Boss4HP;
    public bool isDead = false;
    private float deathTimer = 0.0f;

    void Start()
    {
        Boss4HP = maxHP;
    }

    void Update()
    {
        if (isDead)
        {
            deathTimer += Time.deltaTime;

            // 2秒経ったら、シーンを切り替える
            if (deathTimer >= 1.5f)
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            Boss4HP -= 10;
        }
        else if (collision.gameObject.CompareTag("LongAttack"))
        {
            Boss4HP -= 5;
        }

        Boss4HP = Mathf.Clamp(Boss4HP, 0, maxHP);

        if (Boss4HP <= 0)
        {
            isDead = true;
            if (bossController != null)
            {
                bossController.enabled = false;
            }

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            if (animator != null)
            {
                animator.SetTrigger("Boss4death"); // 一度だけ呼び出す！
            }
            Debug.Log("ボスを撃破した！");
        }
    }
}