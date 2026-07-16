using FPS.Input;
using UnityEngine;

namespace FPS.Player
{
    [RequireComponent(typeof(InputReader))]
    public class PlayerLook : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraPivot;

        [Header("Sensitivity")]
        [SerializeField, Min(0f)] private float mouseSensitivity = 0.12f;
        [SerializeField, Min(0f)] private float joystickSensitivity = 150f;

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
            ApplyCursorState();
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

            float yawDelta;
            float pitchDelta;

            if (inputReader.IsMobileMode)
            {
                yawDelta =
                    lookInput.x *
                    joystickSensitivity *
                    Time.deltaTime;

                pitchDelta =
                    lookInput.y *
                    joystickSensitivity *
                    Time.deltaTime;
            }
            else if (inputReader.IsLookInputFromMouse)
            {
                yawDelta = lookInput.x * mouseSensitivity;
                pitchDelta = lookInput.y * mouseSensitivity;
            }
            else
            {
                yawDelta =
                    lookInput.x *
                    joystickSensitivity *
                    Time.deltaTime;

                pitchDelta =
                    lookInput.y *
                    joystickSensitivity *
                    Time.deltaTime;
            }

            transform.Rotate(Vector3.up * yawDelta);

            pitch -= pitchDelta;
            pitch = Mathf.Clamp(
                pitch,
                minimumPitch,
                maximumPitch
            );

            cameraPivot.localRotation =
                Quaternion.Euler(pitch, 0f, 0f);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
                ApplyCursorState();
        }

        private void ApplyCursorState()
        {
            bool shouldLock = lockCursorOnStart && !inputReader.IsMobileMode;
            SetCursorLocked(shouldLock);
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