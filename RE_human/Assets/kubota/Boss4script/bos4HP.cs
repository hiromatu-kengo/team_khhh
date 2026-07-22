using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class bos4HP : MonoBehaviour
{
    [Header("ステータス設定")]
    [SerializeField] private int maxHP = 100;
    public string nextSceneName = "gameclear";

    [Header("--- 必要な参照（ここをInspectorで指定） ---")]
    public Boss4Controller bossController; // 親にあるコントローラー
    public Animator animator;             // 親にあるアニメーター
    public SpriteRenderer spriteRenderer; // 親にあるスプライト（色変え用）

    private int Boss4HP;
    public bool isDead = false;
    private float deathTimer = 0.0f;
    
    public Material flashmaterial;
    private Material originalMaterial;

    void Start()
    {
        Boss4HP = maxHP;
        // 最初から親のスプライトを持ってくる
        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material;
        }

 //       StartCoroutine(FlashEffect());
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

        bool hit = false;

        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            Boss4HP -= 10;
            hit = true;
        }
        else if (collision.gameObject.CompareTag("LongAttack"))
        {
            Boss4HP -= 5;
            hit = true;
        }

        if(hit)
        { 
            Boss4HP = Mathf.Clamp(Boss4HP, 0, maxHP);

            StopAllCoroutines(); // 前のフラッシュを止めてから開始
            StartCoroutine(FlashEffect());


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

    private IEnumerator FlashEffect()
    {
        // 1. 半透明の赤にする（R=1, G=0, B=0, A=0.5）
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 0f, 0f, 0.5f);
        }

        // 1. 真っ赤にする
 /*       if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }
 */
        yield return new WaitForSeconds(0.1f);

        // 2. 元の色に戻す
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

}