using FPS.Input;
using UnityEngine;

namespace FPS.Player
{
    [RequireComponent(typeof(InputReader))]
    public class PlayerLook : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraPivot;

        [Header("Mouse Settings")]
        [SerializeField, Min(0f)] private float mouseSensitivity = 0.12f;

        [Header("Vertical Rotation")]
        [SerializeField] private float minimumPitch = -85f;
        [SerializeField] private float maximumPitch = 85f;

        [Header("Cursor")]
        [SerializeField] private bool lockCursorOnStart = true;

        private InputReader inputReader;
        private float pitch;

        private void Awake()
        {
            inputReader = GetComponent<InputReader>();

            if (cameraPivot == null)
            {
                Debug.LogError(
                    "PlayerLook necesita una referencia a CameraPivot.",
                    this
                );
            }
        }

        private void Start()
        {
            if (lockCursorOnStart)
                SetCursorLocked(true);
        }

        private void Update()
        {
            UpdateLook();
        }

        private void UpdateLook()
        {
            if (cameraPivot == null)
                return;

            Vector2 lookInput = inputReader.LookInput;

            float yawDelta = lookInput.x * mouseSensitivity;
            float pitchDelta = lookInput.y * mouseSensitivity;

            transform.Rotate(Vector3.up * yawDelta);

            pitch -= pitchDelta;
            pitch = Mathf.Clamp(pitch, minimumPitch, maximumPitch);

            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus && lockCursorOnStart)
                SetCursorLocked(true);
        }

        private static void SetCursorLocked(bool isLocked)
        {
            Cursor.lockState = isLocked
                ? CursorLockMode.Locked
                : CursorLockMode.None;

            Cursor.visible = !isLocked;
        }
    }
}