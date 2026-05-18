using UnityEngine;

public class Meleeattackcon : MonoBehaviour
{
    //攻撃時間
    float span = 0.2f;

    float delta = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        this.delta += Time.deltaTime;
        if(this.delta > this.span)
        {
            Destroy(gameObject);
        }
    }
}
