using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Gameplay.Fishing.Fish;
using MemoryFishing.Utilities;
using MemoryFishing.Gameplay.Fishing.Enumerations;

namespace MemoryFishing.Gameplay.Fishing.Player
{
    public class BobberCastController : FishingController
    {
        public class OnCastBobberEventArgs : System.EventArgs
        {
            public Vector3 TargetPosition { get; private set; }
            public Vector3 Direction { get; private set; }
            public float Magnitude { get; private set; }

            public float TimeToLand { get; private set; }

            public OnCastBobberEventArgs(Vector3 targetPosition, Vector3 direction, float magnitude, float timeToLand)
            {
                TargetPosition = targetPosition;
                Direction = direction;
                Magnitude = magnitude;
                TimeToLand = timeToLand;
            }
        }

        public class OnBobberLandEventArgs : System.EventArgs
        {
            public Vector3 BobberPosition { get; private set; }

            public OnBobberLandEventArgs(Vector3 bobberPosition)
            {
                BobberPosition = bobberPosition;
            }
        }

        public event System.EventHandler<OnCastBobberEventArgs> OnCastBobberEvent;
        public event System.EventHandler<OnBobberLandEventArgs> OnBobberLandEvent;

        [Header("References")]
        [SerializeField] private PlayerDirection direction;
        [SerializeField] private ReelingController reelingController;

        [Header("Bobber Casting: Wind Up")]
        [SerializeField, Range(0.01f, 10f)] private float timeToWindUp;
        [SerializeField] private AnimationCurve windUpCurve;

        [Space, SerializeField] private float castDistance;
        [SerializeField] private float startingCastDistance;

        [Space, SerializeField, Range(0.01f, 10f)] private float timeToLand;

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
            OnCastBobberEvent?.Invoke(this, new(targetBobberPos, castDirection, castMagnitude, timeToLand));
        }

        private void BobberLanding()
        {
            State = FishingState.Waiting;

            counter = 0f;

            BobberPos = targetBobberPos;
            OnBobberLandEvent?.Invoke(this, new(targetBobberPos));
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