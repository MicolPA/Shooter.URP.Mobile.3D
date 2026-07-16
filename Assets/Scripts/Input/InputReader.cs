using UnityEngine;
using UnityEngine.InputSystem;

namespace FPS.Input
{
    public class InputReader : MonoBehaviour
    {
        [Header("Mode")]
        [SerializeField] private GameObject mobileControls;

        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }

        public bool IsLookInputFromMouse { get; private set; }

        public bool IsFiring { get; private set; }
        public bool IsSprinting { get; private set; }

        public bool IsMobileMode =>
            mobileControls != null && mobileControls.activeInHierarchy;

        private bool jumpPressed;
        private bool reloadPressed;
        private bool pausePressed;
        private bool firePressed;

        public void OnMove(InputAction.CallbackContext context)
        {
            if (IsMobileMode)
                return;

            MoveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (IsMobileMode)
            {
                IsLookInputFromMouse = false;
                return;
            }

            LookInput = context.ReadValue<Vector2>();
            IsLookInputFromMouse = context.control?.device is Mouse;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (IsMobileMode && !ShouldProcessAction(context))
                return;

            if (context.performed)
                jumpPressed = true;
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (IsMobileMode && !ShouldProcessAction(context))
            {
                if (context.canceled)
                    IsFiring = false;

                return;
            }

            if (context.started)
            {
                IsFiring = true;
                firePressed = true;
            }
            else if (context.canceled)
            {
                IsFiring = false;
            }
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            if (IsMobileMode && !ShouldProcessAction(context))
                return;

            if (context.started)
            {
                reloadPressed = true;
            }
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (IsMobileMode && !ShouldProcessAction(context))
            {
                if (context.canceled)
                    IsSprinting = false;

                return;
            }

            if (context.performed)
                IsSprinting = true;

            if (context.canceled)
                IsSprinting = false;
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
                pausePressed = true;
        }

        public bool ConsumeJump()
        {
            if (!jumpPressed)
                return false;

            jumpPressed = false;
            return true;
        }

        public bool ConsumeReload()
        {
            if (!reloadPressed)
                return false;

            reloadPressed = false;
            return true;
        }

        public bool ConsumePause()
        {
            if (!pausePressed)
                return false;

            pausePressed = false;
            return true;
        }

        public bool ConsumeFire()
        {
            if (!firePressed)
                return false;

            firePressed = false;
            return true;
        }

        public void TriggerMobileFirePressed()
        {
            if (!IsMobileMode)
                return;

            IsFiring = true;
            firePressed = true;
        }

        public void TriggerMobileFireReleased()
        {
            if (!IsMobileMode)
                return;

            IsFiring = false;
        }

        public void TriggerMobileReloadPressed()
        {
            if (!IsMobileMode)
                return;

            reloadPressed = true;
        }

        public void TriggerMobileJumpPressed()
        {
            if (!IsMobileMode)
                return;

            jumpPressed = true;
        }

        public void PressMobileJump()
        {
            if (!IsMobileMode)
                return;

            jumpPressed = true;
        }

        public void PressMobileReload()
        {
            if (!IsMobileMode)
                return;

            reloadPressed = true;
        }

        public void SetMobileMoveInput(Vector2 value)
        {
            MoveInput = value;
        }

        public void SetMobileLookInput(Vector2 value)
        {
            LookInput = value;
            IsLookInputFromMouse = false;
        }

        public void SetMobileFire(bool isPressed)
        {
            if (!IsMobileMode)
                return;

            IsFiring = isPressed;
            firePressed = isPressed;
        }

        private static bool ShouldProcessAction(
            InputAction.CallbackContext context
        )
        {
            if (context.control?.device is Mouse)
                return false;

            if (context.control?.device is Keyboard)
                return false;

            return true;
        }
    }
}