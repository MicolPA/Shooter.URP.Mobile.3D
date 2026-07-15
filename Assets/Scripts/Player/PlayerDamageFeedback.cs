using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FPS.Player
{
    public class PlayerDamageFeedback : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Image damageOverlay;

        [Header("Camera Shake")]
        [SerializeField, Min(0f)] private float shakeDuration = 0.15f;
        [SerializeField, Min(0f)] private float shakeIntensity = 0.05f;

        [Header("Damage Overlay")]
        [SerializeField, Range(0f, 1f)] private float maximumAlpha = 0.25f;
        [SerializeField, Min(0.01f)] private float overlayDuration = 0.8f;

        private Vector3 cameraPivotInitialPosition;
        private Coroutine feedbackRoutine;

        private void Awake()
        {
            if (playerHealth == null)
                playerHealth = GetComponent<PlayerHealth>();

            if (cameraPivot != null)
                cameraPivotInitialPosition = cameraPivot.localPosition;

            SetOverlayAlpha(0f);
        }

        private void OnEnable()
        {
            if (playerHealth != null)
                playerHealth.OnDamaged += HandlePlayerDamaged;
        }

        private void OnDisable()
        {
            if (playerHealth != null)
                playerHealth.OnDamaged -= HandlePlayerDamaged;
        }

        private void HandlePlayerDamaged(float damage)
        {
            if (feedbackRoutine != null)
                StopCoroutine(feedbackRoutine);

            feedbackRoutine = StartCoroutine(DamageFeedbackRoutine());
        }

        private IEnumerator DamageFeedbackRoutine()
        {
            float elapsedTime = 0f;

            SetOverlayAlpha(maximumAlpha);

            while (elapsedTime < overlayDuration)
            {
                elapsedTime += Time.deltaTime;

                float overlayProgress =
                    Mathf.Clamp01(elapsedTime / overlayDuration);

                SetOverlayAlpha(
                    Mathf.Lerp(maximumAlpha, 0f, overlayProgress)
                );

                if (elapsedTime < shakeDuration && cameraPivot != null)
                {
                    Vector2 randomOffset =
                        Random.insideUnitCircle * shakeIntensity;

                    cameraPivot.localPosition =
                        cameraPivotInitialPosition +
                        new Vector3(randomOffset.x, randomOffset.y, 0f);
                }
                else if (cameraPivot != null)
                {
                    cameraPivot.localPosition =
                        cameraPivotInitialPosition;
                }

                yield return null;
            }

            if (cameraPivot != null)
                cameraPivot.localPosition = cameraPivotInitialPosition;

            SetOverlayAlpha(0f);
            feedbackRoutine = null;
        }

        private void SetOverlayAlpha(float alpha)
        {
            if (damageOverlay == null)
                return;

            Color color = damageOverlay.color;
            color.a = alpha;
            damageOverlay.color = color;
        }
    }
}