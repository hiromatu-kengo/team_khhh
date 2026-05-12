using UnityEngine;
using UnityEngine.InputSystem;

public class player_con : MonoBehaviour
{
    Rigidbody2D rigid2D;

    // 1回目ジャンプ力
    float firstJumpForce = 800.0f;

    // 2回目ジャンプ力
    float secondJumpForce = 600.0f;

    // 移動速度
    [SerializeField] float speed = 10f;

    // ジャンプ回数
    int jumpCount = 0;

    // 最大ジャンプ回数
    int maxJump = 2;

    Vector2 move;

    string jump;

    void Start()
    {
        // FPSを60に固定
        Application.targetFrameRate = 60;

        this.rigid2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //ボタンを押されたかどうかの処理はこちら

        // 最大回数未満ならジャンプ可能
        if (Keyboard.current.spaceKey.wasPressedThisFrame
            && jumpCount < maxJump)
        {
            // 1回目ジャンプ
            if (jumpCount == 0)
            {
                jump = "ikkai";
            }

            // 2回目ジャンプ
            else if (jumpCount == 1)
            {
                jump = "nikai";
            }

            // ジャンプ回数追加
            jumpCount++;
        }

        // 移動方向
        move = Vector3.zero;

        // Aキー
        if (Keyboard.current.aKey.isPressed)
        {
            move.x = -1;
        }

        // Dキー
        if (Keyboard.current.dKey.isPressed)
        {
            move.x = 1;
        }

    }

    private void FixedUpdate()
    {
        // 横移動
        transform.Translate(move * speed * Time.deltaTime);

        // 1段ジャンプ
        if (jump == "ikkai")
        {
            rigid2D.linearVelocity =
                new Vector2(rigid2D.linearVelocity.x, 0);

            rigid2D.AddForce(Vector2.up * firstJumpForce);

            jump = "";
        }

        // 2段ジャンプ
        if (jump == "nikai")
        {
            rigid2D.linearVelocity =
                new Vector2(rigid2D.linearVelocity.x, 0);

            rigid2D.AddForce(Vector2.up * secondJumpForce);

            jump = "";
        }
    }

    // 当たり判定
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Groundタグならリセット
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
        }
    }
}