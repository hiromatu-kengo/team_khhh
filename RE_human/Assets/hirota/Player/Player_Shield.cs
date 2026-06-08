using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Shield : MonoBehaviour
{
    // シールド位置
    float shieldPosition = 1.0f;

    // シールドfab
    [SerializeField] GameObject shieldfab;

    // からのオブジェクト
    GameObject currentShieldHolder;

    //シールドクールタイム
    float span = 5f;

    float delta = 5;

    //シールドを使える時間
    float span1 = 0.5f;

    float delta1 = 0;

    //紐づけ
    [SerializeField] shieldUI shieldUI;

    void Start()
    {
        // クールタイムの最大値を教える
        if (shieldUI != null)
        {
            shieldUI.SetMaxCoolTime(span);
        }
    }


    void Update()
    {

        this.delta += Time.deltaTime;

        if (shieldUI != null)
        {
            shieldUI.UpdateCoolTime(Mathf.Min(this.delta, span));
        }



        //シールドが出ているなら出現時間タイマーを進める
        if (currentShieldHolder != null)
        {
            this.delta1 += Time.deltaTime;
        }
        // 押している間
        if (Keyboard.current.wKey.isPressed && (this.delta > this.span) && (this.delta1 < this.span1))
        {
            // まだ出ていないなら生成
            if (currentShieldHolder == null)
            {
                // 向き
                float direction = Mathf.Sign(transform.localScale.x);

                // 出現位置
                Vector3 spawnPos =transform.position +Vector3.right * direction * shieldPosition;

                // 空のオブジェクト生成
                currentShieldHolder = new GameObject("ShieldHolder");
                currentShieldHolder.transform.position = spawnPos;

                // 空のオブジェクトの子にする
                GameObject shield = Instantiate(shieldfab, spawnPos, Quaternion.identity);
                shield.transform.SetParent(currentShieldHolder.transform);
                
                //向きを合わせる
                currentShieldHolder.transform.localScale = new Vector3(direction, 1, 1);
            }
            else
            {
                // 押しっぱの最中、プレイヤーが移動してもシールドが目の前
                // 毎フレーム位置をプレイヤーの目の前に更新
                float direction = Mathf.Sign(transform.localScale.x);
                Vector3 spawnPos = transform.position + Vector3.right * direction * shieldPosition;
                currentShieldHolder.transform.position = spawnPos;
                currentShieldHolder.transform.localScale = new Vector3(direction, 1, 1);
            }
        }
        else
        {
            // 離したら削除
            if (currentShieldHolder != null)
            {
                Destroy(currentShieldHolder);
                
                //リセット
                this.delta = 0;

                //リセット
                this.delta1 = 0;

            }
        }
    }
}