using MemoryFishing.Gameplay.Enumerations;
using UnityEngine;

namespace MemoryFishing.FX.Camera
{
    public class FightingCameraTracker : CameraTracker
    {
        [Header("Smoothing")]
        [SerializeField] private float speed = 10f;

        public override void UpdatePosition(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos, float delta)
        {
            Vector3 target = bobberPos;
            target += Quaternion.Euler(angle) * -offset;

            transform.position = Vector3.MoveTowards(transform.position, target, speed * delta);
        }

        public override void OnInitialSwitch(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos)
        {
            transform.position = bobberPos + (Quaternion.Euler(angle) * -offset);
        }

        public override bool TryTrackingConditions(PlayerState playerState, FishingState fishingState)
        {
            return fishingState == FishingState.Fighting;
        }
    }
}