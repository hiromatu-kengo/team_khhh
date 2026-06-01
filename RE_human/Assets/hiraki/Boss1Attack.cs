using UnityEngine;

public class Boss1Attack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("プレイヤーに攻撃が当たった！");
        }
    }
}