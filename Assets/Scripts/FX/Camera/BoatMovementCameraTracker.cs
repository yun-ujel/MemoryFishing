using UnityEngine;

using MemoryFishing.Gameplay.Enumerations;

namespace MemoryFishing.FX.Camera
{
    public class BoatMovementCameraTracker : CameraTracker
    {
        [Header("Position")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, 30f);
        [SerializeField] private Vector3 angle = new Vector3(60f, 0f, 0f);

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