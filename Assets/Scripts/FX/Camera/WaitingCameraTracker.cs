using UnityEngine;

using Cinemachine;

using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;
using MemoryFishing.Gameplay;

namespace MemoryFishing.FX.Camera
{
    public class WaitingCameraTracker : CameraTracker
    {
        [Header("Position")]
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

        private float approachTime;
        private float approachTimeCounter;

        private bool fishApproaching;

        public override void Initialize(PlayerManager playerManager, PlayerFishingManager fishingManager, Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos)
        {
            base.Initialize(playerManager, fishingManager, playerPos, playerRotation, bobberPos);

            fishingManager.OnFishApproachingEvent += FishApproaching;
        }

        private void FishApproaching(object sender, OnFishApproachingEventArgs args)
        {
            Debug.Log("Approaching");

            fishApproaching = true;
            approachTime = args.ApproachTime;
        }

        public override void OnInitialSwitch(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos)
        {
            Debug.Log("Switch");
            fishApproaching = false;

            Vector3 target = playerPos;
            target += Quaternion.Euler(angle) * -offset;

            transform.position = target;

            SetTargetPosition(target);
        }

        public override void UpdatePosition(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos, float delta)
        {
            Vector3 target = Vector3.Lerp(playerPos, bobberPos, distanceFromPlayer);

            if (targetPosition != target)
            {
                SetTargetPosition(target);
            }

            if (fishApproaching)
            {
                approachTimeCounter += delta;

                float t = approachTimeCounter / approachTime;

                target = Vector3.Lerp(target, bobberPos, t);
            }

            target += Quaternion.Euler(angle) * -offset;

            float curveT = transitionCurve.Evaluate(t) / transitionDuration;
            transform.position = Vector3.Lerp(startPosition, target, curveT);

            t += delta;
        }
        private void SetTargetPosition(Vector3 target)
        {
            startPosition = transform.position;
            targetPosition = target;
            t = 0f;

            Debug.DrawRay(targetPosition, Vector3.up, Color.yellow, 1f);
        }
    }
}