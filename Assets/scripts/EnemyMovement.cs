using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform[] patrolPoints;
    public Transform player;
    public float detectionRange = 8f;
    public float visionAngle = 90f;

    public LayerMask detectionMask;
    private NavMeshAgent navMeshAgent;
    private int currentPoint = 0;
    private bool isChasing = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (patrolPoints.Length > 0)
        {
            navMeshAgent.SetDestination(patrolPoints[currentPoint].position);
        }
    }

    void Update()
    {
        if (!isChasing)
        {
            Patrol();

            Vector3 directionToPlayer = (player.position - transform.position);
            float distanceToPlayer = directionToPlayer.magnitude;

            if (distanceToPlayer <= detectionRange)
            {
                float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer.normalized);

                if (angleToPlayer <= visionAngle / 2f)
                {
                    Ray ray = new Ray(transform.position + Vector3.up * 0.5f, directionToPlayer.normalized);
                    RaycastHit hit;

                    Debug.DrawRay(ray.origin, ray.direction * detectionRange, Color.red);

                    if (Physics.Raycast(ray, out hit, detectionRange, detectionMask))
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            isChasing = true;
                            Debug.Log("✅ Player detected in vision cone. Chasing...");
                        }
                    }
                }
            }
        }
        else
        {
            if (player != null)
            {
                navMeshAgent.SetDestination(player.position);
            }
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length < 2) return;
        if (navMeshAgent.isStopped) return;

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
            {
                currentPoint = (currentPoint + 1) % patrolPoints.Length;
                navMeshAgent.SetDestination(patrolPoints[currentPoint].position);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isChasing && other.CompareTag("Player"))
        {
            Debug.Log("Player caught!");

            playercontrols pc = other.GetComponent<playercontrols>();
            if (pc != null)
            {
                pc.FreezePlayer();
            }

            isChasing = false;
            navMeshAgent.ResetPath();
            navMeshAgent.isStopped = true;

            Vector3 lookDirection = (other.transform.position - transform.position).normalized;
            lookDirection.y = 0f;
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }

    public void StopChasing()
    {
        isChasing = false;
        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = true;

        Vector3 lookDirection = (player.position - transform.position).normalized;
        lookDirection.y = 0f;
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }
}









