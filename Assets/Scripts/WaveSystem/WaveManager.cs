using System.Collections;
using System.Collections.Generic;
using FPS.Enemies;
using UnityEngine;

namespace FPS.Waves
{
    public class WaveManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EnemySpawner enemySpawner;

        [Header("Waves")]
        [SerializeField] private WaveData[] waves;

        [Header("Startup")]
        [SerializeField, Min(0f)] private float initialDelay = 2f;
        [SerializeField] private bool startAutomatically = true;

        public int CurrentWaveNumber { get; private set; }
        public int AliveEnemyCount => aliveEnemies.Count;
        public bool IsWaveRunning { get; private set; }
        public bool AllWavesCompleted { get; private set; }

        private readonly HashSet<EnemyHealth> aliveEnemies = new();

        private Coroutine waveRoutine;

        private void Start()
        {
            if (startAutomatically)
                StartWaves();
        }

        public void StartWaves()
        {
            if (waveRoutine != null)
                return;

            if (!ValidateConfiguration())
                return;

            waveRoutine = StartCoroutine(RunWavesRoutine());
        }

        private IEnumerator RunWavesRoutine()
        {
            yield return new WaitForSeconds(initialDelay);

            for (int waveIndex = 0; waveIndex < waves.Length; waveIndex++)
            {
                WaveData currentWave = waves[waveIndex];

                CurrentWaveNumber = waveIndex + 1;
                IsWaveRunning = true;

                Debug.Log($"Comienza la oleada {CurrentWaveNumber}.");

                yield return StartCoroutine(SpawnWaveRoutine(currentWave));

                yield return new WaitUntil(() => aliveEnemies.Count == 0);

                IsWaveRunning = false;

                Debug.Log($"Oleada {CurrentWaveNumber} completada.");

                bool isLastWave = waveIndex >= waves.Length - 1;

                if (!isLastWave)
                {
                    yield return new WaitForSeconds(
                        currentWave.DelayBeforeNextWave
                    );
                }
            }

            AllWavesCompleted = true;
            waveRoutine = null;

            Debug.Log("Todas las oleadas fueron completadas.");
        }

        private IEnumerator SpawnWaveRoutine(WaveData wave)
        {
            List<SpawnRequest> spawnRequests = BuildSpawnRequests(wave);

            Shuffle(spawnRequests);

            foreach (SpawnRequest request in spawnRequests)
            {
                EnemyHealth spawnedEnemy = SpawnEnemy(request);

                if (spawnedEnemy != null)
                    RegisterEnemy(spawnedEnemy);

                yield return new WaitForSeconds(wave.SpawnInterval);
            }
        }

        private List<SpawnRequest> BuildSpawnRequests(WaveData wave)
        {
            var requests = new List<SpawnRequest>(wave.TotalEnemyCount);

            AddRequests(
                requests,
                wave.GroundEnemyVer01Prefab,
                wave.GroundEnemyVer01Count,
                false
            );

            AddRequests(
                requests,
                wave.GroundEnemyVer02Prefab,
                wave.GroundEnemyVer02Count,
                false
            );

            AddRequests(
                requests,
                wave.FlyingEnemyPrefab,
                wave.FlyingEnemyCount,
                true
            );

            return requests;
        }

        private static void AddRequests(
            List<SpawnRequest> requests,
            GameObject prefab,
            int count,
            bool isFlying)
        {
            if (count <= 0)
                return;

            if (prefab == null)
            {
                Debug.LogWarning(
                    $"Se solicitaron {count} enemigos, pero no hay prefab asignado."
                );

                return;
            }

            for (int i = 0; i < count; i++)
            {
                requests.Add(new SpawnRequest(prefab, isFlying));
            }
        }

        private EnemyHealth SpawnEnemy(SpawnRequest request)
        {
            return request.IsFlying
                ? enemySpawner.SpawnFlyingEnemy(request.Prefab)
                : enemySpawner.SpawnGroundEnemy(request.Prefab);
        }

        private void RegisterEnemy(EnemyHealth enemy)
        {
            if (!aliveEnemies.Add(enemy))
                return;

            enemy.OnDeath += HandleEnemyDeath;
        }

        private void HandleEnemyDeath(EnemyHealth enemy)
        {
            enemy.OnDeath -= HandleEnemyDeath;
            aliveEnemies.Remove(enemy);

            Debug.Log(
                $"Enemigo eliminado. Quedan {aliveEnemies.Count} enemigos vivos."
            );
        }

        private bool ValidateConfiguration()
        {
            if (enemySpawner == null)
            {
                Debug.LogError(
                    "WaveManager necesita una referencia a EnemySpawner.",
                    this
                );

                return false;
            }

            if (waves == null || waves.Length == 0)
            {
                Debug.LogError(
                    "WaveManager no tiene oleadas configuradas.",
                    this
                );

                return false;
            }

            for (int i = 0; i < waves.Length; i++)
            {
                if (waves[i] != null)
                    continue;

                Debug.LogError(
                    $"La oleada en la posición {i} no está asignada.",
                    this
                );

                return false;
            }

            return true;
        }

        private static void Shuffle(List<SpawnRequest> requests)
        {
            for (int i = requests.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);

                (requests[i], requests[randomIndex]) =
                    (requests[randomIndex], requests[i]);
            }
        }

        private readonly struct SpawnRequest
        {
            public GameObject Prefab { get; }
            public bool IsFlying { get; }

            public SpawnRequest(GameObject prefab, bool isFlying)
            {
                Prefab = prefab;
                IsFlying = isFlying;
            }
        }
    }
}