using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class player_dassyu : MonoBehaviour
{
    player_con playerCon;
    Rigidbody2D rigid2D;


    [SerializeField] float dashSpeed = 50f;     //ダッシュ速度
    [SerializeField] float dashDuration = 0.15f;
    [SerializeField] float sutaminaMax = 9f;    //maxスタミナ
    [SerializeField] float syouhi = 3f;         //消費スタミナ
    [SerializeField] float sutaminaSpan = 1f;   //スタミナ回復スパン
    [SerializeField] staminaMsnager sutaminaUI;  //紐づけ
    [SerializeField] private GameObject sutaminaUiDisplay;
    float sutamina;
    float delta = 0;
    float dashTimer;
    //ダッシュ判定
    bool dassyu= false;

    public AudioSource audioSource;
    public AudioClip dassySound;
    private void PlaySE(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sutamina = sutaminaMax;
        this.rigid2D = GetComponent<Rigidbody2D>();
        playerCon = GetComponent<player_con>();
        //最大スタミナを伝える
        if(sutaminaUI  != null )
        {
            sutaminaUI.SetMaxStamina(sutaminaMax);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0f) return;
        //ダッシュ判定
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            dassyu = true;
        }
    }

    private void FixedUpdate()
    {
        if(sutamina ==sutaminaMax)
        {
            sutaminaUiDisplay.SetActive(false);
        }
        else
        {
            sutaminaUiDisplay.SetActive(true);

        }


        if (Time.timeScale == 0f) return;
        //向きの判定
        float direction = Mathf.Sign(transform.localScale.x);

        this.delta += Time.deltaTime;
        if ((this.delta > this.sutaminaSpan) && sutamina < sutaminaMax)
        {
            this.delta = 0;
            sutamina += 1;
            //現在のスタミナを教える
            if(sutaminaUI != null)
            {
                sutaminaUI.UpdateStamina(sutamina);
            }
        }

        if (dassyu)
        {
            if (!playerCon.isDashing && sutamina >= syouhi)
            {
                PlaySE(dassySound);


                playerCon.isDashing = true; // メインにダッシュしたと伝える
                dashTimer = dashDuration;
                sutamina -= syouhi;

                //現在のスタミナを教える
                if (sutaminaUI != null)
                {
                    sutaminaUI.UpdateStamina(sutamina);
                }
            }
            dassyu = false;
        }
        if (playerCon.isDashing)
        {
         
            // メイン側の通常移動を上書き速度を入れ込む
            rigid2D.linearVelocity = new Vector2(direction * dashSpeed, rigid2D.linearVelocity.y);

            dashTimer -= Time.fixedDeltaTime;

            if (dashTimer <= 0f)
            {
                playerCon.isDashing = false; // 時間を過ぎたらダッシュ状態を解除
            }
        }
    }
}
