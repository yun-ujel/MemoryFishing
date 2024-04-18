using UnityEngine;

namespace MemoryFishing.FX.Camera
{
    public class FightingCameraTracker : CameraTracker
    {
        [Header("Position")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, 30f);
        [SerializeField] private Vector3 angle = new Vector3(60f, 0f, 0f);

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
    }
}