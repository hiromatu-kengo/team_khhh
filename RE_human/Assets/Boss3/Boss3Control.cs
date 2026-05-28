using UnityEngine;

public class Boss3Control : MonoBehaviour
{
    public Transform playerTransform;
    private Boss3RangeAttack RangeAttack;
    public float moveSpeed = 2.0f;
    public float rangeAttackRange = 10.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RangeAttack = GetComponent<Boss3RangeAttack>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void LookAtPlayer()
    {
        if (playerTransform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}

