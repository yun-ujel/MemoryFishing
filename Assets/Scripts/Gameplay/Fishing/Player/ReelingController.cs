using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Gameplay.Fishing.Fish;
using MemoryFishing.Gameplay.Player;

namespace MemoryFishing.Gameplay.Fishing.Player
{
    public class ReelingController : PlayerController
    {
        public class OnStartReelingEventArgs : System.EventArgs
        {
            public FishBehaviour FishBehaviour { get; private set; }
            public Vector3 PlayerPos { get; private set; }
            public Vector3 FishPos { get; private set; }

            public OnStartReelingEventArgs(FishBehaviour fishBehaviour, Vector3 playerPos, Vector3 fishPos)
            {
                FishBehaviour = fishBehaviour;
                PlayerPos = playerPos;
                FishPos = fishPos;
            }
        }

        #region Properties

        [SerializeField] private PlayerDirection player;
        [Space, SerializeField] private FishBehaviour startFish;
        [SerializeField, Range(0f, 1f)] private float fishExhaustion;

        public event System.EventHandler<OnStartReelingEventArgs> OnStartReelingEvent;

        private bool reeling;
        private FishBehaviour currentFish;

        #endregion
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

            Vector3 playerPos = transform.position;
            Vector3 fishPos = fish.transform.position;

            fish.InitiateFishing(playerPos, fishPos);
            OnStartReelingEvent?.Invoke(this, new OnStartReelingEventArgs(fish, playerPos, fishPos));
        }

        private void UpdateReeling()
        {
            _ = currentFish.UpdateFishDirection(Time.deltaTime);
            fishExhaustion += currentFish.UpdateExhaustion(Time.deltaTime, -player.LookDirection);

            Debug.DrawRay(currentFish.transform.position, currentFish.GetFishDirection(), Color.red);
            Debug.DrawRay(currentFish.transform.position, player.LookDirection, Color.green);

            Debug.DrawRay(currentFish.transform.position + new Vector3(-1, 0, 2), 2 * fishExhaustion * Vector3.right);

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