using System.Collections;
using FPS.Core;
using FPS.Input;
using UnityEngine;
using FPS.Audio;
using FPS.UI;

namespace FPS.Weapons
{
    [RequireComponent(typeof(InputReader))]
    public class PlayerWeapon : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera playerCamera;

        [Header("Weapon")]
        [SerializeField, Min(0f)] private float damage = 25f;
        [SerializeField, Min(0.01f)] private float fireRate = 5f;
        [SerializeField, Min(0f)] private float range = 100f;
        [SerializeField] private LayerMask hitLayers = ~0;

        [Header("Ammunition")]
        [SerializeField, Min(1)] private int magazineSize = 20;
        [SerializeField, Min(0)] private int reserveAmmo = 120;
        [SerializeField, Min(0f)] private float reloadDuration = 1.5f;

        private InputReader inputReader;

        private int currentAmmo;
        private float nextFireTime;
        private bool isReloading;

        public int CurrentAmmo => currentAmmo;
        public int ReserveAmmo => reserveAmmo;
        public bool IsReloading => isReloading;

        [Header("Audio")]
        [SerializeField] private AudioManager audioManager;

        private void Awake()
        {
            inputReader = GetComponent<InputReader>();
            currentAmmo = magazineSize;

            UIManager.Instance?.UpdateAmmo(currentAmmo, reserveAmmo);

            if (playerCamera == null)
            {
                Debug.LogError(
                    "PlayerWeapon necesita una referencia a la cámara del jugador.",
                    this
                );
            }
        }

        private void Update()
        {
            HandleReloadInput();
            HandleFireInput();
        }

        private void HandleFireInput()
        {
            if (!inputReader.ConsumeFire())
                return;

            if (isReloading)
                return;

            if (currentAmmo <= 0)
            {
                TryStartReload();
                return;
            }

            if (!CanFire())
                return;

            Fire();
        }

        private bool CanFire()
        {
            if (isReloading)
                return false;

            if (currentAmmo <= 0)
            {
                TryStartReload();
                return false;
            }

            return Time.time >= nextFireTime;
        }

        private void Fire()
        {
            nextFireTime = Time.time + 1f / fireRate;
            currentAmmo--;

            UIManager.Instance?.UpdateAmmo(currentAmmo, reserveAmmo);

            audioManager?.PlayGunshot();

            Ray ray = new Ray(
                playerCamera.transform.position,
                playerCamera.transform.forward
            );

            if (Physics.Raycast(
                    ray,
                    out RaycastHit hit,
                    range,
                    hitLayers,
                    QueryTriggerInteraction.Ignore))
            {
                HandleHit(hit);
            }

            Debug.Log(
                $"Disparo realizado. Munición: {currentAmmo}/{reserveAmmo}"
            );
        }

        private void HandleHit(RaycastHit hit)
        {
            IDamageable damageable =
                hit.collider.GetComponentInParent<IDamageable>();

            damageable?.TakeDamage(damage);
        }

        private void HandleReloadInput()
        {
            if (!inputReader.ConsumeReload())
                return;

            TryStartReload();
        }

        private void TryStartReload()
        {
            Debug.Log(
                $"Intentando recargar. Munición actual: {currentAmmo}, reserva: {reserveAmmo}"
            );

            if (isReloading)
                return;

            if (currentAmmo >= magazineSize)
                return;

            if (reserveAmmo <= 0)
                return;

            StartCoroutine(ReloadRoutine());
        }

        private IEnumerator ReloadRoutine()
        {
            isReloading = true;

            audioManager?.PlayReload();

            Debug.Log("Recargando...");

            yield return new WaitForSeconds(reloadDuration);

            int requiredAmmo = magazineSize - currentAmmo;
            int ammoToLoad = Mathf.Min(requiredAmmo, reserveAmmo);

            currentAmmo += ammoToLoad;
            reserveAmmo -= ammoToLoad;

            UIManager.Instance?.UpdateAmmo(currentAmmo, reserveAmmo);

            isReloading = false;

            Debug.Log(
                $"Recarga terminada. Munición: {currentAmmo}/{reserveAmmo}"
            );
        }
    }
}