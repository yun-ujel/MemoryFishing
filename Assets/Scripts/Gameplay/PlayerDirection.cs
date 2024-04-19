using MemoryFishing.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace MemoryFishing.Gameplay
{
    public class PlayerDirection : PlayerController
    {
        [SerializeField] private Camera lookCamera;

        [Header("Mouse Look")]
        [SerializeField] private bool usePlane;
        [SerializeField] private float planeYPos;

        [Space, SerializeField] private LayerMask mouseHitLayers;
        private Vector2 stickDirection;

        public Vector3 LookDirection { get; private set; } = Vector3.forward;

        private Vector3 mousePoint;
        private bool mouseUsedLast;

        public Vector3 GetLookDirection(Vector3 fromPosition)
        {
            if (mouseUsedLast)
            {
                Vector2Control mouseControl = Mouse.current.position;
                UpdateLookDirectionMouse(mouseControl.value);
                return (mousePoint - fromPosition).ExcludeYAxis().normalized;
            }

            return LookDirection;
        }

        public override void SubscribeToInputActions()
        {
            playerInput.actions["Player/Look"].performed += OnLook;
            playerInput.actions["Player/MouseLook"].performed += MouseLook;
        }

        public override void UnsubscribeFromInputActions()
        {
            playerInput.actions["Player/Look"].performed -= OnLook;
            playerInput.actions["Player/MouseLook"].performed -= MouseLook;
        }

        private void MouseLook(InputAction.CallbackContext ctx)
        {
            UpdateLookDirectionMouse(ctx.ReadValue<Vector2>());
            mouseUsedLast = true;
        }

        private void UpdateLookDirectionMouse(Vector2 mouseControl)
        {
            Ray lookRay = lookCamera.ScreenPointToRay(mouseControl);

            mousePoint = GetMousePoint(lookRay);

            LookDirection = (mousePoint - transform.position).ExcludeYAxis().normalized;
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
            mouseUsedLast = false;
        }
    }
}