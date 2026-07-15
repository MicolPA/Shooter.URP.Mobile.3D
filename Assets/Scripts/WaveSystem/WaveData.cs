using UnityEngine;

namespace FPS.Waves
{
    [CreateAssetMenu(
        fileName = "WaveData",
        menuName = "FPS/Waves/Wave Data"
    )]
    public class WaveData : ScriptableObject
    {
        [Header("Ground Enemy Ver01")]
        [SerializeField] private GameObject groundEnemyVer01Prefab;
        [SerializeField, Min(0)] private int groundEnemyVer01Count;

        [Header("Ground Enemy Ver02")]
        [SerializeField] private GameObject groundEnemyVer02Prefab;
        [SerializeField, Min(0)] private int groundEnemyVer02Count;

        [Header("Flying Enemy")]
        [SerializeField] private GameObject flyingEnemyPrefab;
        [SerializeField, Min(0)] private int flyingEnemyCount;

        [Header("Wave Settings")]
        [SerializeField, Min(0.05f)] private float spawnInterval = 1f;
        [SerializeField, Min(0f)] private float delayBeforeNextWave = 5f;

        public GameObject GroundEnemyVer01Prefab => groundEnemyVer01Prefab;
        public int GroundEnemyVer01Count => groundEnemyVer01Count;

        public GameObject GroundEnemyVer02Prefab => groundEnemyVer02Prefab;
        public int GroundEnemyVer02Count => groundEnemyVer02Count;

        public GameObject FlyingEnemyPrefab => flyingEnemyPrefab;
        public int FlyingEnemyCount => flyingEnemyCount;

        public float SpawnInterval => spawnInterval;
        public float DelayBeforeNextWave => delayBeforeNextWave;

        public int TotalEnemyCount =>
            groundEnemyVer01Count +
            groundEnemyVer02Count +
            flyingEnemyCount;
    }
}