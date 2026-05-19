/*using UnityEngine;

public class BossMove : MonoBehaviour
{
    public float moveSpeed = 3f;

    private Vector2 moveDirection;

    private float moveTimer;

    void Start()
    {
        ChooseDirection();
    }

    void Update()
    {
        RandomMove();
    }

    void RandomMove()
    {
        moveTimer -= Time.deltaTime;

        if (moveTimer <= 0)
        {
            ChooseDirection();
        }

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    void ChooseDirection()
    {
        int rand = Random.Range(0, 2);

        Debug.Log(rand);

        if (rand == 0)
        {
            moveDirection = Vector2.left;
        }
        else
        {
            moveDirection = Vector2.right;
        }

        moveTimer = Random.Range(1f, 3f);
    }
}
*/