using UnityEngine;
using UnityEngine.InputSystem;

public class player_LongAttaack : MonoBehaviour
{

    //遠距離攻撃のfab入れ
    [SerializeField] GameObject LongAttackfab;

    [SerializeField] private player_con player_con;

    private Vector2 mouseWorldPosition;

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
                //マウスの座標を計算
                Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
                Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

                //プレイヤーからマウスへ方向ベクトを計算
                Vector2 directionToMouse = mouseWorldPos - (Vector2)transform.position;
                directionToMouse.Normalize(); 

                //弾を生成する位置を計算
                // プレイヤーの向いている方向
                float lookDirection = Mathf.Sign(transform.localScale.x);
                Vector3 spawnPos = transform.position + Vector3.right * lookDirection * 1f;

                // 弾を出現
                GameObject bullet = Instantiate(LongAttackfab, spawnPos, Quaternion.identity);


                //弾に方向ベクトルを掛けて速度を与える
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                if (bulletRb != null)
                {
                    float bulletSpeed = 15f; // 弾のスピード

                    // 方向ベクトル × スピード を弾の速度に
                    bulletRb.linearVelocity = directionToMouse * bulletSpeed;
                }


                //弾の見た目の向きをマウスの方に傾ける 
                // 弾の進行方向から角度を求め度数に変換
                float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
                // 弾のZ軸を回転させる
                bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            
        }
    }
    
}
