using UnityEngine;
using UnityEngine.UI;

public class staminaMsnager : MonoBehaviour
{

   
    private Slider staminaSlider;

   
    
    void Awake()
    {
        // 自身からSliderを取得
        staminaSlider = GetComponent<Slider>();
     
    }

    
    //最大値を設定
    public void SetMaxStamina(float maxStamina)
    {
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = maxStamina;
        }
    }


    //現在のスタミナ表示を更新
    public void UpdateStamina(float currentStamina)
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina;
        }

    }
}
