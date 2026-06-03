using UnityEngine;
using System.Collections;

public class Boss4Guard : MonoBehaviour
{
    [Header("--- ガードの設定 ---")]
    public float guardDuration = 2.0f;  // ガードしている時間（秒）
    public float cooldown = 5.0f;       // ガードのクールタイム（秒）

    [Header("--- 見た目の設定 ---")]
    public GameObject guardVisual;      // ★追加：エディターで作った「盾のオブジェクト」を入れる枠

    private float timer = 0f;

    // isAttackingと同じように、今はガード中かどうかを判定する変数
    public bool isGuarding = false;

    void Start()
    {
        // ★追加：ゲーム開始時は盾の画像を非表示（OFF）にしておく
        if (guardVisual != null)
        {
            guardVisual.SetActive(false);
        }
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public bool CanGuard()
    {
        return (isGuarding == false && timer <= 0);
    }

    public void Execute()
    {
        StartCoroutine(GuardRoutine());
    }

    IEnumerator GuardRoutine()
    {
        isGuarding = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        Debug.Log("【ガード】盾を構えた！この間は遠距離攻撃を弾く！");

        // ★追加：ガードの瞬間に盾の画像を表示（ON）する！
        if (guardVisual != null)
        {
            guardVisual.SetActive(true);
        }

        // ※ここでボスのダメージ受け判定スクリプトを無敵にしたり、
        // アニメーションをガード状態にする処理を呼び出します。

        yield return new WaitForSeconds(guardDuration); // ガードしている時間

        Debug.Log("【ガード】盾を下ろした。");

        // ★追加：ガードが終わったら盾の画像を隠す（OFF）！
        if (guardVisual != null)
        {
            guardVisual.SetActive(false);
        }

        isGuarding = false;
        timer = cooldown;
    }
}