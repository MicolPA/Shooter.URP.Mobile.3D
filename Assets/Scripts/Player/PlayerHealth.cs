using System;
using UnityEngine;
using FPS.Audio;

namespace FPS.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private float maxHealth = 100f;

        public float CurrentHealth { get; private set; }

        public bool IsDead { get; private set; }

        public event Action<PlayerHealth> OnDeath;
        public event Action<float> OnDamaged;
        
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

            audioManager?.PlayPlayerDamage();

            OnDamaged?.Invoke(damage);

            Debug.Log(
                $"Player recibió {damage} de daño. Vida: {CurrentHealth}"
            );

            if (CurrentHealth <= 0f)
                Die();
        }

        public void Heal(float amount)
        {
            if (IsDead)
                return;

            CurrentHealth = Mathf.Min(
                CurrentHealth + amount,
                maxHealth);
        }

        private void Die()
        {
            IsDead = true;

            Debug.Log("Player murió.");

            OnDeath?.Invoke(this);
        }
    }
}