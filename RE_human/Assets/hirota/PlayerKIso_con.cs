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

    // プレイヤーのHP
    int playerHp;

    //プレイヤーのMaxHP
    int playerMaxHp = 5;

    //攻撃判定
    bool meleeAttack = false;

    //攻撃位置
    float attackPosition = 2.0f;

    Vector2 move;

    //ジャンプ判定
    string jump;

    //近接攻撃のfab入れ
    [SerializeField] GameObject MeleeattackPfab;

    //近接ダメージ
    public int Meleedamage;


    void Start()
    {
        playerHp = playerMaxHp;
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
            //向きを変える
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // Dキー
        if (Keyboard.current.dKey.isPressed)
        {
            move.x = 1;
            //向きを変える
            transform.localScale = new Vector3(1, 1, 1);
        }

        //近接攻撃

        //左クリック
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            meleeAttack = true;
        }

    }

    private void FixedUpdate()
    {
        // 横移動（ここを修正）
        rigid2D.linearVelocity = new Vector2(
            move.x * speed,
            rigid2D.linearVelocity.y
        );

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

        //近接攻撃
        if(meleeAttack)
        {
            //プレイヤーの向きを判定
            float direction = Mathf.Sign(transform.localScale.x);

            //プレイヤーの目の前の位置
            Vector3 spawnPos =transform.position +Vector3.right * direction * attackPosition;

            //出現させる
            Instantiate(MeleeattackPfab, spawnPos,Quaternion.identity);

            meleeAttack = false;
        }





    }

    // 当たり判定
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Groundタグならジャンプ回数をリセット
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
        }

        //エネミーにぶつかったらHPを減らす
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //HPが減る
            playerHp--;
            Debug.Log("ss");
        }

        //HPがなくなったら消える
        if (playerHp <= 0)
        {
            Destroy(gameObject);
        }


    }
}