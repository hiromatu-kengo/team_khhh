using UnityEngine;
using UnityEngine.UI;

public class staminaMsnager : MonoBehaviour
{

   
    private Slider staminaSlider;

    private Vector3 initialScale;
    
    void Awake()
    {
        // 自身からSliderを取得
        staminaSlider = GetComponent<Slider>();

        //起動時の初期スケールを取得
        initialScale = transform.localScale;
    }

    private void LateUpdate()
    {
        //親のスケールが反転しても向きを生に保
        Vector3 currentScale = transform.localScale;

        //親の向きに合わせローカルスケールを補正
        float parentXScale = transform.parent.lossyScale.x;

        if (parentXScale < 0)
        {
            currentScale.x = -initialScale.x;        
        }
        else
        {
            currentScale.x = initialScale.x;
        }
        transform.localScale = currentScale;


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
