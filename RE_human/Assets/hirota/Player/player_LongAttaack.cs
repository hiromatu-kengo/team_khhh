using UnityEngine;
using UnityEngine.InputSystem;

public class player_LongAttaack : MonoBehaviour
{

    //遠距離攻撃のfab入れ
    [SerializeField] GameObject LongAttackfab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            //向き
            float direction = Mathf.Sign(transform.localScale.x);

            //プレイヤーの目の前の位置
            Vector3 spawnPos = transform.position + Vector3.right * direction * 1f;
            //出現させる
            GameObject bullet = Instantiate(LongAttackfab, spawnPos, Quaternion.identity);

            Vector3 bulletScale = bullet.transform.localScale;
            bulletScale.x = Mathf.Abs(bulletScale.x) * direction;

            bullet.transform.localScale = bulletScale;
        }
    }
    
}
