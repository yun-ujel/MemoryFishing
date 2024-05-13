using MemoryFishing.Gameplay.Enumerations;
using UnityEngine;

namespace MemoryFishing.FX.Camera
{
    public class StartCutsceneCameraTracker : CameraTracker
    {
        public bool UseTracker { get; set; } = true;

        public override void UpdatePosition(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos, float delta)
        {
            transform.position = Vector3.zero + (Quaternion.Euler(angle) * -offset);
        }

        public override bool TryTrackingConditions(PlayerState playerState, FishingState fishingState)
        {
            return UseTracker;
        }
    }
}