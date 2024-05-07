using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MemoryFishing.FX
{
    public class FollowPosition : MonoBehaviour
    {
        [SerializeField] private Transform follow;

        [Space]

        [Header("Track Position")]
        [SerializeField] private bool trackXPosition;
        [SerializeField] private bool trackYPosition;
        [SerializeField] private bool trackZPosition;

        private bool TrackPosition => trackXPosition || trackYPosition || trackZPosition;
        private Vector3 TargetPos => follow.position;

        [Space, SerializeField] private Vector3 positionOffset;

        [Header("Track Rotation")]
        [SerializeField] private bool trackXRotation;
        [SerializeField] private bool trackYRotation;
        [SerializeField] private bool trackZRotation;

        private bool TrackRotation => trackXRotation || trackYRotation || trackZRotation;
        private Vector3 TargetEulers => follow.rotation.eulerAngles;

        [Space, SerializeField] private Vector3 rotationOffset;

        private void Update()
        {
            if (TrackPosition)
            {
                Vector3 pos = positionOffset;

                if (trackXPosition) { pos.x += TargetPos.x; }
                if (trackYPosition) { pos.y += TargetPos.y; }
                if (trackZPosition) { pos.z += TargetPos.z; }

                transform.position = pos;
            }

            if (TrackRotation)
            {
                Vector3 euler = rotationOffset;

                if (trackXRotation) { euler.x += TargetEulers.x; }
                if (trackYRotation) { euler.y += TargetEulers.y; }
                if (trackZRotation) { euler.z += TargetEulers.z; }

                transform.rotation = Quaternion.Euler(euler);
            }
        }
    }
}