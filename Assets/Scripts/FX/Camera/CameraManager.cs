using UnityEngine;

using MemoryFishing.Gameplay;
using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay.Enumerations;
using MemoryFishing.Utilities;

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

        [SerializeField] private int cameraState;

        private void Start()
        {
            for (int i = 0; i < trackers.Length; i++)
            {
                trackers[i].Initialize(playerManager, fishingManager, player.position, player.rotation, bobber.position);
            }

            SetState(0);
        }

        private void Update()
        {
            UpdatePriority();

            trackers[cameraState].UpdatePosition(player.position, player.rotation, bobber.position, Time.deltaTime);
        }

        private void UpdatePriority()
        {
            int state = GetState(playerManager.State, fishingManager.State);

            if (!state.IsInRangeOf(trackers) || state == cameraState)
            {
                return;
            }
            
            SetState(state);
        }

        private void SetState(int state)
        {
            for (int i = 0; i < trackers.Length; i++)
            {
                if (i == state)
                {
                    trackers[i].OnInitialSwitch(player.position, player.rotation, bobber.position);
                    trackers[i].Priority = 1;
                    continue;
                }

                trackers[i].Priority = 0;
            }

            cameraState = state;
        }

        private int GetState(PlayerState playerState, FishingState fishingState)
        {
            for (int i = 0; i < trackers.Length; i++)
            {
                if (trackers[i].TryTrackingConditions(playerState, fishingState))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}