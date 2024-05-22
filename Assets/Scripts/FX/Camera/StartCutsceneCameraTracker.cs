using MemoryFishing.Gameplay.Enumerations;
using UnityEngine;

namespace MemoryFishing.FX.Camera
{
    public class StartCutsceneCameraTracker : CameraTracker
    {
        public bool UseTracker { get; set; } = true;
        public Vector3 Target { get; set; }

        public override void UpdatePosition(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos, float delta)
        {
            transform.position = Target + (Quaternion.Euler(angle) * -offset);
            Debug.DrawLine(transform.position, Target);
        }

        public override bool TryTrackingConditions(PlayerState playerState, FishingState fishingState)
        {
            return UseTracker;
        }
    }
}