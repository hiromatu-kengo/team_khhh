using UnityEngine;

public class EnemyMoveAndAttack : MonoBehaviour
{
    public float moveSpeed = 30f;          // 移動速度
    public float moveTime = 2f;           // 動く時間
    public float stopTime = 1.5f;         // 止まる時間（攻撃時間）

    private Vector2 moveDirection;
    private float timer;
    private bool isStopped = false;

    void Start()
    {
        
        SetRandomDirection(); // 最初の方向
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (isStopped)
        {
            // 止まって攻撃中
            if (timer >= stopTime)
            {
                isStopped = false;
                timer = 0f;
                SetRandomDirection(); // 次の方向決定
            }
        }
        else
        {
            // 移動中
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

            if (timer >= moveTime)
            {
                isStopped = true;
                timer = 0f;
                Attack();
            }
        }
    }

    void SetRandomDirection()
    {
        int rand = Random.Range(0, 0);

        if (rand == 0)
            moveDirection = Vector2.right;
        else
            moveDirection = Vector2.left;
    }

    void Attack()
    {
        Debug.Log("攻撃！");
        // ここに弾発射や近接攻撃を書く
    }
}