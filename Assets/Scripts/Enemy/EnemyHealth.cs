using System;
using FPS.Core;
using UnityEngine;
using FPS.Audio;

namespace FPS.Enemies
{
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [Header("Health")]
        [SerializeField, Min(1f)] private float maxHealth = 100f;

        [Header("Death")]
        [SerializeField] private ParticleSystem deathEffect;

        public float CurrentHealth { get; private set; }
        public bool IsDead { get; private set; }

        public event Action<EnemyHealth> OnDeath;

        [Header("Audio")]
        [SerializeField] private AudioManager audioManager;

        private void Awake()
        {
            CurrentHealth = maxHealth;
            
            if (audioManager == null)
                audioManager = FindFirstObjectByType<AudioManager>();
        }

        public void TakeDamage(float damage)
        {
            if (IsDead || damage <= 0f)
                return;

            CurrentHealth = Mathf.Max(CurrentHealth - damage, 0f);

            audioManager?.PlayEnemyDamage();

            Debug.Log(
                $"{name} recibió {damage} de daño. Vida: {CurrentHealth}"
            );

            if (CurrentHealth <= 0f)
                Die();
        }

        private void Die()
        {
            if (IsDead)
                return;

            IsDead = true;

            audioManager?.PlayEnemyDeath();
            
            OnDeath?.Invoke(this);

            SpawnDeathEffect();

            Destroy(gameObject);
        }

        private void SpawnDeathEffect()
        {
            if (deathEffect == null)
                return;

            ParticleSystem effectInstance = Instantiate(
                deathEffect,
                transform.position,
                Quaternion.identity
            );

            effectInstance.Play();
        }
    }
}