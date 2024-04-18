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

        private Vector3 targetPlayerOffset;
        private Vector3 startPlayerOffset;

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
        }

        public override void UpdatePosition(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos, float delta)
        {
            Vector3 direction = bobberPos - playerPos;
            Vector3 playerOffset = Vector3.Lerp(Vector3.zero, direction, distanceFromPlayer);

            float curveT = transitionCurve.Evaluate(t) / transitionDuration;

            if (targetPlayerOffset != playerOffset)
            {
                startPlayerOffset = Vector3.Lerp(Vector3.zero, targetPlayerOffset, curveT);
                targetPlayerOffset = playerOffset;

                t = 0f;
                curveT = 0f;
            }

            if (fishApproaching)
            {
                approachTimeCounter += Time.deltaTime;
                float fishT = approachTimeCounter / approachTime;
                fishT *= 1 - distanceFromPlayer;

                playerOffset = Vector3.Lerp(Vector3.zero, direction, distanceFromPlayer + fishT);
            }

            Vector3 cameraPos = playerPos + Vector3.Lerp(startPlayerOffset, playerOffset, curveT);

            SetCameraPosition(delta, cameraPos);
        }

        private void SetCameraPosition(float delta, Vector3 cameraPos)
        {
            transform.position = cameraPos + Quaternion.Euler(angle) * -offset;
            t += delta;
        }
    }
}