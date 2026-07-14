using UnityEngine;
using System.Collections;

public class GuardEffect : MonoBehaviour
{
    [Header("--- 視覚エフェクト ---")]
    [Tooltip("ガードした瞬間に表示するエフェクトのPrefab（光や火花など）")]
    public GameObject guardPrefab;
    [Tooltip("エフェクトを出す位置（未指定なら自分の位置）")]
    public Transform effectPosition;

    [Header("--- キャラクターのフラッシュ（2D用） ---")]
    [Tooltip("キャラクターのSpriteRenderer（3Dの場合はMaterialを変更します）")]
    public SpriteRenderer characterSprite;
    [Tooltip("ガードした瞬間の色（黄色や白がおすすめ）")]
    public Color flashColor = Color.yellow;
    [Tooltip("光っている時間（秒）")]
    public float flashDuration = 0.1f;

    [Header("--- サウンド ---")]
    [Tooltip("ガードしたときの効果音（キィン！などの金属音）")]
    public AudioClip guardSFX;
    private AudioSource audioSource;

    private Color originalColor;

    void Start()
    {
        // 必要なコンポーネントの自動取得
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && guardSFX != null)
        {
            // AudioSourceがなければ自動で追加
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (characterSprite != null)
        {
            originalColor = characterSprite.color;
        }
    }

    /// <summary>
    /// ガードが成功した瞬間に、他のスクリプトからこの関数を呼んでください！
    /// </summary>
    public void PlayGuardEffect()
    {
        // 1. ガードエフェクトの生成
        if (guardPrefab != null)
        {
            Vector3 spawnPos = effectPosition != null ? effectPosition.position : transform.position;
            // エフェクトを生成して、2秒後に自動消滅させる
            GameObject effect = Instantiate(guardPrefab, spawnPos, Quaternion.identity);
            Destroy(effect, 2.0f);
        }

        // 2. キャラクターの一瞬のフラッシュ（コルーチンを開始）
        if (characterSprite != null)
        {
            StartCoroutine(FlashCoroutine());
        }

        // 3. ガード音を鳴らす
        if (audioSource != null && guardSFX != null)
        {
            audioSource.PlayOneShot(guardSFX);
        }

        // 4. カメラシェイク（もしカメラを揺らす仕組みがあればここで呼ぶ）
        // CameraShake.Instance.Shake(0.1f, 0.2f); 
    }

    // キャラクターを一瞬だけ光らせて元に戻す処理
    private IEnumerator FlashCoroutine()
    {
        characterSprite.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        characterSprite.color = originalColor;
    }
}