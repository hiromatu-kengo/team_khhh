using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Shield : MonoBehaviour
{
    // シールド位置
    float shieldPosition = 1.0f;

    // シールドfab
    [SerializeField] GameObject shieldfab;

    // 出ているシールド
    GameObject currentShield;

    void Update()
    {
        // 押している間
        if (Keyboard.current.wKey.isPressed)
        {
            // まだ出ていないなら生成
            if (currentShield == null)
            {
                // 向き
                float direction = Mathf.Sign(transform.localScale.x);

                // 出現位置
                Vector3 spawnPos =transform.position +Vector3.right * direction * shieldPosition;

                // 生成
                currentShield =Instantiate(shieldfab, spawnPos, Quaternion.identity);

                // プレイヤーの子にする
                currentShield.transform.parent = transform;
            }
        }
        else
        {
            // 離したら削除
            if (currentShield != null)
            {
                Destroy(currentShield);
            }
        }
    }
}