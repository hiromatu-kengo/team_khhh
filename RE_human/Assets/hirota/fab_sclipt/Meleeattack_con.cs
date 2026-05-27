using UnityEngine;

public class Meleeattackcon : MonoBehaviour
{
    //攻撃時間
    [SerializeField] float span = 0.2f;

    [SerializeField] float delta = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        if(this.delta > this.span)
        {
            Destroy(gameObject);
        }
    }
    private void FixedUpdate()
    {
        this.delta += Time.deltaTime;

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        //警告を止めてる
        // enemyに当たった時攻撃をダメージを入れる
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("");
        }
    }
}
