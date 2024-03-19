using UnityEngine;

using MemoryFishing.Gameplay.Fishing.Enumerations;

namespace MemoryFishing.Gameplay.Fishing.Player
{
    public class PlayerFishingManager : MonoBehaviour
    {
        [Space, SerializeField] private Transform bobber;
        public FishingState State { get; set; } = FishingState.None;
        public Vector3 BobberPos
        {
            get => bobber.position;
            set => bobber.position = value;
        }
    }
}