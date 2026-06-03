using UnityEngine;
using UnityEngine.InputSystem;

public class player_con : MonoBehaviour
{
    [SerializeField] private heartManager heartManager;

    Rigidbody2D rigid2D;

    // 1回目ジャンプ力
    
    [SerializeField] float firstJumpForce = 800.0f;

    // 2回目ジャンプ力
    [SerializeField] float secondJumpForce = 600.0f;

    // 移動速度
    [SerializeField] float speed = 10f;

    // ジャンプ回数
    int jumpCount = 0;

    // 最大ジャンプ回数
    [SerializeField] int maxJump = 2;

    // プレイヤーのHP
    int playerHp;

    //プレイヤーのMaxHP
    [SerializeField] int playerMaxHp = 5;

    //攻撃判定
    bool meleeAttack = false;

    //攻撃位置
    [SerializeField] float attackPosition = 2.0f;

    Vector2 move;

    //ジャンプ判定
    string jump;

    //近接攻撃のfab入れ
    [SerializeField] GameObject MeleeattackPfab;

    // 他スクリプトからアクセスダッシュ中かどうか
    [HideInInspector] public bool isDashing = false;

    public bool kirikae = false;

    void Start()
    {
        playerHp = playerMaxHp;
        this.rigid2D = GetComponent<Rigidbody2D>();

        Cursor.visible = true;

        Cursor.lockState = CursorLockMode.None;
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

        if (!isDashing)
        {
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
        }
        //近接攻撃
        if (kirikae)
        {
            //左クリック
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                meleeAttack = true;
            }
        }

        if(Keyboard.current.eKey.wasPressedThisFrame)
        {
            kirikae = true;

            Cursor.visible = false;
        }
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            kirikae = false;
            Cursor.visible = true;
        }

    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            // 横移動
            rigid2D.linearVelocity = new Vector2(move.x * speed, rigid2D.linearVelocity.y);
        }
        // 1段ジャンプ
        if (jump == "ikkai")
        {
            rigid2D.linearVelocity = new Vector2(rigid2D.linearVelocity.x, 0);

            rigid2D.AddForce(Vector2.up * firstJumpForce);

            jump = "";
        }

        // 2段ジャンプ
        if (jump == "nikai")
        {
            rigid2D.linearVelocity = new Vector2(rigid2D.linearVelocity.x, 0);

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
            GameObject meleeEffect = Instantiate(MeleeattackPfab, spawnPos, Quaternion.identity);
            meleeEffect.transform.SetParent(this.transform);

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
            Debug.Log("p1ダメ");
            heartManager.UpdateHearts(playerHp);
        }
        //つおい攻撃をあたった
        if (collision.gameObject.CompareTag("BossAttack"))
        {
            //HPが減る
            playerHp -= 2;
            Debug.Log("p2ダメ");
            heartManager.UpdateHearts(playerHp);
        }

        //HPがなくなったら消える
        if (playerHp <= 0)
        {
            Destroy(gameObject);
        }
    }
        private void OnTriggerEnter2D(Collider2D collision)
    {
        // すり抜ける敵の弾（タグがEnemyの場合）に当たったとき
        if (collision.CompareTag("Enemy"))
        {
            playerHp--;
            Debug.Log("p1ダメ（弾）");
            heartManager.UpdateHearts(playerHp);
        }

        if (playerHp <= 0)
        {
            Destroy(gameObject);
        }
    }




}
