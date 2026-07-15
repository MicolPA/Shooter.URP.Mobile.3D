using FPS.Player;
using UnityEngine;

namespace FPS.Enemies
{
    [RequireComponent(typeof(EnemyHealth))]
    public class FlyingEnemyAI : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Movement")]
        [SerializeField, Min(0f)] private float moveSpeed = 4f;
        [SerializeField, Min(0f)] private float rotationSpeed = 360f;
        [SerializeField, Min(0f)] private float preferredHeight = 2.5f;

        [Header("Attack")]
        [SerializeField, Min(0.1f)] private float attackRange = 2.5f;
        [SerializeField, Min(0f)] private float attackDamage = 10f;
        [SerializeField, Min(0.1f)] private float attackCooldown = 1f;

        private EnemyHealth enemyHealth;
        private PlayerHealth targetHealth;

        private float nextAttackTime;

        private void Awake()
        {
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
                    $"{name} encontró al jugador, pero no encontró PlayerHealth.",
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
                return;

            Vector3 attackTargetPosition = GetAttackTargetPosition();

            float distanceToTarget = Vector3.Distance(
                transform.position,
                attackTargetPosition
            );

            FaceTarget(attackTargetPosition);

            if (distanceToTarget <= attackRange)
            {
                TryAttack();
                return;
            }

            MoveTowardsTarget(attackTargetPosition);
        }

        private void FindTarget()
        {
            if (target != null)
                return;

            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
                target = player.transform;
        }

        private Vector3 GetAttackTargetPosition()
        {
            return target.position + Vector3.up * preferredHeight;
        }

        private void MoveTowardsTarget(Vector3 targetPosition)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
        }

        private void FaceTarget(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - transform.position;

            if (direction.sqrMagnitude <= 0.001f)
                return;

            Quaternion desiredRotation =
                Quaternion.LookRotation(direction.normalized);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                desiredRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        private void TryAttack()
        {
            if (Time.time < nextAttackTime)
                return;

            nextAttackTime = Time.time + attackCooldown;

            targetHealth.TakeDamage(attackDamage);

            Debug.Log(
                $"{name} atacó al jugador desde el aire e hizo " +
                $"{attackDamage} de daño."
            );
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}