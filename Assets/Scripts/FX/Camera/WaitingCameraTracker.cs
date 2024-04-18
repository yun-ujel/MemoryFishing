using UnityEngine;

using Cinemachine;

namespace MemoryFishing.FX.Camera
{
    public class WaitingCameraTracker : CameraTracker
    {
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, 30f);
        [SerializeField] private Vector3 angle = new Vector3(60f, 0f, 0f);

        [Space]

        [SerializeField, Range(0f, 1f)] private float distanceFromPlayer = 0.7f;

        [Header("Lerp Settings")]
        [SerializeField] private AnimationCurve transitionCurve;
        [SerializeField, Range(0f, 1f)] private float transitionDuration = 0.5f;

        private Vector3 targetPosition;
        private Vector3 startPosition;
        private float t;

        public override void UpdatePosition(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos, float delta)
        {
            Vector3 target = Vector3.Lerp(playerPos, bobberPos, distanceFromPlayer);
            target += Quaternion.Euler(angle) * -offset;

            if (targetPosition != target)
            {
                startPosition = targetPosition;
                targetPosition = target;
                t = 0f;
            }

            float curveT = transitionCurve.Evaluate(t) / transitionDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, curveT);

            t += delta;
        }
    }
}