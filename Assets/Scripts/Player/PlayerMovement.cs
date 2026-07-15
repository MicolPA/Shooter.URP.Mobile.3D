using FPS.Input;
using UnityEngine;

namespace FPS.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(InputReader))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField, Min(0f)] private float walkSpeed = 5f;
        [SerializeField, Min(0f)] private float sprintSpeed = 8f;
        [SerializeField, Min(0f)] private float acceleration = 12f;
        [SerializeField, Min(0f)] private float deceleration = 16f;

        [Header("Jump")]
        [SerializeField, Min(0f)] private float jumpHeight = 1.5f;

        [Header("Gravity")]
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float groundedGravity = -2f;

        private CharacterController characterController;
        private InputReader inputReader;

        private Vector3 horizontalVelocity;
        private float verticalVelocity;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            inputReader = GetComponent<InputReader>();
        }

        private void Update()
        {
            UpdateHorizontalMovement();
            UpdateVerticalMovement();
            ApplyMovement();
        }

        private void UpdateHorizontalMovement()
        {
            Vector2 input = inputReader.MoveInput;

            Vector3 desiredDirection =
                transform.right * input.x +
                transform.forward * input.y;

            desiredDirection = Vector3.ClampMagnitude(
                desiredDirection,
                1f
            );

            float targetSpeed = inputReader.IsSprinting
                ? sprintSpeed
                : walkSpeed;

            Vector3 targetVelocity =
                desiredDirection * targetSpeed;

            bool isMoving =
                desiredDirection.sqrMagnitude > 0f;

            float speedChangeRate = isMoving
                ? acceleration
                : deceleration;

            horizontalVelocity = Vector3.MoveTowards(
                horizontalVelocity,
                targetVelocity,
                speedChangeRate * Time.deltaTime
            );
        }

        private void UpdateVerticalMovement()
        {
            bool isGrounded = characterController.isGrounded;

            if (isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = groundedGravity;
            }

            if (inputReader.ConsumeJump() && isGrounded)
            {
                verticalVelocity = Mathf.Sqrt(
                    jumpHeight * -2f * gravity
                );
            }

            verticalVelocity += gravity * Time.deltaTime;
        }

        private void ApplyMovement()
        {
            Vector3 finalVelocity = horizontalVelocity;
            finalVelocity.y = verticalVelocity;

            characterController.Move(
                finalVelocity * Time.deltaTime
            );
        }
    }
}