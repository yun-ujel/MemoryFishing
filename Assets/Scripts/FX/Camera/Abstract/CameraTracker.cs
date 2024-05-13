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
            get => VirtualCam.Priority;
            set => VirtualCam.Priority = value;
        }

        public CinemachineVirtualCamera VirtualCam { get; private set; }

        [Header("Position")]
        [SerializeField] protected Vector3 offset = new Vector3(0f, 0f, 30f);
        [SerializeField] protected Vector3 angle = new Vector3(60f, 0f, 0f);

        public virtual void Initialize(PlayerManager playerManager, PlayerFishingManager fishingManager, Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos)
        {
            VirtualCam = GetComponent<CinemachineVirtualCamera>();
        }

        public virtual void OnInitialSwitch(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos)
        {

        }

        public abstract void UpdatePosition(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos, float delta);

        public abstract bool TryTrackingConditions(PlayerState playerState, FishingState fishingState);
    }
}