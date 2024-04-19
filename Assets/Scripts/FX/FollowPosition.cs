using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MemoryFishing.FX
{
    public class FollowPosition : MonoBehaviour
    {
        [SerializeField] private Transform follow;

        [Space]

        [SerializeField] private bool trackPosition;
        [SerializeField] private bool trackRotation;

        [Space]

        [SerializeField] private Vector3 offset;

        private void Update()
        {
            if (trackPosition)
            {
                Vector3 pos = follow.position + offset;
                transform.position = pos;
            }

            if (trackRotation)
            {
                transform.rotation = follow.rotation;
            }
        }
    }
}