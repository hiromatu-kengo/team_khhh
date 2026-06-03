using UnityEngine;
using UnityEngine.UI;

public class shieldUI : MonoBehaviour
{
    private Image fillImage;
    private float maxCoolTime;

    void Awake()
    {
        // 自身のImageコンポーネントを取得
        fillImage = GetComponent<Image>();
    }

    // クールタイムの最大値を設定
    public void SetMaxCoolTime(float maxCoolTime)
    {
        this.maxCoolTime = maxCoolTime;
    }

    // 現在のクールタイムの進捗を更新
    public void UpdateCoolTime(float currentDelta)
    {
        if (fillImage != null && maxCoolTime > 0)
        {
            // delta (0～5) を ImageのFill Amount (0.0～1.0) の割合に変換する
            // 5秒時点で 5 / 5 = 1.0 満タン
            fillImage.fillAmount = currentDelta / maxCoolTime;
        }
    }
}
