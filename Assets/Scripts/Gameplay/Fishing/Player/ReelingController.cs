using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Gameplay.Fishing.Fish;

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
            public FishBehaviour FishBehaviour { get; private set; }
            public Vector3 PlayerPos { get; private set; }
            public Vector3 FishPos { get; private set; }

            public OnEndReelingEventArgs(FishBehaviour fishBehaviour, Vector3 playerPos, Vector3 fishPos)
            {
                FishBehaviour = fishBehaviour;
                PlayerPos = playerPos;
                FishPos = fishPos;
            }
        }

        #region Properties

        [SerializeField] private PlayerDirection direction;

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

            //StartReeling(startFish);
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
                currentFish.MoveFishReeling(Time.fixedDeltaTime);
            }
        }

        public void StartReeling(FishBehaviour fish)
        {
            currentFish = fish;
            reeling = true;

            FishExhaustion = 0;

            Vector3 playerPos = transform.position;
            startingFishPosition = fish.transform.position;

            fish.InitiateReeling(playerPos, startingFishPosition);
            OnStartReelingEvent?.Invoke(this, new OnStartReelingEventArgs(fish, playerPos, startingFishPosition));
        }

        private void UpdateReeling()
        {
            currentFish.UpdateFishReeling(Time.deltaTime);
            Vector3 lookDirection = direction.GetLookDirection(currentFish.transform.position);

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
        }

        private void EndReeling()
        {
            currentFish.StopReeling();
            OnEndReelingEvent?.Invoke(this, new(currentFish, transform.position, currentFish.transform.position));

            reeling = false;
            currentFish = null;
        }
    }
}