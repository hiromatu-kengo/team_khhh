using UnityEngine;

// ボスの移動と攻撃を管理するクラス
public class Boss4AI : MonoBehaviour
{
    // プレイヤー
    public Transform player;

    // 移動速度
    public float speed = 3f;

    // プレイヤーを見つける距離
    public float detectDistance = 40f;

    // 攻撃距離
    public float meleeDistance = 5f;
    public float rangeminDistance = 20f;
    public float rangeDistance = 30f;

    // 攻撃クールタイム
    public float attackCoolTime = 5f;

    // 攻撃中停止時間
    public float attackStopTime = 1f;

    // 移動範囲制限
    public float moveLimit = 50f;

    [Header("ステージの中心座標")]
    public Vector2 stageCenter = Vector2.zero;

    // クールタイム
    private float attackTimer;

    // 攻撃停止時間
    private float stopTimer;

    // ランダム移動方向
    private Vector2 moveDirection;　　　　　　　　　  // Vector2は2Dの位置や方向を表す型

    private float moveTimer;

    void Start()
    { 

        RandomMove();
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;　　　　　　　　// アタックタイマーを毎フレーム少しずつ減らす
        stopTimer -= Time.deltaTime;                  // 停止時間の残りを毎フレーム減らす

        // プレイヤーとの距離
        float distance =Vector2.Distance( transform.position,player.position);

        // 攻撃中停止
        if (stopTimer > 0)
        {
            return;
        }

        // クールタイム中
        if (attackTimer > 0)
        {
            FollowPlayer(distance);// distance を渡してプレイヤーを追跡する関数
            Debug.Log("追跡");
            return;
        }

        // 近距離攻撃
        if (distance <= meleeDistance)
        {
            MeleeAttack();

            attackTimer = attackCoolTime;            // 攻撃タイマーをクールタイムの値に戻す
            stopTimer = attackStopTime;              // 停止時間をクールタイムの値に戻す
        }

        // 遠距離攻撃
        else if (distance <= rangeminDistance && rangeminDistance <= rangeDistance)          // 実際の距離 が 判定する範囲の距離 以下か？
        {
            RangeAttack();

            attackTimer = attackCoolTime;
            stopTimer = attackStopTime;
        }

        // プレイヤー発見
        else if (distance <= detectDistance)
        {
            FollowPlayer(distance);
            Debug.Log("プレイヤー発見");
        }

        // 見つからない
        else
        {
            RandomMoveMove();
            Debug.Log("見つからない");
        }
    }

    // プレイヤー追跡
    void FollowPlayer(float distance)
    {
        // プレイヤー方向を計算
        Vector2 direction =
        (player.position -
        transform.position).normalized;              // normalizedはベクトルの向きをそのままにして、長さを 1 にする

        Vector2 nextPosition =
        (Vector2)transform.position +   　　　　　　 // 今いる位置
        direction *　　　　　　　　　　              // 移動する向き
        speed *
        Time.deltaTime;                              // 1フレームの時間

        // 初期位置から距離確認
        float distanceFromStart =　　　　　　　　　  //  distanceFromStar = 開始位置からの距離
        Vector2.Distance(
        stageCenter,
        nextPosition);

        // 制限範囲内だけ移動
        if (distanceFromStart <= moveLimit)
        {
            transform.position =
            nextPosition;
        }
    }

    // ランダム移動
    void RandomMoveMove()
    {
        transform.Translate(
            moveDirection *
            speed *
            Time.deltaTime);

        moveTimer -= Time.deltaTime;

        if (moveTimer <= 0)
        {
            RandomMove();
        }
    }

    // 左右ランダム
    void RandomMove()
    {
        int rand = Random.Range(0, 2);

        if (rand == 0)
        {
            moveDirection = Vector2.left;
        }
        else
        {
            moveDirection = Vector2.right;
        }

        moveTimer = 2f;
    }

    // 近距離攻撃
    void MeleeAttack()
    {
        Debug.Log("近距離攻撃");
    }

    // 遠距離攻撃
    void RangeAttack()
    {
        Debug.Log("遠距離攻撃");
    }
}