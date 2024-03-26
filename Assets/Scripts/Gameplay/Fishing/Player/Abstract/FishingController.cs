using UnityEngine;
using MemoryFishing.Gameplay.Enumerations;

namespace MemoryFishing.Gameplay.Fishing.Player
{
    [RequireComponent(typeof(PlayerFishingManager))]
    public class FishingController : PlayerController
    {
        protected PlayerFishingManager fishingManager;
        protected FishingState State
        {
            get => fishingManager.State;
            set => fishingManager.State = value;
        }
        protected Vector3 BobberPos
        {
            get => fishingManager.BobberPos;
            set => fishingManager.BobberPos = value;
        }


        public override void Start()
        {
            base.Start();

            fishingManager = GetComponent<PlayerFishingManager>();
        }
    }
}