using UnityEngine;
using UnityEngine.UI; // 💡UIを操作するためにこれが必要！

public class Boss4HPBar : MonoBehaviour
{
    // インスペクターからSliderを紐付けるための変数
    [SerializeField] private Slider hpSlider;

    // ボスの最大HPをバーに設定する関数
    public void SetupBossHP(int maxHP)
    {
        hpSlider.maxValue = maxHP; // スライダーの最大値を設定
        hpSlider.value = maxHP;    // 最初は満タンにする
    }

    // HPが変動した時にバーを更新する関数
    public void UpdateHPBar(int currentHP)
    {
        hpSlider.value = currentHP; // 現在のHPをスライダーに反映
    }
}