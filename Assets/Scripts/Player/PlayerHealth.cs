using System;
using UnityEngine;

namespace FPS.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private float maxHealth = 100f;

        public float CurrentHealth { get; private set; }

        public bool IsDead { get; private set; }

        public event Action<PlayerHealth> OnDeath;

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (IsDead)
                return;

            CurrentHealth -= damage;

            Debug.Log($"Player recibió {damage} de daño. Vida: {CurrentHealth}");

            if (CurrentHealth <= 0f)
            {
                Die();
            }
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