using UnityEngine;
using System.Collections;

public class Boss4Guard : MonoBehaviour
{
    [Header("--- ガードの設定 ---")]
    public float guardDuration = 2.0f;  // ガードしている時間（秒）
    public float cooldown = 5.0f;       // ガードのクールタイム（秒）

    private float timer = 0f;

    // isAttackingと同じように、今はガード中かどうかを判定する変数
    public bool isGuarding = false;

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

        // ※ここでボスのダメージ受け判定スクリプトを無敵にしたり、
        // アニメーションをガード状態にする処理を呼び出します。

        yield return new WaitForSeconds(guardDuration); // ガードしている時間

        Debug.Log("【ガード】盾を下ろした。");

        isGuarding = false;
        timer = cooldown;
    }
}