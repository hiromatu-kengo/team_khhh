using UnityEngine;

// ボスの移動と攻撃を管理するクラス
public class Boss4AI : MonoBehaviour
{
    // プレイヤー
    public Transform player;

    // 移動速度
    public float speed = 3f;

    // 攻撃距離
    public float meleeDistance = 2f;
    public float rangeDistance = 6f;

    // 攻撃クールタイム
    public float attackCoolTime = 2f;

    // 現在のクールタイム
    private float attackTimer = 2f;

    // 移動方向
    private Vector2 moveDirection;

    // 移動タイマー
    private float moveTimer;

    void Start()
    {
        RandomMove();
    }

    void Update()
    {
        // クールタイムを減らす
        attackTimer -= Time.deltaTime;

        // プレイヤーとの距離
        float distance =
            Vector2.Distance(
            transform.position,
            player.position);

        // クールタイム中
        if (attackTimer > 0)
        {
            // 攻撃せず移動
            Move();

            return;
        }

        // 近距離
        if (distance <= meleeDistance)
        {
            MeleeAttack();

            // クールタイム開始
            attackTimer = attackCoolTime;
        }

        // 遠距離
        else if (distance <= rangeDistance)
        {
            RangeAttack();

            attackTimer = attackCoolTime;
        }

        // プレイヤーが遠い
        else
        {
            Move();
        }
    }

    // 移動処理
    void Move()
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

    // 左右ランダム移動
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
