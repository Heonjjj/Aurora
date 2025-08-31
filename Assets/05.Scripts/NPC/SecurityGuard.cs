using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// AI 상태를 구별하기 위한 enum 선언
public enum AIState
{
    Idle,
    Wandering,
    Attacking
}

public class NPC : MonoBehaviour
{
    [Header("Stats")]
    public float walkSpeed;
    public float runSpeed;
    public float detectionRadius = 1.0f; // 플레이어가 NavMesh에서 벗어났는지 감지할 반경
    public int attackPower = 1;

    [Header("AI")]
    private NavMeshAgent agent;
    public float detectDistance;
    private AIState aiState;

    // Wandering 상태에 필요한 정보
    // min-max 사이의 대기 시간마다 min-max 사이의 거리에 있는 
    // 랜덤한 곳까지 돌아다니는 기능을 만들 때 필요한 정보들
    [Header("Wandering")]
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;

    // Attacking 상태에 필요한 정보들
    [Header("Combat")]
    public int damage;
    public float attackRate;
    private float lastAttackTime;
    public float attackDistance;
    public bool aggro = true;

    private float playerDistance;     // player와의 거리를 담아 둘 변수

    public float fieldOfView = 120f;

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private int walkableAreaMask;
    PlayerVM player;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        var audio = AudioManager.Instance;
    }

    private void Start()
    {
        // 최초 상태는 Wandering으로 설정
        SetState(AIState.Wandering);

        int walkableAreaIndex = 0;
        walkableAreaMask = 1 << walkableAreaIndex;
    }


    private void Update()
    {
        // player와의 거리를 매 프레임마다 계산
        playerDistance = Vector3.Distance(transform.position, Player.Instance.transform.position);

        animator.SetBool("IsMove", aiState != AIState.Idle);

        switch (aiState)
        {
            case AIState.Idle:
                PassiveUpdate();
                break;
            case AIState.Wandering:
                PassiveUpdate();
                break;
            case AIState.Attacking:
                AttackingUpdate();
                break;
        }
    }

    // 상태에 따른 agent의 이동속도, 정지여부를 설정
    private void SetState(AIState state)
    {
        aiState = state;

        switch (aiState)
        {
            case AIState.Idle:
                agent.speed = walkSpeed;
                agent.isStopped = true;
                break;
            case AIState.Wandering:
                agent.speed = walkSpeed;
                agent.isStopped = false;

                // AudioManager의 bgmGame2 변수에 할당된 오디오 클립을 가져와 재생
                AudioManager.Instance.bgmSource.clip = AudioManager.Instance.bgmGame;
                // 소리 재생
                AudioManager.Instance.bgmSource.Play();

                break;
            case AIState.Attacking:
                agent.speed = runSpeed;
                agent.isStopped = false;

                // AudioManager의 bgmGame2 변수에 할당된 오디오 클립을 가져와 재생
                AudioManager.Instance.bgmSource.clip = AudioManager.Instance.bgmGame3;
                // 소리 재생
                AudioManager.Instance.bgmSource.Play();

                break;
        }

        // 기본 값(walkSpeed)에 대한 비율로 재설정
        animator.speed = agent.speed / walkSpeed;
    }

    void PassiveUpdate()
    {
        NavMeshHit hit;

        // 플레이어 위치를 샘플링하여 유효한 NavMesh 지점을 찾기
        bool isPlayerOnNavMesh = NavMesh.SamplePosition(Player.Instance.transform.position, out hit, detectionRadius, NavMesh.AllAreas);

        // Wandering 상태이고, 목표한 지점에 거의 다 왔을 때
        if (aiState == AIState.Wandering && agent.remainingDistance < 0.1f)
        {
            SetState(AIState.Idle);
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
        }

        // 플레이어와의 거리가 감지 범위 안에 있을 때
        if (aggro == true && playerDistance < detectDistance && hit.mask == walkableAreaMask)
        {
            SetState(AIState.Attacking);

            
        }
    }

    // 새로운 Wander 목표지점 찾기
    void WanderToNewLocation()
    {
        if (aiState != AIState.Idle) return;

        SetState(AIState.Wandering);
        agent.SetDestination(GetWanderLocation());
    }

    Vector3 GetWanderLocation()
    {
        NavMeshHit hit;

        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);

        // 원하는 값이 안나왔을 때
        int i = 0;
        while (Vector3.Distance(transform.position, hit.position) < detectDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 5) break;
        }

        return hit.position;
    }

    void AttackingUpdate()
    {
        // 플레이어와의 거리가 공격범위 안에 있고,
        // 시야각 안에 있고,
        if (playerDistance < attackDistance
            && IsPlayerInFieldOfView())
        {
            agent.isStopped = true;
            if (Time.time - lastAttackTime > attackRate)
            {
                lastAttackTime = Time.time;
                animator.speed = 2;

                player.TakeDamage(attackPower);
                animator.SetTrigger("IsAttack");
                GameManager.Instance.GameOver();
            }
        }
        else
        {
            // 공격범위 안에는 없지만 감지범위 안에는 있을 때
            if (playerDistance < detectDistance)
            {
                

                agent.isStopped = false;
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(Player.Instance.transform.position, path))
                {
                    agent.SetDestination(Player.Instance.transform.position);
                }
                else
                {
                    agent.SetDestination(transform.position);
                    agent.isStopped = true;
                    SetState(AIState.Wandering);
                }
            }
            // 감지범위 밖으로 나갔을 때
            else
            {
                

                agent.SetDestination(transform.position);
                agent.isStopped = true;
                SetState(AIState.Wandering);
            }
        }
    }

    bool IsPlayerInFieldOfView()
    {
        // 뱡향 구하기(타겟 -내 위치) -- ⓐ
        Vector3 directionToPlayer = Player.Instance.transform.position - transform.position;
        // 내 정면 방향과 ⓐ 사이의 각도 구하기
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        // 설정한 시야각의 1 / 2 보다 작다면 시야각 안에 있는 것.
        // 시야각(ex.120도) = 내 정면 방향으로 좌우로 60도씩
        return angle < fieldOfView * 0.5f;
    }
}