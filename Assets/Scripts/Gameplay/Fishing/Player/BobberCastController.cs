using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Gameplay.Fishing.Fish;

namespace MemoryFishing.Gameplay.Fishing.Player
{
    public class BobberCastController : PlayerController
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

        }

        public event System.EventHandler<OnCastBobberEventArgs> OnCastBobberEvent;
        public event System.EventHandler<OnBobberLandEventArgs> OnBobberLandEvent;

        [Header("References")]
        [SerializeField] private PlayerDirection direction;
        [SerializeField] private ReelingController reelingController;

        [Space, SerializeField] private FishBehaviour startFish;

        [Header("Bobber Casting: Wind Up")]
        [SerializeField, Range(0.01f, 10f)] private float timeToWindUp;
        [SerializeField] private AnimationCurve windUpCurve;

        [Space, SerializeField] private float castDistance;
        [SerializeField] private float startingCastDistance;

        [Space, SerializeField, Range(0.01f, 10f)] private float timeToLand;

        private Vector3 castDirection;
        private float castMagnitude;

        private bool inCastWindup;
        private float windUpCounter;

        private bool isCasting;
        private float castTimeCounter;

        public override void SubscribeToInputActions()
        {
            base.SubscribeToInputActions();
            playerInput.actions["Player/Fling"].performed += OnFlingPressed;
            playerInput.actions["Player/Fling"].canceled += OnFlingReleased;
        }

        private void OnFlingPressed(InputAction.CallbackContext ctx)
        {
            inCastWindup = true;

            windUpCounter = 0f;
            castMagnitude = 0f;
        }

        private void OnFlingReleased(InputAction.CallbackContext ctx)
        {
            inCastWindup = false;
            CastBobber();
        }

        private void Update()
        {
            if (inCastWindup)
            {
                castDirection = direction.GetLookDirection(transform.position);
                WindUpCast();
                return;
            }

            if (isCasting)
            {
                castTimeCounter += Time.deltaTime;
                if (castTimeCounter > timeToLand)
                {
                    BobberLanding();
                }
                return;
            }
        }

        private void WindUpCast()
        {
            windUpCounter += Time.deltaTime;
            float t = windUpCurve.Evaluate(windUpCounter / timeToWindUp);

            castMagnitude = Mathf.Lerp(0f, castDistance, t) + startingCastDistance;

            Vector3 cast = castDirection * castMagnitude;
            Debug.DrawRay(transform.position, cast, Color.cyan);
        }

        private void CastBobber()
        {
            inCastWindup = false;
            isCasting = true;

            castTimeCounter = 0f;

            OnCastBobberEvent?.Invoke(this, new(transform.position + (castDirection * castMagnitude), castDirection, castMagnitude, timeToLand));
        }

        private void BobberLanding()
        {
            castTimeCounter = 0f;

            OnBobberLandEvent?.Invoke(this, new());
        }
    }
}