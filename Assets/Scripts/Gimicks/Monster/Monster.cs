using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    [Header("Monster Status")]
    [SerializeField] private bool isDead = false;
    [SerializeField] private float maxHp = 20;
    [SerializeField] private float _hp;
    public float hp
    {
        get => _hp;
        private set => _hp = value;
    }
    [SerializeField] private int _attackPower;
    public int attackPower
    {
        get => _attackPower;
        set => _attackPower = value;
    }

    [Header("HP Bar")]
    [SerializeField] private RectTransform hpBar;// 체력이 최대일때는 Width가 2.

    [Header("Monster AI")]
    [SerializeField] private Transform target;
    [SerializeField] private float lostDistance = 15; // 타겟을 잃어버리는 거리.
    NavMeshAgent navMeshAgent;
    Animator anim;
    State state = State.Idle;
    enum State
    {
        Idle,
        Chase,
        Attack,
    }// 몬스터 상태
    public Func<bool> InRange; 


    private void OnEnable()
    {
        isDead = false;
        _hp = maxHp;
    }
    private void Start()
    {
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        target = GameManager.Instance.player.transform;
        StartCoroutine(StateMachine());// 몬스터 상태 머신 시작.
    }
    private void Update()
    {
        if (hp <= 0)
        {
            isDead = true;
            gameObject.SetActive(false);
        }
        if(hpBar != null)
        {
            hpBar.sizeDelta = new Vector2(hp/maxHp * 2, hpBar.sizeDelta.y);
        }
        // 몬스터 AI
        if (isDead) return;
        if(InRange() && target == null)// 범위 내에 플레이어가 있고, 타겟이 없을 때
        {
            target = GameManager.Instance.player.transform;
            ChangeState(State.Chase);
        }
        else if(!InRange())
        {
            target = null;
            ChangeState(State.Idle);
        }
        if(target == null) return; // 타겟이 없거나, 범위 밖일때는 return.
        navMeshAgent.SetDestination(target.position);
    }
    private void ChangeState(State newState)
    {
        state = newState;
    }
    private void OnTriggerEnter(Collider other)// 피격 판정.
    {
        if (other.CompareTag("AttackBox"))// 공격의 판정박스에 닿았을 때.
        {
            Player player = other.GetComponent<PlayerAttackBox>().player;
            if (player == null || player.attackCount <= 0) return; // 플레이어의 공격이 아니거나 공격 판정 잔여 횟수가 0일때는 return.
            hp -= player.GetDamage();
            player.attackCount--;
           
        }
    }

    IEnumerator StateMachine()
    {
        while (hp > 0)
            yield return StartCoroutine(state.ToString());
    }
    IEnumerator Idle()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // 애니메이션 이름이 Idle이 아니면 Play
        if (curAnimStateInfo.IsName("Idle") == false)
            anim.Play("Idle", 0, 0);

        //재생 시간 동안 대기.
        for (float i = 0; i < curAnimStateInfo.length; i += Time.deltaTime)
        {
            yield return null;
        }
    }

    IEnumerator Chase()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (curAnimStateInfo.IsName("Chase") == false)
        {
            anim.Play("Chase", 0, 0);
            yield return null;// SetDestination 을 위해 frame.
        }

        // 목표까지의 남은 거리가 멈추는 지점보다 작거나 같으면 StateMachine 을 공격으로 변경
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if(navMeshAgent.remainingDistance <= 10000f && navMeshAgent.remainingDistance >= 0.1f)// 벽을 사이에 두면 거리가 +-infinity로 되서 한번더 예외처리.
            {
                ChangeState(State.Attack);
                yield break;
            }
        }
        else if (navMeshAgent.remainingDistance > lostDistance)// 목표와의 거리가 멀어진 경우
        {
            target = null;
            navMeshAgent.SetDestination(transform.position);
            yield return null;
            if(!InRange())
                ChangeState(State.Idle);// 범위 밖까지 멀어지면 idle로 변경
        }
        else
        {
            yield return new WaitForSeconds(curAnimStateInfo.length);// 애니메이션의 한 사이클 동안 대기
        }
    }

    IEnumerator Attack()
    {
        Debug.Log("Attack");
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // 공격 애니메이션은 공격 후 Idle Battle 로 이동하기 때문에 
        // 코드가 이 지점에 오면 무조건 Attack01 을 Play
        anim.Play("Attack", 0, 0);
        if (target != null) target.GetComponent<Player>().TakeDamage(attackPower);// 공격력만큼 플레이어에게 데미지를 줌.
        // 거리가 멀어지면
        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            // StateMachine을 추적으로 변경
            ChangeState(State.Chase);
        }
        else
            yield return new WaitForSeconds(curAnimStateInfo.length * 2f);// 공격 animation 의 두 배만큼 대기
    }
}
