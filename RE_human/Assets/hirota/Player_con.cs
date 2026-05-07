using UnityEngine;

public class Player_con : MonoBehaviour
{
    Rigidbody2D rigid2D;

    // ジャンプ力
    public float jumpForce = 10f;

    void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // スペースキーでジャンプ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 上方向に力を加える
            rigid2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
