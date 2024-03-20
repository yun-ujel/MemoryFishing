using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Gameplay.Fishing.Fish;
using MemoryFishing.Gameplay.Fishing.Enumerations;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;

namespace MemoryFishing.Gameplay.Fishing.Player
{
    public partial class ReelingController : FishingController
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
            if (State == FishingState.Reeling)
            {
                UpdateReeling();
            }
        }

        private void FixedUpdate()
        {
            if (State == FishingState.Reeling)
            {
                currentFish.MoveFishReeling(Time.fixedDeltaTime);
            }
        }

        public void StartReeling(FishBehaviour fish)
        {
            currentFish = fish;
            State = FishingState.Reeling;

            FishExhaustion = 0;

            Vector3 playerPos = transform.position;
            startingFishPosition = fish.transform.position;

            fish.InitiateReeling(playerPos, startingFishPosition);
            fishingManager.StartReelingEvent(new OnStartReelingEventArgs(fish, playerPos, startingFishPosition));
        }

        private void UpdateReeling()
        {
            currentFish.UpdateFishReeling(Time.deltaTime);
            Vector3 lookDirection = direction.GetLookDirection(currentFish.transform.position);

            FishExhaustion += currentFish.UpdateFishExhaustion(Time.deltaTime, -lookDirection);
            FishExhaustion = Mathf.Clamp01(FishExhaustion);
            
            DrawDirections(currentFish.transform.position, lookDirection);
            BobberPos = currentFish.transform.position;

            if (FishExhaustion >= 1f)
            {
                EndReeling();
            }
        }

        private void DrawDirections(Vector3 fishPos, Vector3 playerDirection)
        {
            Debug.DrawRay(fishPos, currentFish.GetFishDirection(), Color.red);
            Debug.DrawRay(fishPos, playerDirection, Color.green);
        }

        private void EndReeling()
        {
            currentFish.StopReeling();
            fishingManager.EndReelingEvent(new(currentFish, transform.position, currentFish.transform.position));

            currentFish = null;
        }
    }
}