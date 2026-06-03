using UnityEngine;
using UnityEngine.InputSystem;

public class player_LongAttaack : MonoBehaviour
{

    //遠距離攻撃のfab入れ
    [SerializeField] GameObject LongAttackfab;

    [SerializeField] private player_con player_con;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!player_con.kirikae)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                //プレイヤーの向き
                float direction = Mathf.Sign(transform.localScale.x);

                //マウスの位置
                Vector3 mouseScreenPos = Input.mousePosition;
                //カメラから見たマウスの位置に変換
                Vector3 mouseWorldPos2D = Camera.main.ScreenToWorldPoint(mouseScreenPos);
               float mousePos = mouseWorldPos2D.x;
                if (direction < mousePos && direction > 0)
                {
                    //プレイヤーの目の前の位置
                    Vector3 spawnPos = transform.position + Vector3.right * direction * 1f;
                    //出現させる
                    Instantiate(LongAttackfab, spawnPos, Quaternion.identity);
                }

                if (direction > mousePos && direction < 0)
                {
                    //プレイヤーの目の前の位置
                    Vector3 spawnPos = transform.position + Vector3.right * direction * 1f;
                    //出現させる
                    Instantiate(LongAttackfab, spawnPos, Quaternion.identity);
                }
            }

        }
    }
    
}
