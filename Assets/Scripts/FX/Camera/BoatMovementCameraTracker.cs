using UnityEngine;

using MemoryFishing.Gameplay.Enumerations;

namespace MemoryFishing.FX.Camera
{
    public class BoatMovementCameraTracker : CameraTracker
    {
        public override bool TryTrackingConditions(PlayerState playerState, FishingState fishingState)
        {
            return playerState == PlayerState.Boat;
        }

        public override void UpdatePosition(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos, float delta)
        {
            Vector3 position = playerPos;

            position += Quaternion.Euler(angle) * -offset;

            transform.position = position;
        }
    }
}