using UnityEngine;

public class playerJumpVFX : MonoBehaviour
{
    //エフェクトfub
    [SerializeField] private GameObject jumpDustPrefab;

    //エフェクトの位置
    [SerializeField] private Vector3 footOffset = new Vector3(0f, -0.6f, 0f);

    //エフェクトが消える時間
    [SerializeField] private float destroyDelay = 0.8f;

    public void SpawnJumpDust()
    {

        // 足元の位置を計算
        Vector3 spawnPosition = transform.position + footOffset;

        // エフェクトを生成
        GameObject dust = Instantiate(jumpDustPrefab, spawnPosition, Quaternion.identity);

        // 指定時間後消去！
        Destroy(dust, destroyDelay);
    }

}
