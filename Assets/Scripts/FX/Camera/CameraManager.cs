using UnityEngine;

using MemoryFishing.Gameplay;

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

        private void Start()
        {
            for (int i = 0; i < trackers.Length; i++)
            {
                trackers[i].Initialize(player.position, player.rotation, bobber.position);
            }
        }

        private void Update()
        {
            for (int i = 0; i < trackers.Length; i++)
            {
                trackers[i].UpdatePosition(player.position, player.rotation, bobber.position, Time.deltaTime);
            }
        }
    }
}