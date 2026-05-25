using UnityEngine;

// ボスの攻撃判定を管理するクラス
public class BossAttack : MonoBehaviour
{
    // プレイヤーに与えるダメージ量
    public int damage = 10;

    // Triggerに何かが入った時に呼ばれる
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 当たった相手がPlayerか確認
        if (other.CompareTag("Player"))
        {
            // PlayerについているPlayerHPスクリプトを取得
            PlayerHP hp =
            other.GetComponent<PlayerHP>();

            // PlayerHPが見つかった場合
            if (hp != null)
            {
                // プレイヤーにダメージを与える
                hp.TakeDamage(damage);
                Debug.Log("ダメージ");
            }
        }
    }
}
