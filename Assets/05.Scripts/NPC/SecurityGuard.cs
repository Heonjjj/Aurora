using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// AI ���¸� �����ϱ� ���� enum ����
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
    public float detectionRadius = 1.0f; // �÷��̾ NavMesh���� ������� ������ �ݰ�
    public int attackPower = 1;

    [Header("AI")]
    private NavMeshAgent agent;
    public float detectDistance;
    private AIState aiState;

    // Wandering ���¿� �ʿ��� ����
    // min-max ������ ��� �ð����� min-max ������ �Ÿ��� �ִ� 
    // ������ ������ ���ƴٴϴ� ����� ���� �� �ʿ��� ������
    [Header("Wandering")]
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;

    // Attacking ���¿� �ʿ��� ������
    [Header("Combat")]
    public int damage;
    public float attackRate;
    private float lastAttackTime;
    public float attackDistance;
    public bool aggro = true;

    private float playerDistance;     // player���� �Ÿ��� ��� �� ����

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
        // ���� ���´� Wandering���� ����
        SetState(AIState.Wandering);

        int walkableAreaIndex = 0;
        walkableAreaMask = 1 << walkableAreaIndex;
    }


    private void Update()
    {
        // player���� �Ÿ��� �� �����Ӹ��� ���
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

    // ���¿� ���� agent�� �̵��ӵ�, �������θ� ����
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

                // AudioManager�� bgmGame2 ������ �Ҵ�� ����� Ŭ���� ������ ���
                AudioManager.Instance.bgmSource.clip = AudioManager.Instance.bgmGame;
                // �Ҹ� ���
                AudioManager.Instance.bgmSource.Play();

                break;
            case AIState.Attacking:
                agent.speed = runSpeed;
                agent.isStopped = false;

                // AudioManager�� bgmGame2 ������ �Ҵ�� ����� Ŭ���� ������ ���
                AudioManager.Instance.bgmSource.clip = AudioManager.Instance.bgmGame3;
                // �Ҹ� ���
                AudioManager.Instance.bgmSource.Play();

                break;
        }

        // �⺻ ��(walkSpeed)�� ���� ������ �缳��
        animator.speed = agent.speed / walkSpeed;
    }

    void PassiveUpdate()
    {
        NavMeshHit hit;

        // �÷��̾� ��ġ�� ���ø��Ͽ� ��ȿ�� NavMesh ������ ã��
        bool isPlayerOnNavMesh = NavMesh.SamplePosition(Player.Instance.transform.position, out hit, detectionRadius, NavMesh.AllAreas);

        // Wandering �����̰�, ��ǥ�� ������ ���� �� ���� ��
        if (aiState == AIState.Wandering && agent.remainingDistance < 0.1f)
        {
            SetState(AIState.Idle);
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
        }

        // �÷��̾���� �Ÿ��� ���� ���� �ȿ� ���� ��
        if (aggro == true && playerDistance < detectDistance && hit.mask == walkableAreaMask)
        {
            SetState(AIState.Attacking);

            
        }
    }

    // ���ο� Wander ��ǥ���� ã��
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

        // ���ϴ� ���� �ȳ����� ��
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
        // �÷��̾���� �Ÿ��� ���ݹ��� �ȿ� �ְ�,
        // �þ߰� �ȿ� �ְ�,
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
            // ���ݹ��� �ȿ��� ������ �������� �ȿ��� ���� ��
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
            // �������� ������ ������ ��
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
        // ���� ���ϱ�(Ÿ�� -�� ��ġ) -- ��
        Vector3 directionToPlayer = Player.Instance.transform.position - transform.position;
        // �� ���� ����� �� ������ ���� ���ϱ�
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        // ������ �þ߰��� 1 / 2 ���� �۴ٸ� �þ߰� �ȿ� �ִ� ��.
        // �þ߰�(ex.120��) = �� ���� �������� �¿�� 60����
        return angle < fieldOfView * 0.5f;
    }
}