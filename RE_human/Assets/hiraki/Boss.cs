using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("通常移動")]
    public float moveSpeed = 3f;

    public float minX = -5f;
    public float maxX = 5f;

    public float groundY = -3f;

    [Header("突進")]
    public float rushSpeed = 10f;
    public float rushInterval = 5f;
    public float rushDistance = 3f;

    private Vector2 targetPos;

    private Transform player;

    private bool isRushing = false;

    private float rushTimer = 0f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        SetNewTarget();
    }

    void Update()
    {
        rushTimer += Time.deltaTime;

        if (rushTimer >= rushInterval && !isRushing)
        {
            StartRush();
        }
    }

    void FixedUpdate()
    {
        float speed = isRushing ? rushSpeed : moveSpeed;

        rb.MovePosition(
            Vector2.MoveTowards(
                rb.position,
                targetPos,
                speed * Time.fixedDeltaTime
            )
        );

        if (Vector2.Distance(rb.position, targetPos) < 0.1f)
        {
            if (isRushing)
            {
                isRushing = false;
                rushTimer = 0f;
            }

            SetNewTarget();
        }
    }

    void SetNewTarget()
    {
        float randomX = Random.Range(minX, maxX);

        targetPos = new Vector2(randomX, groundY);
    }

    void StartRush()
    {
        isRushing = true;

        Vector2 direction =
            (player.position - transform.position).normalized;

        Vector2 rushTarget =
            (Vector2)transform.position + direction * rushDistance;

        float clampedX = Mathf.Clamp(rushTarget.x, minX, maxX);

        targetPos = new Vector2(clampedX, groundY);
    }
}