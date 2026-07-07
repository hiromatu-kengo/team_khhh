using UnityEngine;

public class GrabTrigger : MonoBehaviour
{
    [Header("つかんだ時にプレイヤーを拘束する位置")]
    public float damagePerSecond = 10f;

    void OnTriggerStay2D(Collider2D other)
    {
        // 触れたオブジェクトがプレイヤーだった場合
        if (other.CompareTag("Player"))
        {
            // ① プレイヤーのコントロールを奪う（プレイヤー側にそういったフラグやメソッドが必要）
            // 例: other.GetComponent<PlayerController>().GetGrabbed(transform.position);

            // ② プレイヤーを闇の中心（またはボスの手元）にじわじわ吸い寄せる
            other.transform.position = Vector3.Lerp(other.transform.position, transform.position, Time.deltaTime * 10f);

            // ③ 持続ダメージを与える（もしプレイヤーにダメージ機能があれば）
            // other.GetComponent<PlayerHealth>().TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }
}
