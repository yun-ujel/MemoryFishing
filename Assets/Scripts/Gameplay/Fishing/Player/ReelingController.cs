using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Gameplay.Fishing.Fish;

namespace MemoryFishing.Gameplay.Fishing.Player
{
    public class ReelingController : PlayerController
    {
        private FishBehaviour currentFish;
        private bool reeling;

        public override void SubscribeToInputActions()
        {

        }

        private void Update()
        {
            if (reeling)
            {
                UpdateReeling();
            }
        }

        private void StartReeling(FishBehaviour fish)
        {
            currentFish = fish;
            reeling = true;

            fish.InitiateFishing();
        }

        private void UpdateReeling()
        {

        }

        private void EndReeling()
        {
            reeling = false;
            currentFish = null;
        }
    }
}