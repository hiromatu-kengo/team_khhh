using UnityEngine;

// ボスのガード処理
public class Boss4Guard : MonoBehaviour
{
    // ガード可能かどうか
    // true = ガードできる
    // false = クールタイム中
    public bool canGuard = true;

    // ガード後のクールタイム時間
    public float guardCooldown = 5f;

    // Triggerに何か入った時に呼ばれる
    void OnTriggerEnter2D(Collider2D other)
    {
        // 当たったものがプレイヤーの弾か確認
        if (other.CompareTag("PlayerBullet"))
        {
            // ガード可能なら
            if (canGuard)
            {
                // ガード処理開始
                StartCoroutine(GuardCoroutine());
            }
        }
    }

    // コルーチン（時間を待てる処理）
    System.Collections.IEnumerator GuardCoroutine()
    {
        // ガード中は再ガード禁止
        canGuard = false;

        // ガード開始
        Debug.Log("ガード");

        // ガードしている時間
        yield return new WaitForSeconds(1f);

        // ここにガードアニメーション終了処理を入れてもOK

        // クールタイム開始
        yield return new WaitForSeconds(guardCooldown);

        // 再びガード可能にする
        canGuard = true;
    }
}