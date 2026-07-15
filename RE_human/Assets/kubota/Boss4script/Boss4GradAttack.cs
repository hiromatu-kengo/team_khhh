using UnityEngine;
using System.Collections;

public class Boss4GrabAttack : MonoBehaviour
{
    [HideInInspector] public bool isAttacking = false; // Controllerが移動を止めるために監視するフラグ

    [Header("--- 闇のエフェクト ---")]
    public GameObject darkEffectPrefab; // 出現させる闇（ワープと手）のプレハブ
    public Transform effectSpawnPoint;  // ボスの手元（ここから術を放つポーズの基準）
    public float grabDuration = 1.6f;    // 闇が出現している（手を挙げたままにする）時間

    [Header("--- プレイヤーの参照 ---")]
    public Transform playerTransform;   // プレイヤーの位置（インスペクターでプレイヤーをアタッチしてください）
    public Vector2 spawnOffset = new Vector2(0f, 3.0f); // プレイヤーのどれくらい上空にワープを出すか

    [Header("--- クールタイムの設定 ---")]
    public float attackCooldown = 7.0f;
    private float nextAttackTime = 0f;

    private Animator anim;
    private GameObject currentEffect; // 生成されたエフェクトの保持用

    void Start()
    {
        anim = GetComponent<Animator>();

        // もしインスペクターでプレイヤーが未登録の場合、自動で探す
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }
    }

    // Controllerから呼ばれる実行関数
    public bool CanAttack()
    {
        return !isAttacking && Time.time >= nextAttackTime;
    }

    public void Execute()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        // Spellアニメーションを再生
        anim.Play("Boss4GradAttack");
    }

    // ★Animation Eventから自動的に呼び出される関数（ここを大改造！）
    public void OnGrabHandRaised()
    {
        // 1. ボスのアニメーションの再生速度を 0 にして一時停止（手を挙げたままにする）
        anim.speed = 0f;

        // 2. 【改造！】プレイヤーの「頭上」にワープエフェクトを生成
        if (darkEffectPrefab != null && playerTransform != null)
        {
            // プレイヤーの現在位置 ＋ 設定した高さ（spawnOffset）の座標を計算
            Vector3 spawnPosition = playerTransform.position + (Vector3)spawnOffset;

            // 計算したプレイヤー頭上の位置に、ワーププレハブを召喚！
            currentEffect = Instantiate(darkEffectPrefab, spawnPosition, Quaternion.identity);

            // ※エフェクトの左右の向きをボスに合わせる処理（必要に応じて）
            Vector3 effectScale = currentEffect.transform.localScale;
            // プレイヤーに向きを合わせる場合はそのまま、ボスの向きに合わせる場合は以下を実行
            effectScale.x = Mathf.Abs(effectScale.x) * (transform.localScale.x < 0 ? -1f : 1f);
            currentEffect.transform.localScale = effectScale;
        }
        else if (darkEffectPrefab != null && effectSpawnPoint != null)
        {
            // もしプレイヤーが見つからなかった時のための安全装置（ボスの手元に出す）
            currentEffect = Instantiate(darkEffectPrefab, effectSpawnPoint.position, effectSpawnPoint.rotation);
        }

        // 3. 一定時間後に技を終了するコルーチンを開始
        StartCoroutine(GrabRoutine());
    }

    private IEnumerator GrabRoutine()
    {
        // 闇が出現している時間、ボスは手を挙げたまま待機
        yield return new WaitForSeconds(grabDuration);

        // 4. 闇のエフェクトを消去
        if (currentEffect != null)
        {
            Destroy(currentEffect);
        }

        // 5. アニメーションの再生速度を 1 に戻して、手を下ろすモーションを再開させる
        anim.speed = 1f;

        // アニメーションが完全に終わる（Idleに戻る）まで少し待ってからフラグを戻す
        anim.Play("Boss4Idle");
        isAttacking = false;
    }
}