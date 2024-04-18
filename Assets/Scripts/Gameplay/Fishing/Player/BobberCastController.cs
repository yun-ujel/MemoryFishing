using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Gameplay.Fishing.Fish;
using MemoryFishing.Utilities;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;
using MemoryFishing.Gameplay.Enumerations;

namespace MemoryFishing.Gameplay.Fishing.Player
{
    public class BobberCastController : FishingController
    {
        [Header("References")]
        [SerializeField] private PlayerDirection direction;
        [SerializeField] private FishFightController fightController;

        [Header("Bobber Casting: Wind Up")]
        [SerializeField, Range(0.01f, 10f)] private float timeToWindUp;
        [SerializeField] private AnimationCurve windUpCurve;

        [Space, SerializeField] private float castDistance;
        [SerializeField] private float startingCastDistance;

        [Space, SerializeField, Range(0.01f, 10f)] private float timeToLand;
        [SerializeField, Range(0.01f, 10f)] private float timeToRecall;

        private Vector3 targetBobberPos;

        private Vector3 castDirection;
        private float castMagnitude;

        private float counter;

        private bool fishApproaching;

        private FishBehaviour approachingFish;
        private float fishApproachTime;

        public override void SubscribeToInputActions()
        {
            playerInput.actions["Player/CastReel"].performed += OnCastPressed;
            playerInput.actions["Player/CastReel"].canceled += OnCastReleased;
        }
        public override void UnsubscribeFromInputActions()
        {
            playerInput.actions["Player/CastReel"].performed -= OnCastPressed;
            playerInput.actions["Player/CastReel"].canceled -= OnCastReleased;
        }

        private void OnCastPressed(InputAction.CallbackContext ctx)
        {
            if (State == FishingState.None)
            {
                State = FishingState.WindUp;

                counter = 0f;
                castMagnitude = 0f;
                fishingManager.StartWindUpEvent(new(direction.GetLookDirection(transform.position),
                                                    timeToWindUp,
                                                    windUpCurve,
                                                    startingCastDistance,
                                                    castDistance));

                return;
            }

            if (State == FishingState.Waiting || State == FishingState.Casting)
            {
                RecallBobber();
            }
        }

        private void OnCastReleased(InputAction.CallbackContext ctx)
        {
            if (State == FishingState.WindUp)
            {
                CastBobber();
            }
        }

        protected override void OnEnableFishing(object sender, OnEnableFishingEventArgs args)
        {
            base.OnEnableFishing(sender, args);

            counter = 0f;
        }

        protected override void OnDisableFishing(object sender, OnEnableFishingEventArgs args)
        {
            base.OnDisableFishing(sender, args);
            
            counter = 0f;

            if (State == FishingState.Waiting || State == FishingState.Casting)
            {
                RecallBobber();
                return;
            }

            if (State == FishingState.WindUp)
            {
                State = FishingState.None;
                return;
            }
        }

        private void Update()
        {
            if (State == FishingState.WindUp)
            {
                castDirection = direction.GetLookDirection(transform.position);
                WindUpCast();
                return;
            }

            if (State == FishingState.Casting)
            {
                counter += Time.deltaTime;
                if (counter > timeToLand)
                {
                    BobberLanding();
                }
                return;
            }

            if (fishApproaching)
            {
                counter += Time.deltaTime;
                float t = counter / fishApproachTime;

                approachingFish.ApproachBobber(targetBobberPos, t);

                if (t >= 1f)
                {
                    fightController.StartFighting(approachingFish, true);
                    
                    fishApproaching = false;
                }

                return;
            }
        }

        private void WindUpCast()
        {
            counter += Time.deltaTime;
            float t = windUpCurve.Evaluate(counter / timeToWindUp);

            castMagnitude = Mathf.Lerp(0f, castDistance, t) + startingCastDistance;

            Vector3 cast = castDirection * castMagnitude;
            Debug.DrawRay(transform.position, cast, Color.cyan);
        }

        private void CastBobber()
        {
            State = FishingState.Casting;

            counter = 0f;

            targetBobberPos = transform.position + (castDirection * castMagnitude);
            fishingManager.CastBobberEvent(new(targetBobberPos, castDirection, castMagnitude, timeToLand));
        }

        private void BobberLanding()
        {
            State = FishingState.Waiting;

            counter = 0f;

            BobberPos = targetBobberPos;
            fishingManager.BobberLandEvent(new(targetBobberPos));
        }

        public void RecallBobber()
        {
            State = FishingState.None;

            fishApproaching = false;
            counter = 0f;

            BobberPos = transform.position;

            fishingManager.RecallBobberEvent(new(BobberPos, castDirection, timeToLand));
        }

        public void FishGainedInterest(FishBehaviour fish, float approachTime)
        {
            fishApproaching = true;

            approachingFish = fish;
            fishApproachTime = approachTime;

            counter = 0f;
        }
    }
}