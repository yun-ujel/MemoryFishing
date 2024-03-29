using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Gameplay.Fishing.Player.EventArgs;
using MemoryFishing.Gameplay.Enumerations;
using MemoryFishing.Gameplay.Fishing.Fish;

using static MemoryFishing.Utilities.VectorUtils;
using MemoryFishing.Utilities;

namespace MemoryFishing.Gameplay.Fishing.Player
{
    public class ReelingController : FishingController
    {
        [SerializeField] private PlayerDirection direction;
        [SerializeField] private FishFightController fightController;

        [Header("Settings")]
        [SerializeField, Range(0f, 10f)] private float catchFishRadius = 2f;

        [Space]

        [SerializeField, Range(0f, 10f)] private float timeToReelFully = 2f;

        [Space]

        [SerializeField, Range(0f, 10f)] private float reelAcceleration = 10f;
        [SerializeField, Range(0f, 10f)] private float reelDeceleration = 1f;

        private FishBehaviour fish;
        private Rigidbody fishBody;

        private bool reelHeld;

        private float startingDistance;

        private float maxReelSpeed;
        private float currentReelSpeed;

        private float fishReawakenDuration;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, catchFishRadius);
        }
#endif

        public override void Start()
        {
            base.Start();

            fishingManager.OnEndFightingEvent += EndFighting;
        }

        public override void SubscribeToInputActions()
        {
            base.SubscribeToInputActions();

            playerInput.actions["Player/CastReel"].performed += ReelPressedInput;
            playerInput.actions["Player/CastReel"].canceled += ReelPressedInput;
        }

        #region Event Methods

        private void ReelPressedInput(InputAction.CallbackContext ctx)
        {
            reelHeld = ctx.performed;

            if (State != FishingState.Exhausted && State != FishingState.Reeling)
            {
                return;
            }

            if (reelHeld)
            {
                State = FishingState.Reeling;
                return;
            }

            State = FishingState.Exhausted;
        }

        private void EndFighting(object sender, OnEndFightingEventArgs args)
        {
            State = FishingState.Exhausted;

            if (args.FishBehaviour != fish)
            {
                fish = args.FishBehaviour;
                fishBody = fish.GetComponent<Rigidbody>();
            }
            
            currentReelSpeed = fishBody.velocity.magnitude;

            if (args.FirstFight)
            {
                startingDistance = Vector3.Distance(args.PlayerPos, args.FishPos);
                maxReelSpeed = startingDistance / timeToReelFully;
            }
        }

        #endregion

        #region Private Methods

        private void FixedUpdate()
        {
            UpdateFishVelocity(Time.fixedDeltaTime);
        }

        private void Update()
        {
            if (State != FishingState.Exhausted && State != FishingState.Reeling)
            {
                return;
            }

            BobberPos = fishBody.position;
        }

        private void UpdateFishVelocity(float delta)
        {
            if (State != FishingState.Exhausted && State != FishingState.Reeling)
            {
                return;
            }

            (transform.position - fishBody.position).DirectionMagnitude(out Vector3 direction, out float distance);

            fishReawakenDuration = fish.UpdateReawakenDuration(startingDistance, distance, delta);

            if (fishReawakenDuration <= 0f)
            {
                fightController.StartFighting(fish, false);
                return;
            }

            if (State == FishingState.Reeling)
            {
                currentReelSpeed = Mathf.MoveTowards(currentReelSpeed, maxReelSpeed, delta * maxReelSpeed * reelAcceleration);
            }

            if (State == FishingState.Exhausted)
            {
                currentReelSpeed = Mathf.MoveTowards(currentReelSpeed, 0, delta * maxReelSpeed * reelDeceleration);
            }

            fishBody.velocity = direction * currentReelSpeed;

            if (FishInRange(transform.position, fishBody.position))
            {
                CatchFish();
                return;
            }
        }

        private void CatchFish()
        {
            fishBody.velocity = Vector3.zero;
            State = FishingState.None;

            fishingManager.CatchFishEvent(new(fish.GetItem(), fish, fishBody.position));
        }

        private bool FishInRange(Vector3 playerPosition, Vector3 fishPosition)
        {
            float sqrDistance = VectorUtils.SqrDistance(playerPosition.ExcludeYAxis(), fishPosition.ExcludeYAxis());

            if (sqrDistance < Mathf.Pow(catchFishRadius, 2))
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}