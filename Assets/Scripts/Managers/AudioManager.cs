using UnityEngine;

namespace FPS.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Music")]
        [SerializeField] private AudioClip backgroundMusic;

        [Header("Player SFX")]
        [SerializeField] private AudioClip gunshotClip;
        [SerializeField] private AudioClip reloadClip;
        [SerializeField] private AudioClip playerDamageClip;

        [Header("Enemy SFX")]
        [SerializeField] private AudioClip enemyDamageClip;
        [SerializeField] private AudioClip enemyDeathClip;

        private void Start()
        {
            PlayBackgroundMusic();
        }

        public void PlayGunshot()
        {
            PlaySFX(gunshotClip);
        }

        public void PlayReload()
        {
            PlaySFX(reloadClip);
        }

        public void PlayPlayerDamage()
        {
            PlaySFX(playerDamageClip);
        }

        public void PlayEnemyDamage()
        {
            PlaySFX(enemyDamageClip);
        }

        public void PlayEnemyDeath()
        {
            PlaySFX(enemyDeathClip);
        }

        private void PlayBackgroundMusic()
        {
            if (musicSource == null || backgroundMusic == null)
                return;

            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }

        private void PlaySFX(AudioClip clip)
        {
            if (sfxSource == null || clip == null)
                return;

            sfxSource.PlayOneShot(clip);
        }

        private void OnValidate()
        {
            if (musicSource == sfxSource && musicSource != null)
            {
                Debug.LogWarning(
                    "Music Source y SFX Source deberían ser AudioSource diferentes.",
                    this
                );
            }
        }
    }
}