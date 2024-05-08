using MemoryFishing.Gameplay.Enumerations;
using UnityEngine;

namespace MemoryFishing.FX.Camera
{
    public class WaitingCameraTracker : CameraTracker
    {
        [Header("Position")]
        [SerializeField] private Vector3 offset = new(0f, 0f, 30f);
        [SerializeField] private Vector3 angle = new(60f, 0f, 0f);

        public override void UpdatePosition(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos, float delta)
        {
            transform.position = bobberPos + (Quaternion.Euler(angle) * -offset);
        }

        public override void OnInitialSwitch(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos)
        {
            transform.position = bobberPos + (Quaternion.Euler(angle) * -offset);
        }

        public override bool TryTrackingConditions(PlayerState playerState, FishingState fishingState)
        {
            return fishingState == FishingState.Waiting;
        }
    }
}