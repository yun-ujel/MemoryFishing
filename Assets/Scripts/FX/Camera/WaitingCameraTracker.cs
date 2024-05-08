using MemoryFishing.Gameplay.Enumerations;
using UnityEngine;

namespace MemoryFishing.FX.Camera
{
    public class WaitingCameraTracker : CameraTracker
    {
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