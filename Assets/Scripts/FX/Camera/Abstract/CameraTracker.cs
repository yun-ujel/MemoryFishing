using UnityEngine;

using Cinemachine;

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

        public virtual void Initialize(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos)
        {
            virtualCam = GetComponent<CinemachineVirtualCamera>();
        }

        public abstract void UpdatePosition(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos, float delta);
    }
}