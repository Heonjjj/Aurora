using UnityEngine;
using UnityEngine.AI;

public class SecurityGuardFootStep : MonoBehaviour
{
    public AudioClip footstepClip;
    private AudioSource audioSource;
    private NavMeshAgent navMeshAgent; // Rigidbody 대신 NavMeshAgent 사용

    public float footstepThreshold;
    public float footstepRate;
    private float footStepTime;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // NavMeshAgent가 목적지로 이동 중인지 확인
        if (navMeshAgent.hasPath && navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            // 에이전트의 현재 속도가 임계값보다 높은지 확인
            if (navMeshAgent.velocity.magnitude > footstepThreshold)
            {
                if (Time.time - footStepTime > footstepRate)
                {
                    if (!audioSource.isPlaying)
                    {
                        footStepTime = Time.time;
                        audioSource.PlayOneShot(footstepClip);
                    }
                }
            }
        }
    }
}