using UnityEngine;

using MemoryFishing.Gameplay;
using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.FX.Camera.Enumerations;
using MemoryFishing.Gameplay.Enumerations;

namespace MemoryFishing.FX.Camera
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CameraTracker[] trackers;

        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private Transform bobber;

        [Space]

        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private PlayerFishingManager fishingManager;

        private CameraState cameraState;

        private void Start()
        {
            for (int i = 0; i < trackers.Length; i++)
            {
                trackers[i].Initialize(player.position, player.rotation, bobber.position);
            }
        }

        private void Update()
        {
            UpdatePriority();

            for (int i = 0; i < trackers.Length; i++)
            {
                trackers[i].UpdatePosition(player.position, player.rotation, bobber.position, Time.deltaTime);
            }
        }

        private void UpdatePriority()
        {
            CameraState state = GetState(playerManager.State, fishingManager.State);

            if ((int)state >= trackers.Length)
            {
                return;
            }

            for (int i = 0; i < trackers.Length; i++)
            {
                if (i == (int)state)
                {
                    trackers[i].Priority = 1;
                    continue;
                }

                trackers[i].Priority = 0;
            }

            cameraState = state;
        }

        private CameraState GetState(PlayerState playerState, FishingState fishingState)
        {
            if (playerState == PlayerState.Boat)
            {
                return CameraState.BoatMoving;
            }

            switch (fishingState)
            {
                case FishingState.Fighting:
                    return CameraState.Fighting;

                case FishingState.Reeling:
                    return CameraState.Reeling;

                default:
                    return CameraState.Waiting;
            }
        }
    }
}