using UnityEngine;

namespace FPS.Input
{
    public class MobileJoystickInput : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;

        [SerializeField] private Joystick movementJoystick;
        [SerializeField] private Joystick lookJoystick;

        private void Update()
        {
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
            ReadMobileInput();
#endif
        }

        private void ReadMobileInput()
        {
            if (inputReader == null)
                return;

            if (!inputReader.IsMobileMode)
                return;

            inputReader.SetMobileMoveInput(
                movementJoystick != null
                    ? new Vector2(
                        movementJoystick.Horizontal,
                        movementJoystick.Vertical)
                    : Vector2.zero
            );

            inputReader.SetMobileLookInput(
                lookJoystick != null
                    ? new Vector2(
                        lookJoystick.Horizontal,
                        lookJoystick.Vertical)
                    : Vector2.zero
            );
        }
    }
}