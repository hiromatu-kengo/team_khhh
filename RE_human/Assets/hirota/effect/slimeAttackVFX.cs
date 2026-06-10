using UnityEngine;

public class slimeAttackVFX : MonoBehaviour
{
    //エフェクトfab入れ
    [SerializeField] private GameObject waterSplashPrefab;

    //発生位置　プレイヤーの前
    [SerializeField] private Vector3 spawnOffset = new Vector3(1.0f, -0.2f, 0f);

    //消えるまでの時間
    [SerializeField] private float destroyDelay = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SpawnWaterSplash()
    {

        // スライムの向いている方向
        float direction = Mathf.Sign(transform.localScale.x);

        // 向きに合わせて発生位置を調整
        Vector3 actualOffset = new Vector3(spawnOffset.x * direction, spawnOffset.y, spawnOffset.z);
        Vector3 spawnPosition = transform.position + actualOffset;

        // エフェクトを生成
        GameObject splash = Instantiate(waterSplashPrefab, spawnPosition, Quaternion.identity);

        // エフェクトの向きもスライムに合わせる
        Vector3 splashScale = splash.transform.localScale;
        splashScale.x *= direction;
        splash.transform.localScale = splashScale;

        // 指定時間後に自動消滅
        Destroy(splash, destroyDelay);
    }

  
}
