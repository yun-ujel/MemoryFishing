using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Gameplay.Fishing.Fish;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;
using MemoryFishing.Gameplay.Enumerations;

namespace MemoryFishing.Gameplay.Fishing.Player
{
    public class FishFightController : FishingController
    {
        #region Properties
        public float FishExhaustion { get; private set; }

        [Space, SerializeField] private PlayerDirection direction;

        private FishBehaviour currentFish;

        private Vector3 startingFishPosition;

        #endregion

        public override void Start()
        {
            base.Start();
        }

        private void Update()
        {
            if (State == FishingState.Fighting)
            {
                UpdateFighting();
            }
        }

        private void FixedUpdate()
        {
            if (State == FishingState.Fighting)
            {
                currentFish.MoveFishFighting(Time.fixedDeltaTime);
            }
        }

        public void StartFighting(FishBehaviour fish)
        {
            currentFish = fish;
            State = FishingState.Fighting;

            FishExhaustion = 0;

            Vector3 playerPos = transform.position;
            startingFishPosition = fish.transform.position;

            fish.InitiateFighting(playerPos, startingFishPosition);
            fishingManager.StartFightingEvent(new OnStartFightingEventArgs(fish, playerPos, startingFishPosition));
        }

        private void UpdateFighting()
        {
            currentFish.UpdateFishFighting(Time.deltaTime);
            Vector3 lookDirection = direction.GetLookDirection(currentFish.transform.position);

            FishExhaustion += currentFish.UpdateFishExhaustion(Time.deltaTime, -lookDirection);
            FishExhaustion = Mathf.Clamp01(FishExhaustion);
            
            DrawDirections(currentFish.transform.position, lookDirection);
            BobberPos = currentFish.transform.position;

            if (FishExhaustion >= 1f)
            {
                EndFighting();
            }
        }

        private void DrawDirections(Vector3 fishPos, Vector3 playerDirection)
        {
            Debug.DrawRay(fishPos, currentFish.GetFishDirection(), Color.red);
            Debug.DrawRay(fishPos, playerDirection, Color.green);
        }

        private void EndFighting()
        {
            currentFish.StopFighting();
            fishingManager.EndFightingEvent(new(currentFish, transform.position, currentFish.transform.position));

            currentFish = null;
        }
    }
}