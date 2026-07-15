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
        [SerializeField, Min(0.02f)] private float destinationUpdateInterval = 0.15f;

        private NavMeshAgent agent;
        private EnemyHealth health;

        private float nextAttackTime;
        private float nextDestinationUpdateTime;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<EnemyHealth>();
        }

        private void Start()
        {
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");

                if (player != null)
                    target = player.transform;
            }

            if (target == null)
            {
                Debug.LogError(
                    $"{name} no encontró un objeto con el tag Player.",
                    this
                );

                enabled = false;
            }
        }

        private void Update()
        {
            if (health.IsDead || target == null)
                return;

            float distanceToTarget = Vector3.Distance(
                transform.position,
                target.position
            );

            if (distanceToTarget <= attackRange)
            {
                StopAndAttack();
                return;
            }

            ChaseTarget();
        }

        private void ChaseTarget()
        {
            agent.isStopped = false;

            if (Time.time < nextDestinationUpdateTime)
                return;

            nextDestinationUpdateTime =
                Time.time + destinationUpdateInterval;

            agent.SetDestination(target.position);
        }

        private void StopAndAttack()
        {
            agent.isStopped = true;

            FaceTarget();

            if (Time.time < nextAttackTime)
                return;

            nextAttackTime = Time.time + attackCooldown;

            Debug.Log(
                $"{name} atacó al jugador e hizo {attackDamage} de daño."
            );
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