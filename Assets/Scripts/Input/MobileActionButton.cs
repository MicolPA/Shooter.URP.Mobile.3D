using UnityEngine;
using UnityEngine.EventSystems;

namespace FPS.Input
{
    public enum MobileAction
    {
        Fire,
        Jump,
        Reload
    }

    public class MobileActionButton :
        MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler
    {
        [Header("References")]
        [SerializeField] private InputReader inputReader;

        [Header("Action")]
        [SerializeField] private MobileAction action;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (inputReader == null)
                return;

            switch (action)
            {
                case MobileAction.Fire:
                    inputReader.SetMobileFire(true);
                    break;

                case MobileAction.Jump:
                    inputReader.PressMobileJump();
                    break;

                case MobileAction.Reload:
                    inputReader.PressMobileReload();
                    break;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (inputReader == null)
                return;

            if (action == MobileAction.Fire)
                inputReader.SetMobileFire(false);
        }

        private void OnDisable()
        {
            if (inputReader != null && action == MobileAction.Fire)
                inputReader.SetMobileFire(false);
        }
    }
}
