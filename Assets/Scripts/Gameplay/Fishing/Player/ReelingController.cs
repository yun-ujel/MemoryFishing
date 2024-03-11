using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Gameplay.Fishing.Fish;
using MemoryFishing.Gameplay.Player;

namespace MemoryFishing.Gameplay.Fishing.Player
{
    public class ReelingController : PlayerController
    {
        [SerializeField] private PlayerDirection player;
        [Space, SerializeField] private FishBehaviour startFish;

        private bool reeling;

        private FishBehaviour currentFish;
        [SerializeField, Range(0f, 1f)] private float fishExhaustion;

        public override void Start()
        {
            base.Start();

            StartReeling(startFish);
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

            fishExhaustion = 0;

            fish.InitiateFishing();
        }

        private void UpdateReeling()
        {
            _ = currentFish.UpdateFishDirection(Time.deltaTime);
            fishExhaustion += currentFish.UpdateExhaustion(Time.deltaTime, -player.LookDirection);

            Debug.DrawRay(transform.position, currentFish.GetFishDirection(), Color.red);
            Debug.DrawRay(transform.position, player.LookDirection, Color.green);

            Debug.DrawRay(transform.position, Vector3.forward * fishExhaustion, Color.blue);

            if (fishExhaustion > 1f)
            {
                EndReeling();
            }
        }

        private void EndReeling()
        {
            reeling = false;
            currentFish = null;
        }
    }
}