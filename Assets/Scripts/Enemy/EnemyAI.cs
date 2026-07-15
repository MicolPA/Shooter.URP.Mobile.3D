using FPS.Player;
using UnityEngine;
using UnityEngine.AI;

namespace FPS.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyHealth))]
    public class EnemyAI : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Attack")]
        [SerializeField, Min(0.1f)] private float attackRange = 2f;
        [SerializeField, Min(0f)] private float attackDamage = 10f;
        [SerializeField, Min(0.1f)] private float attackCooldown = 1f;

        [Header("Navigation")]
        [SerializeField, Min(0.02f)]
        private float destinationUpdateInterval = 0.15f;

        private NavMeshAgent agent;
        private EnemyHealth enemyHealth;
        private PlayerHealth targetHealth;

        private float nextAttackTime;
        private float nextDestinationUpdateTime;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            enemyHealth = GetComponent<EnemyHealth>();
        }

        private void Start()
        {
            FindTarget();

            if (target == null)
            {
                Debug.LogError(
                    $"{name} no encontró un objeto con el tag Player.",
                    this
                );

                enabled = false;
                return;
            }

            targetHealth = target.GetComponent<PlayerHealth>();

            if (targetHealth == null)
            {
                Debug.LogError(
                    $"{name} encontró al Player, pero no encontró PlayerHealth.",
                    target
                );

                enabled = false;
            }
        }

        private void Update()
        {
            if (enemyHealth.IsDead || target == null)
                return;

            if (targetHealth == null || targetHealth.IsDead)
            {
                StopAgent();
                return;
            }

            float distanceToTarget = Vector3.Distance(
                transform.position,
                target.position
            );

            if (distanceToTarget <= attackRange)
            {
                StopAndAttack();
            }
            else
            {
                ChaseTarget();
            }
        }

        private void FindTarget()
        {
            if (target != null)
                return;

            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
                target = player.transform;
        }

        private void ChaseTarget()
        {
            if (!agent.isOnNavMesh)
                return;

            agent.isStopped = false;

            if (Time.time < nextDestinationUpdateTime)
                return;

            nextDestinationUpdateTime =
                Time.time + destinationUpdateInterval;

            agent.SetDestination(target.position);
        }

        private void StopAndAttack()
        {
            StopAgent();
            FaceTarget();

            if (targetHealth == null || targetHealth.IsDead)
                return;

            if (Time.time < nextAttackTime)
                return;

            nextAttackTime = Time.time + attackCooldown;

            targetHealth.TakeDamage(attackDamage);

            Debug.Log(
                $"{name} atacó al jugador e hizo {attackDamage} de daño."
            );
        }

        private void StopAgent()
        {
            if (!agent.isOnNavMesh)
                return;

            agent.isStopped = true;
            agent.ResetPath();
        }

        private void FaceTarget()
        {
            Vector3 direction = target.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.001f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                agent.angularSpeed * Time.deltaTime
            );
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}