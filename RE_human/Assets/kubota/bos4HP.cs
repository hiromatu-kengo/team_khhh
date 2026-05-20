using UnityEngine;

// ボスのHPを管理するクラス
public class bos4HP : MonoBehaviour
{
    // ボスの最大HP
    public int maxHP = 100;

    // 現在のHP
    public int currentHP;

    // ゲーム開始時に1回実行
    void Start()
    {
        // 現在HPを最大HPにする
        currentHP = maxHP;
    }

    // ダメージを受ける処理
    public void TakeDamage(int damage)
    {
        // 現在HPからダメージ分減らす
        currentHP -= damage;

        // Consoleに現在HPを表示
        Debug.Log("Boss HP : " + currentHP);

        // HPが0以下になったら
        if (currentHP <= 0)
        {
            // 死亡処理
            Die();
        }
    }

    // ボス死亡処理
    void Die()
    {
        // Consoleに死亡表示
        Debug.Log("Boss Dead");

        // ボスを削除
        Destroy(gameObject);
    }
}