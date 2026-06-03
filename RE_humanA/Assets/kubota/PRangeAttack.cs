using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("--- 遠距離攻撃の設定 ---")]
    public GameObject bulletPrefab;     // プレイヤーの弾プレハブ
    public Transform firePoint;         // 弾が出る位置（銃口や手の先）
    public KeyCode shootKey = KeyCode.J; // 発射するキー
    public float cooldown = 0.5f;       // 連射速度（秒）

    private float timer = 0f;

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        // キーが押され、クールタイムが終わっていたら発射
        if (Input.GetKeyDown(shootKey) && timer <= 0)
        {
            Shoot();
            timer = cooldown; // クールタイム開始
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // 弾を生成
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            PlayerBullet bulletScript = bullet.GetComponent<PlayerBullet>();

            if (bulletScript != null)
            {
                // プレイヤーの向いている方向（localScaleのプラスマイナス）を取得
                // 右向きなら 1、左向きなら -1 になります
                float directionX = Mathf.Sign(transform.localScale.x);
                Vector2 direction = new Vector2(directionX, 0).normalized;

                // 弾に進む方向を伝える
                bulletScript.Setup(direction);
            }
        }
    }
}
