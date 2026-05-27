using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    private enum BossState
    {
        Move,
        Charge,
        Rush,
        Stun
    }

    [Header("追尾設定")]
    [SerializeField] private float moveSpeed = 2f;//ボスの移動速度

    [Header("突進設定")]
    [SerializeField] private float detectRange = 5f;//プレイヤーの感知距離
    [SerializeField] private float chargeTime = 2f;//突進ため時間
    [SerializeField] private float rushSpeed = 10f;//突進速度
    [SerializeField] private float rushTime = 1f;//突進時間

    [Header("硬直設定")]
    [SerializeField] private float stunTime = 2f;//硬直時間

    [Header("クールダウン")]
    [SerializeField] private float rushCooldown = 10f;//突進クールタイム

    [Header("参照")]
    [SerializeField] private Transform player;//プレイヤーの位置取得

    private Rigidbody2D rb;

    private BossState currentState;

    private bool canRush = true;//突進可能かどうか　trueの場合は突進可能

    private Vector2 rushDirection;

    [SerializeField] private int bossHp;//ボスHP

    private void Start()
    {
        bossHp = 1000;

        rb = GetComponent<Rigidbody2D>();

        currentState = BossState.Move;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    //判定・入力・状態確認の処理
    private void Update()
    {
        if (player == null)
        {
            return;
        }

        Flip();

        float distance = Vector2.Distance(transform.position, player.position);

        if (currentState == BossState.Move &&
            distance <= detectRange &&
            canRush)
        {
            StartCoroutine(RushRoutine());
        }
    }


    //物理処理
    private void FixedUpdate()
    {
        switch (currentState)
        {
            case BossState.Move://Move状態
                MoveToPlayer();
                break;

            case BossState.Rush://Rush状態
                RushMove();
                break;
        }
    }

    private void Flip()
    {
        if (player == null)
        {
            return;
        }

        // プレイヤーが右側
        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        // プレイヤーが左側
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void MoveToPlayer()//プレイヤー追尾
    {
        Vector2 direction =
            (player.position - transform.position).normalized;

        rb.linearVelocity = direction * moveSpeed;//移動速度決定
    }

    private void RushMove()//突進移動
    {
        rb.linearVelocity = rushDirection * rushSpeed;
    }

    private IEnumerator RushRoutine()
    {
        canRush = false;//突進不可

        //ため状態
        currentState = BossState.Charge;

        rb.linearVelocity = Vector2.zero;//止まる

        //突進方向を固定
        rushDirection =
            (player.position - transform.position).normalized;

        yield return new WaitForSeconds(chargeTime);//２秒待機

        // 突進
        currentState = BossState.Rush;

        yield return new WaitForSeconds(rushTime);//突進時間

        // 硬直
        currentState = BossState.Stun;

        rb.linearVelocity = Vector2.zero;//止まる

        yield return new WaitForSeconds(stunTime);//硬直時間

        // 通常状態へ
        currentState = BossState.Move;

        //10秒間のクールダウン
        yield return new WaitForSeconds(rushCooldown);

        canRush = true;//再び突進可能
    }
}