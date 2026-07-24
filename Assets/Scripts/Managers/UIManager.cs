using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FPS.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Gameplay HUD")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private TMP_Text waveText;
        [SerializeField] private TMP_Text enemiesText;
        [SerializeField] private TMP_Text ammoText;

        private int totalEnemiesInWave;

        [Header("Wave Announcement")]
        [SerializeField] private CanvasGroup waveAnnouncementGroup;
        [SerializeField] private TMP_Text waveAnnouncementText;
        [SerializeField, Min(0f)] private float fadeDuration = 0.5f;

        [Header("Lose")]
        [SerializeField] private GameObject loseCanvas;

        private Coroutine announcementRoutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (loseCanvas != null)
                loseCanvas.SetActive(false);

            if (waveAnnouncementGroup != null)
            {
                waveAnnouncementGroup.alpha = 0f;
                waveAnnouncementGroup.interactable = false;
                waveAnnouncementGroup.blocksRaycasts = false;
            }
        }

        public void UpdateHealth(float current, float max)
        {
            if (healthBar == null || max <= 0f)
                return;

            healthBar.value = current / max;
        }

        public void UpdateWave(int wave)
        {
            if (waveText != null)
                waveText.text = $"Wave {wave}";
        }

        public void SetTotalEnemiesInWave(int total)
        {
            totalEnemiesInWave = total;
        }

        public void UpdateEnemiesRemaining(int aliveCount)
        {
            if (enemiesText != null)
                enemiesText.text = $"Enemies: {aliveCount}";
        }

        public void ShowWaveAnnouncement(int waveNumber, float visibleDuration)
        {
            if (waveAnnouncementGroup == null ||
                waveAnnouncementText == null)
            {
                return;
            }

            if (announcementRoutine != null)
                StopCoroutine(announcementRoutine);

            announcementRoutine = StartCoroutine(
                WaveAnnouncementRoutine(waveNumber, visibleDuration)
            );
        }

        private IEnumerator WaveAnnouncementRoutine(
            int waveNumber,
            float visibleDuration)
        {
            waveAnnouncementText.text =
                $"Wave {waveNumber} begins soon";

            yield return FadeAnnouncement(0f, 1f);

            yield return new WaitForSeconds(visibleDuration);

            yield return FadeAnnouncement(1f, 0f);

            announcementRoutine = null;
        }

        private IEnumerator FadeAnnouncement(
            float startAlpha,
            float endAlpha)
        {
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;

                float progress = Mathf.Clamp01(
                    elapsed / fadeDuration
                );

                waveAnnouncementGroup.alpha = Mathf.Lerp(
                    startAlpha,
                    endAlpha,
                    progress
                );

                yield return null;
            }

            waveAnnouncementGroup.alpha = endAlpha;
        }

        public void ShowLose()
        {
            if (loseCanvas != null)
                loseCanvas.SetActive(true);
        }

        public void UpdateAmmo(int currentAmmo, int reserveAmmo)
        {
            if (ammoText == null)
                return;

            ammoText.text = $"{currentAmmo} / {reserveAmmo}";
        }
    }
}