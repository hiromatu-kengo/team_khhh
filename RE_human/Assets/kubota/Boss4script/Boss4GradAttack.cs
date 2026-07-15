using UnityEngine;
using System.Collections;

public class Boss4GrabAttack : MonoBehaviour
{
    [HideInInspector] public bool isAttacking = false; // Controllerが移動を止めるために監視するフラグ

    [Header("--- 闇のエフェクト ---")]
    public GameObject darkEffectPrefab; // 出現させる闇のプレハブ
    public Transform effectSpawnPoint;  // 闇を出現させる位置（ボスの手元や前方の目印）
    public float grabDuration = 2.0f;    // 闇が出現している（手を挙げたままにする）時間

    [Header("--- クールタイムの設定 ---")]
    public float attackCooldown = 7.0f;
    private float nextAttackTime = 0f;

    private Animator anim;
    private GameObject currentEffect; // 生成されたエフェクトの保持用

    void Start()
    {
        anim = GetComponent<Animator>();
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
        anim.Play("Spell");
    }

    // ★Animation Eventから自動的に呼び出される関数
    public void OnGrabHandRaised()
    {
        // 1. ボスのアニメーションの再生速度を 0 にして一時停止（手を挙げたままにする）
        anim.speed = 0f;

        // 2. 闇のエフェクトを生成
        if (darkEffectPrefab != null && effectSpawnPoint != null)
        {
            currentEffect = Instantiate(darkEffectPrefab, effectSpawnPoint.position, effectSpawnPoint.rotation);

            // エフェクトの向きをボスの向き（localScale）に合わせる
            Vector3 effectScale = currentEffect.transform.localScale;
            effectScale.x *= transform.localScale.x;
            currentEffect.transform.localScale = effectScale;
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
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }
}