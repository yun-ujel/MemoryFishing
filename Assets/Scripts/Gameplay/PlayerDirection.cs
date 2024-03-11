using UnityEngine;
using UnityEngine.InputSystem;

namespace MemoryFishing.Gameplay.Player
{
    public class PlayerDirection : PlayerController
    {
        [SerializeField] private Camera lookCamera;

        [Header("Mouse Look")]
        [SerializeField] private bool usePlane;
        [SerializeField] private float planeYPos;

        [Space, SerializeField] private LayerMask mouseHitLayers;
        private Vector2 stickDirection;

        public Vector3 LookDirection { get; private set; }
        public Vector3 MousePoint { get; private set; }


        public override void SubscribeToInputActions()
        {
            playerInput.actions["Player/Look"].performed += OnLook;

            playerInput.actions["Player/MouseLook"].performed += MouseLook;
        }

        private void MouseLook(InputAction.CallbackContext ctx)
        {
            Ray lookRay = lookCamera.ScreenPointToRay(ctx.ReadValue<Vector2>());

            MousePoint = GetMousePoint(lookRay);

            LookDirection = (MousePoint - transform.position).ExcludeYAxis();
            LookDirection.Normalize();
        }

        private Vector3 GetMousePoint(Ray lookRay)
        {
            if (usePlane)
            {
                Plane plane = new(Vector3.up, Vector3.up * planeYPos);

                if (plane.Raycast(lookRay, out float distance))
                {
                    return lookRay.GetPoint(distance);
                }
                return Vector3.zero;
            }

            if (Physics.Raycast(lookRay, out RaycastHit hitInfo, Mathf.Infinity, mouseHitLayers))
            {
                return hitInfo.point;
            }
            return Vector3.zero;
        }

        private void OnLook(InputAction.CallbackContext ctx)
        {
            stickDirection = ctx.ReadValue<Vector2>();
            
            if (stickDirection.sqrMagnitude <= 0)
            {
                return;
            }

            LookDirection = stickDirection.normalized.OnZAxis();
        }
    }
}