using FPS.Enemies;
using UnityEngine;
using UnityEngine.AI;

namespace FPS.Waves
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Ground Spawn Points")]
        [SerializeField] private Transform[] groundSpawnPoints;

        [Header("Flying Spawn Points")]
        [SerializeField] private Transform[] flyingSpawnPoints;

        [Header("NavMesh")]
        [SerializeField, Min(0.1f)]
        private float navMeshSearchRadius = 2f;

        public EnemyHealth SpawnGroundEnemy(GameObject enemyPrefab)
        {
            if (!CanSpawn(enemyPrefab, groundSpawnPoints, "terrestre"))
                return null;

            Transform spawnPoint = GetRandomSpawnPoint(groundSpawnPoints);

            if (!TryGetNavMeshPosition(spawnPoint.position, out Vector3 spawnPosition))
            {
                Debug.LogWarning(
                    $"No se encontró un NavMesh cerca de {spawnPoint.name}.",
                    spawnPoint
                );

                return null;
            }

            return CreateEnemy(
                enemyPrefab,
                spawnPosition,
                spawnPoint.rotation
            );
        }

        public EnemyHealth SpawnFlyingEnemy(GameObject enemyPrefab)
        {
            if (!CanSpawn(enemyPrefab, flyingSpawnPoints, "volador"))
                return null;

            Transform spawnPoint = GetRandomSpawnPoint(flyingSpawnPoints);

            return CreateEnemy(
                enemyPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );
        }

        private bool CanSpawn(
            GameObject enemyPrefab,
            Transform[] spawnPoints,
            string enemyType)
        {
            if (enemyPrefab == null)
            {
                Debug.LogError(
                    $"No se asignó el prefab del enemigo {enemyType}.",
                    this
                );

                return false;
            }

            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogError(
                    $"No existen puntos de aparición para el enemigo {enemyType}.",
                    this
                );

                return false;
            }

            return true;
        }

        private static Transform GetRandomSpawnPoint(
            Transform[] spawnPoints)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            return spawnPoints[randomIndex];
        }

        private bool TryGetNavMeshPosition(
            Vector3 requestedPosition,
            out Vector3 validPosition)
        {
            if (NavMesh.SamplePosition(
                    requestedPosition,
                    out NavMeshHit hit,
                    navMeshSearchRadius,
                    NavMesh.AllAreas))
            {
                validPosition = hit.position;
                return true;
            }

            validPosition = default;
            return false;
        }

        private EnemyHealth CreateEnemy(
            GameObject enemyPrefab,
            Vector3 position,
            Quaternion rotation)
        {
            GameObject enemyInstance = Instantiate(
                enemyPrefab,
                position,
                rotation
            );

            EnemyHealth enemyHealth =
                enemyInstance.GetComponent<EnemyHealth>();

            if (enemyHealth == null)
            {
                Debug.LogError(
                    $"El prefab {enemyPrefab.name} no contiene EnemyHealth.",
                    enemyInstance
                );

                Destroy(enemyInstance);
                return null;
            }

            return enemyHealth;
        }

        private void OnDrawGizmosSelected()
        {
            DrawSpawnPoints(groundSpawnPoints);
            DrawSpawnPoints(flyingSpawnPoints);
        }

        private static void DrawSpawnPoints(Transform[] spawnPoints)
        {
            if (spawnPoints == null)
                return;

            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint == null)
                    continue;

                Gizmos.DrawWireSphere(spawnPoint.position, 0.4f);
                Gizmos.DrawLine(
                    spawnPoint.position,
                    spawnPoint.position + spawnPoint.forward
                );
            }
        }
        
    }
}