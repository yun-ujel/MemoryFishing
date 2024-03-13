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

        public class OnEndReelingEventArgs : System.EventArgs
        {
            public OnEndReelingEventArgs()
            {

            }
        }

        #region Properties

        [SerializeField] private PlayerDirection player;
        [Space, SerializeField] private FishBehaviour startFish;

        public event System.EventHandler<OnStartReelingEventArgs> OnStartReelingEvent;
        public event System.EventHandler<OnEndReelingEventArgs> OnEndReelingEvent;
        public float FishExhaustion { get; private set; }

        private bool reeling;
        private FishBehaviour currentFish;

        private Vector3 startingFishPosition;

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

        private void FixedUpdate()
        {
            if (reeling)
            {
                currentFish.MoveFish(Time.fixedDeltaTime);
            }
        }

        private void StartReeling(FishBehaviour fish)
        {
            currentFish = fish;
            reeling = true;

            FishExhaustion = 0;

            Vector3 playerPos = transform.position;
            startingFishPosition = fish.transform.position;

            fish.InitiateFishing(playerPos, startingFishPosition);
            OnStartReelingEvent?.Invoke(this, new OnStartReelingEventArgs(fish, playerPos, startingFishPosition));
        }

        private void UpdateReeling()
        {
            currentFish.UpdateFish(Time.deltaTime);
            Vector3 lookDirection = player.GetLookDirection(currentFish.transform.position);

            FishExhaustion += currentFish.UpdateFishExhaustion(Time.deltaTime, -lookDirection);
            FishExhaustion = Mathf.Clamp01(FishExhaustion);
            
            DrawDirections(currentFish.transform.position, lookDirection);

            if (FishExhaustion >= 1f)
            {
                EndReeling();
            }
        }

        private void DrawDirections(Vector3 fishPos, Vector3 playerDirection)
        {
            Debug.DrawRay(fishPos, currentFish.GetFishDirection(), Color.red);
            Debug.DrawRay(fishPos, playerDirection, Color.green);

            //Debug.DrawRay(fishPos + new Vector3(-1, 0, 2), 2 * FishExhaustion * Vector3.right);
        }

        private void EndReeling()
        {
            currentFish.StopFishing();
            OnEndReelingEvent?.Invoke(this, new());

            reeling = false;
            currentFish = null;
        }
    }
}