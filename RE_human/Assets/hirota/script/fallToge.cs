using UnityEngine;

public class fallToge : MonoBehaviour
{
    //とげのrigidbodyを取得
    [SerializeField] private Rigidbody2D togeRigidbody;
 
    private bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true; //2回反応しないよう

            togeRigidbody.gravityScale = 3f;
            togeRigidbody.constraints = RigidbodyConstraints2D.None;
            togeRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        }
    }



}
