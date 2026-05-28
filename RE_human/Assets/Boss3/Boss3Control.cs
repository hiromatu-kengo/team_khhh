using UnityEngine;

public class Boss3Control : MonoBehaviour
{
    public Transform playerTransform;
    private Boss3RangeAttack RangeAttack;
    public float moveSpeed = 2.0f;
    public float rangeAttackRange = 10.0f;

    [Header("この距離より近づかれたら逃げる")]
    public float escapeRange = 5.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RangeAttack = GetComponent<Boss3RangeAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        // プレイヤーがセットされていない場合は処理しない（エラー防止）
        if (playerTransform == null) return;

        // 1. 常にプレイヤーの方を向く
        LookAtPlayer();

        // 2. プレイヤーとの距離を計算
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // 3. 距離に応じた行動をとる
        if (distance < escapeRange)
        {
            // 近づきすぎなので離れる
            EscapeFromPlayer();
        }
        else if (distance <= rangeAttackRange)
        {
            // 遠距離攻撃の適正距離にいる場合の処理（ここに攻撃命令などを入れる）
            // 例: RangeAttack.ExecuteAttack(); など
        }
    }

    void LookAtPlayer()
    {
        if (playerTransform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void EscapeFromPlayer()
    {
        // プレイヤーとは「逆」の方向（X軸）を計算
        // プレイヤーが右(プラス)にいたら、ボスは左(マイナス)に移動する
        float directionX = transform.position.x - playerTransform.position.x;

        // 符号（プラスかマイナスか）だけを取り出す（1 または -1）
        float moveDir = Mathf.Sign(directionX);

        // ボスを移動させる（横移動のみを想定）
        transform.position += new Vector3(moveDir * moveSpeed * Time.deltaTime, 0, 0);
    }
}
