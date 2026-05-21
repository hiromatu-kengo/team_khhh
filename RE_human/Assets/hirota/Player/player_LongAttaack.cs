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
            //プレイヤーの目の前の位置
            Vector3 spawnPos = transform.position + Vector3.right * 1;

            //出現させる
            Instantiate(LongAttackfab, spawnPos, Quaternion.identity);
        }
    }
}
