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

    //シールドクールタイム
    float span = 5f;

    float delta = 5;

    //シールドを使える時間
    float span1 = 0.5f;

    float delta1 = 0;

    void Update()
    {

        this.delta += Time.deltaTime;

        //シールドが出ているなら出現時間タイマーを進める
        if (currentShield != null)
        {
            this.delta1 += Time.deltaTime;
        }
        // 押している間
        if (Keyboard.current.wKey.isPressed && (this.delta > this.span) && (this.delta1 < this.span1))
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
                
                //リセット
                this.delta = 0;

                //リセット
                this.delta1 = 0;

            }
        }
    }
}