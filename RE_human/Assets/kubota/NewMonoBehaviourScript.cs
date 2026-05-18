/*using UnityEngine;

public class BossStateMachine : MonoBehaviour
{
    public enum BossState
    {
        Idle,
        Move,
        MeleeAttack,
        RangeAttack,
        Guard,
        GrabAttack,
        Stunned,
        Dead
    }

    public BossState currentState;

    [Header("Player")]
    public Transform player;

    [Header("Distance")]
    public float meleeDistance = 2f;
    public float rangeDistance = 6f;
    public float grabDistance = 1.5f;

    private Bos4Move move;
    private Bos4Attack attack;
    private Boss4Guard guard;
    private bos4HP hp;

    private bool canAttack = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        move = GetComponent<Bos4Move>();
        attack = GetComponent<Bos4Attack>();
        guard = GetComponent<Boss4Guard>();
        hp = GetComponent<bos4HP>();

        currentState = BossState.Move;
    }

    void Update()
    {
        if (currentState == BossState.Dead)
            return;

        float distance = Vector2.Distance(transform.position, player.position);


        // HP0なら死亡
        if (hp.currentHP <= 0)
        {
            currentState = BossState.Dead;
            return;
        }


        if (canAttack == false)
        {
            currentState = BossState.Move;
            move.RandomMove();
        }

        // 硬直中
        if (currentState == BossState.Stunned)
            return;

        // つかみ
        if (distance <= grabDistance && Random.value < 0.01f)
        {
            currentState = BossState.GrabAttack;
            attack.GrabAttack();
            return;
        }

        // 近距離
        if (distance <= meleeDistance && canAttack == true)
        {
            currentState = BossState.MeleeAttack;
            attack.MeleeAttack();
        }

        // 遠距離
        else if (distance >= rangeDistance && canAttack == true)
        {
            currentState = BossState.RangeAttack;
            attack.RangeAttack();
        }


    }

    public void SetStunned(float time)
    {
        StartCoroutine(StunnedCoroutine(time));
    }

    System.Collections.IEnumerator StunnedCoroutine(float time)
    {
        currentState = BossState.Stunned;

        yield return new WaitForSeconds(time);

        currentState = BossState.Move;
    }
}
*/