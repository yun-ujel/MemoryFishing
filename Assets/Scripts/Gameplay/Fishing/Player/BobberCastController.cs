using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Gameplay.Fishing.Fish;
using MemoryFishing.Utilities;
using MemoryFishing.Gameplay.Fishing.Enumerations;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;

namespace MemoryFishing.Gameplay.Fishing.Player
{
    public partial class BobberCastController : FishingController
    {
        [Header("References")]
        [SerializeField] private PlayerDirection direction;
        [SerializeField] private ReelingController reelingController;

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
            base.SubscribeToInputActions();
            playerInput.actions["Player/Fling"].performed += OnFlingPressed;
            playerInput.actions["Player/Fling"].canceled += OnFlingReleased;
        }

        private void OnFlingPressed(InputAction.CallbackContext ctx)
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

            if (State == FishingState.Waiting)
            {
                RecallBobber();
            }
        }

        private void OnFlingReleased(InputAction.CallbackContext ctx)
        {
            if (State == FishingState.WindUp)
            {
                CastBobber();
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

            if (State == FishingState.Recall)
            {
                counter += Time.deltaTime;

                if (counter > timeToRecall)
                {
                    State = FishingState.None;
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
                    reelingController.StartReeling(approachingFish);
                    
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

        private void RecallBobber()
        {
            State = FishingState.Recall;

            fishApproaching = false;
            counter = 0f;

            fishingManager.RecallBobberEvent(new(targetBobberPos, castDirection, timeToLand));
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