using UnityEngine;

public class WarpHitbox : MonoBehaviour
{
    [Header("--- 当たり判定用のオブジェクト（子） ---")]
    public GameObject grabTriggerArea;

    void Start()
    {
        if (grabTriggerArea != null)
        {
            grabTriggerArea.SetActive(false);
            Debug.Log("【確認】ゲーム開始：当たり判定をOFFにしました");
        }
        else
        {
            Debug.LogError("【エラー】GrabTriggerAreaがアタッチされていません！インプレクターを確認してください！");
        }
    }

    public void EnableHitbox()
    {
        if (grabTriggerArea != null)
        {
            grabTriggerArea.SetActive(true);
            Debug.Log("【成功！】アニメーションイベントから命令が届き、当たり判定がONになりました！");
        }
    }

    public void DisableHitbox()
    {
        if (grabTriggerArea != null)
        {
            grabTriggerArea.SetActive(false);
            Debug.Log("【成功！】当たり判定がOFFになりました！");
        }
    }
}