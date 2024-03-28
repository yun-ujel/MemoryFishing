using UnityEngine;
using UnityEngine.InputSystem;

using static MemoryFishing.Utilities.VectorUtils;

namespace MemoryFishing.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class BoatMovement : PlayerController
    {
        private Vector2 moveDir;
        private Rigidbody body;

        private float currentRotationSpeed;
        private float currentMovementSpeed;

        [Header("Turning")]
        [SerializeField, Range(0f, 10f)] private float turnAcceleration = 1f;
        [SerializeField, Range(0f, 10f)] private float turnRate = 0.1f;

        [Header("Moving")]
        [SerializeField, Range(0f, 10f)] private float moveAcceleration = 0.01f;

        [Space]

        [SerializeField, Range(0f, 10f)] private float maxForwardSpeed = 2f;
        [SerializeField, Range(0f, 10f)] private float maxReverseSpeed = 0.5f;

        private float YRotation
        {
            get
            {
                return transform.rotation.eulerAngles.y;
            }
            set
            {
                Vector3 eulers = transform.rotation.eulerAngles;
                eulers.y = value;

                transform.rotation = Quaternion.Euler(eulers);
            }
        }

        public override void Start()
        {
            base.Start();

            body = GetComponent<Rigidbody>();
        }

        public override void SubscribeToInputActions()
        {
            base.SubscribeToInputActions();

            playerInput.actions["Player/Move"].performed += ReceiveMoveInput;
            playerInput.actions["Player/Move"].canceled += ReceiveMoveInput;
        }

        private void ReceiveMoveInput(InputAction.CallbackContext ctx)
        {
            moveDir = ctx.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            RotateBoat();
            AccelerateBoat();
        }

        private void RotateBoat()
        {
            currentRotationSpeed = Mathf.MoveTowards(currentRotationSpeed, moveDir.x, Time.fixedDeltaTime * turnAcceleration);
            YRotation += currentRotationSpeed * turnRate;
        }

        private void AccelerateBoat()
        {
            currentMovementSpeed = Mathf.MoveTowards(currentMovementSpeed, moveDir.y, Time.fixedDeltaTime * moveAcceleration);

            float speed = currentMovementSpeed < 0 ? maxReverseSpeed : maxForwardSpeed;
            speed *= currentMovementSpeed;

            body.velocity = transform.right * speed;
        }
    }
}