using UnityEngine;
using UnityEngine.InputSystem;

namespace FPS.Input
{
    public class InputReader : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }

        public bool IsFiring { get; private set; }
        public bool IsSprinting { get; private set; }

        private bool jumpPressed;
        private bool reloadPressed;
        private bool pausePressed;

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            // Debug.Log($"Move Input: {LookInput}");
            LookInput = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
                jumpPressed = true;
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.performed)
                IsFiring = true;

            if (context.canceled)
                IsFiring = false;
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            if (context.performed)
                reloadPressed = true;
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
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
    }
}