using UnityEngine;
using System.Collections;

public class Boss4GrabAttack : MonoBehaviour
{
    [Header("--- つかみ攻撃の設定 ---")]
    public Transform grabPoint;         // つかみ判定の中心
    public float grabRadius = 1.0f;     // つかみ判定の半径（近接より小さめ）
    public LayerMask playerLayer;       // プレイヤーのレイヤー
    public float cooldown = 4.0f;       // クールタイム（秒）

    [Header("--- 見た目の設定 ---")]
    public GameObject grabVisual;

    private float timer = 0f;
    public bool isAttacking = false;

    void Start()
    {
        // ★追加：ゲーム開始時は■の画像を非表示（OFF）にしておく
        if (grabVisual != null)
        {
            grabVisual.SetActive(false);
        }
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public bool CanAttack()
    {
        return (isAttacking == false && timer <= 0);
    }

    public void Execute()
    {
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        Debug.Log("【つかみ】腕を伸ばしている…（予兆）");
        yield return new WaitForSeconds(0.3f); // つかみは少し出が早いイメージ

        Debug.Log("【つかみ】ガシッ！判定発生！");

        // ★追加：攻撃の瞬間に画像を表示（ON）する！
        if (grabVisual != null) grabVisual.SetActive(true);

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(grabPoint.position, grabRadius, playerLayer);
        foreach (Collider2D player in hitPlayers)
        {
            Debug.Log("プレイヤーを捕まえた！大ダメージ！");
            // ここに大ダメージや拘束の処理を書く
        }

        yield return new WaitForSeconds(0.5f); // 失敗したときの隙

        // ★追加：攻撃が終わったら画像を隠す（OFF）！
        if (grabVisual != null) grabVisual.SetActive(false);

        isAttacking = false;
        timer = cooldown;
    }
}