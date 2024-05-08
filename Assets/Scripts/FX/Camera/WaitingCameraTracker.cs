using UnityEngine;

using Cinemachine;

using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;
using MemoryFishing.Gameplay;
using MemoryFishing.Gameplay.Enumerations;

namespace MemoryFishing.FX.Camera
{
    public class WaitingCameraTracker : CameraTracker
    {
        [Header("Position")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, 30f);
        [SerializeField] private Vector3 angle = new Vector3(60f, 0f, 0f);

        [Space]

        [SerializeField, Range(0f, 1f)] private float distanceFromPlayer = 0.7f;

        [Header("Smoothing")]
        [SerializeField] private float smoothing = 2f;
        [SerializeField] private float approachingSmoothing;

        private Vector3 currentPlayerOffset;

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
        }

        public override void OnInitialSwitch(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos)
        {
            Debug.Log("Switch");
            fishApproaching = false;
        }

        public override void UpdatePosition(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos, float delta)
        {
            Vector3 direction = bobberPos - playerPos;
            Vector3 targetOffset = Vector3.Lerp(Vector3.zero, direction, distanceFromPlayer);
            currentPlayerOffset = Vector3.MoveTowards(currentPlayerOffset, targetOffset, delta * smoothing);

            if (fishApproaching)
            {
                currentPlayerOffset = Vector3.MoveTowards(currentPlayerOffset, direction, delta * approachingSmoothing);
            }

            Vector3 cameraPos = playerPos + currentPlayerOffset;
            SetCameraPosition(cameraPos);
        }

        private void SetCameraPosition(Vector3 cameraPos)
        {
            transform.position = cameraPos + Quaternion.Euler(angle) * -offset;
        }

        public override bool TryTrackingConditions(PlayerState playerState, FishingState fishingState)
        {
            return playerState == PlayerState.Fishing && fishingState != FishingState.Exhausted && fishingState != FishingState.Reeling && fishingState != FishingState.Fighting;
        }
    }
}