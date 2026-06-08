using UnityEngine;
using UnityEngine.InputSystem;

public class player_con : MonoBehaviour
{
    [SerializeField] private heartManager heartManager;

    Rigidbody2D rigid2D;

    //アニメーション追加
    Animator anim;

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

    //プレイヤーの死亡判定
    private bool isDead = false;

    [SerializeField] private float attackMotionTime = 0.3f; // 攻撃アニメーション時間
    private float attackTimer = 0f; //時間のカウント
    //攻撃までのクールタイム
    [SerializeField] private float attackCoolTime = 0.5f;
    //クールタイムのカウント
    private float coolTimeTimer = 0f;

    private bool _kirikae = false;

    public bool kirikae
    {
        get { return _kirikae; }
        private set { _kirikae = value; }
    }

    void Start()
    {
        playerHp = playerMaxHp;
        this.rigid2D = GetComponent<Rigidbody2D>();

        Cursor.visible = true;

        Cursor.lockState = CursorLockMode.None;

        this.anim = GetComponent<Animator>();
    }

    void Update()
    {
        //攻撃タイマー
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        //近接クールタイム
        if (coolTimeTimer > 0)
        {
            coolTimeTimer -= Time.deltaTime;
        }


        //死んでいるなら処理しない
        if (isDead) return;
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
            //左クリック& クールタイムが終わっている
            if (Mouse.current.leftButton.wasPressedThisFrame && coolTimeTimer <= 0)
            {
                meleeAttack = true;
                attackTimer = attackMotionTime; // アニメーション維持時間
                coolTimeTimer = attackCoolTime; // クールタイムセット！
            }
        }

        //攻撃の切り替え
        if(Keyboard.current.eKey.wasPressedThisFrame)
        {
            kirikae = !kirikae;

            Cursor.visible = !kirikae;
        }

        //アニメーシ
        if (isDashing)
        {
            //ダッシュ中なら最優先ダッシュアニメ
            anim.Play("Dash");
        }
        else if (attackTimer > 0)
        {
            //攻撃タイマーが残っているなら最優先
            anim.Play("Attack");
        }
        else if (jumpCount > 0)
        {
            //空中にいるときジャンプ
            anim.Play("Jump");
        }
        else
        {
            // 地面にいるダッシュ中でない
            if (move.x != 0)
            {
                //移動入力ラン
                anim.Play("Run");
            }
            else
            {
                //移動していない待機
                anim.Play("Idle");
            }
        }
    }

    private void FixedUpdate()
    {

        //死んでいるなら物理移動、攻撃をしない
        if (isDead)
        {
            rigid2D.linearVelocity = new Vector2(0, rigid2D.linearVelocity.y);
            return;
        }

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
        if (meleeAttack)
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

    // HPが0時に呼び出す
    void Die()
    {
        if (isDead) return; // 二重に呼び出されるのを防ぐ

        isDead = true;

        // 死亡アニメーション
        anim.Play("Death");

        //プレイヤーを画面から完全に消去
        Invoke("DestroyPlayer", 1.5f);
    }

    //消滅
    void DestroyPlayer()
    {
        Destroy(gameObject);
       //ゲームオーバー画面

    }



    // 当たり判定
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.otherCollider.gameObject != this.gameObject) return;

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
            Die();
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
           Die();
        }
    }




}
