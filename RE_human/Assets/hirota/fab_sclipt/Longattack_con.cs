using UnityEngine;
public class player_ : MonoBehaviour
{
    Rigidbody2D rigid2D;

    Vector2 move;

    public float longSpeed = 15f;

    private Vector3 mouseVector;

    public Transform playerObj;

    public float maxDistance = 20f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");

        playerObj = player.transform;

        rigid2D = GetComponent<Rigidbody2D>();
        //マウスの座標を取得
        Vector3 mouseScreenPos = Input.mousePosition;
        //カメラから見たマウスの位置に変換
        Vector3 mouseWorldPos2D = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos2D.z = 0f;
        //計算して正規化
        mouseVector = (mouseWorldPos2D - transform.position).normalized;
      
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        rigid2D.linearVelocity = mouseVector * longSpeed;

        Vector3 targetPos = playerObj.position;
        Vector3 attackPos = transform.position;

        //距離を計算
        float currentDistance = Vector3.Distance(targetPos, attackPos);

        if (currentDistance > maxDistance)
        {
            Destroy(gameObject); // 弾を消す
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("aa");

        if (!collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }

    }
}
