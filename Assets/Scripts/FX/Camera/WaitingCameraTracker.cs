using UnityEngine;

using Cinemachine;

namespace MemoryFishing.FX.Camera
{
    public class WaitingCameraTracker : CameraTracker
    {
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, 30f);
        [SerializeField] private Vector3 angle = new Vector3(60f, 0f, 0f);

        public override void UpdatePosition(Vector3 playerPos, Quaternion playerRotation, Vector3 bobberPos, float delta)
        {
            Vector3 position = playerPos;
            position += Quaternion.Euler(angle) * -offset;

            transform.position = position;
        }
    }
}