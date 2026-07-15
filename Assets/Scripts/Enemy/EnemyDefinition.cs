using UnityEngine;

namespace FPS.Enemies
{
    [CreateAssetMenu(
        fileName = "EnemyDefinition",
        menuName = "FPS/Enemies/Enemy Definition"
    )]
    public class EnemyDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string enemyName;
        [SerializeField] private EnemyType enemyType;

        [Header("Prefab")]
        [SerializeField] private GameObject enemyPrefab;

        public string EnemyName => enemyName;
        public EnemyType EnemyType => enemyType;
        public GameObject EnemyPrefab => enemyPrefab;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(enemyName))
                enemyName = name;

            if (enemyPrefab == null)
                return;

            if (enemyPrefab.GetComponent<EnemyHealth>() == null)
            {
                Debug.LogWarning(
                    $"El prefab asignado a {name} no contiene EnemyHealth.",
                    this
                );
            }
        }
    }
}