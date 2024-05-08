using UnityEngine;

using Cinemachine;
using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay;
using MemoryFishing.Gameplay.Enumerations;

namespace MemoryFishing.FX.Camera
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public abstract class CameraTracker : MonoBehaviour
    {
        public int Priority
        {
            get => virtualCam.Priority;
            set => virtualCam.Priority = value;
        }

        protected CinemachineVirtualCamera virtualCam;

        public virtual void Initialize(PlayerManager playerManager, PlayerFishingManager fishingManager, Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos)
        {
            virtualCam = GetComponent<CinemachineVirtualCamera>();
        }

        public virtual void OnInitialSwitch(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos)
        {

        }

        public abstract void UpdatePosition(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos, float delta);

        public abstract bool TryTrackingConditions(PlayerState playerState, FishingState fishingState);
    }
}