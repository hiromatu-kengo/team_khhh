using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class kamera : MonoBehaviour
{
    //追いかける対象
    [SerializeField] private Transform target;

    //追従速度
    [SerializeField] private float smoothSpeed = 0.125f;

    //プレイヤーとカメラの位置
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);

    //左限界
    [SerializeField] private float minX = -10f;

    //右限界
    [SerializeField] private float maxX = 10f;

    //下限界
    [SerializeField] private float minY = -5f;

    //上限界
    [SerializeField] private float maxY = 5f;

    //制限をつけるか
    [SerializeField] private bool useLimits = true;

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null) return;

        //目標座標
        float targetx = target.position.x + offset.x;

        float targety = target.position.y + offset.y;

        if (useLimits)
        {
            targetx = Mathf.Clamp(targetx, minX, maxX);
            targety = Mathf.Clamp(targety, minY, maxY);
        }

        Vector3 targetPosition = new Vector3(targetx, targety, target.position.z + offset.z);

        //lerpを使って現在地から目標地に近づける
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

        //位置の更新
        transform.position = smoothedPosition;


    }

    private void OnDrawGizmosSelected()
    {
        if (useLimits)
        {
            Gizmos.color = Color.green; // 緑色の線で描画するばい

            // 四隅の角の座標を計算
            Vector3 topLeft = new Vector3(minX, maxY, 0);
            Vector3 topRight = new Vector3(maxX, maxY, 0);
            Vector3 bottomLeft = new Vector3(minX, minY, 0);
            Vector3 bottomRight = new Vector3(maxX, minY, 0);

            // 枠線を結ぶばい
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }

    }
}
