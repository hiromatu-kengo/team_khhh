using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public float speed = 3f;       // 移動速度
    public float changeTime = 2f; // 方向を変える時間

    private float timer;
    private int direction; // -1 = 左, 1 = 右

    void Start()
    {
        ChangeDirection();
    }

    void Update()
    {
        // 左右移動
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        // タイマー
        timer += Time.deltaTime;

        if (timer >= changeTime)
        {
            ChangeDirection();
            timer = 0f;
        }
    }

    void ChangeDirection()
    {
        int rand = Random.Range(0, 2);

        if (rand == 0)
        {
            direction = -1; // 左
        }
        else
        {
            direction = 1; // 右
        }
    }
}