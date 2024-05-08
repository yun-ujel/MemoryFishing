using UnityEngine;

using Cinemachine;

using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;
using MemoryFishing.Gameplay;
using MemoryFishing.Gameplay.Enumerations;

namespace MemoryFishing.FX.Camera
{
    public class IdleFishingCameraTracker : CameraTracker
    {
        [Header("Position")]
        [SerializeField] private Vector3 offset = new(0f, 0f, 30f);
        [SerializeField] private Vector3 angle = new(60f, 0f, 0f);

        [Header("Smoothing")]
        [SerializeField] private float speed = 10f;

        public override void UpdatePosition(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos, float delta)
        {
            Vector3 target = playerPos;
            target += Quaternion.Euler(angle) * -offset;

            transform.position = Vector3.MoveTowards(transform.position, target, speed * delta);
        }

        public override void OnInitialSwitch(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos)
        {
            transform.position = playerPos + (Quaternion.Euler(angle) * -offset);
        }

        public override bool TryTrackingConditions(PlayerState playerState, FishingState fishingState)
        {
            bool fishingMatch = fishingState == FishingState.WindUp || fishingState == FishingState.None;

            return fishingMatch && playerState == PlayerState.Fishing;
        }
    }
}