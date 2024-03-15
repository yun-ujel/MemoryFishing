using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Gameplay.Fishing.Fish;

namespace MemoryFishing.Gameplay.Fishing.Player
{
    public class BobberCastController : PlayerController
    {
        [Header("References")]
        [SerializeField] private PlayerDirection direction;
        [SerializeField] private ReelingController reelingController;

        [Space, SerializeField] private FishBehaviour startFish;

        [Header("Bobber Casting")]
        [SerializeField, Range(0.01f, 10f)] private float timeToCastFully;
        [SerializeField] private float maxCastDistance;
        [SerializeField] private float minCastDistance;

        [Space, SerializeField] private AnimationCurve castCurve;

        private Vector3 castDirection;
        private float castMagnitude;

        private float timeSinceCastStarted;
        private bool isCasting;

        public override void SubscribeToInputActions()
        {
            base.SubscribeToInputActions();
            playerInput.actions["Player/Fling"].performed += OnFlingPressed;
            playerInput.actions["Player/Fling"].canceled += OnFlingReleased;
        }

        private void OnFlingPressed(InputAction.CallbackContext ctx)
        {
            isCasting = true;
            castDirection = direction.GetLookDirection(transform.position);

            timeSinceCastStarted = 0f;
            castMagnitude = 0f;
        }

        private void OnFlingReleased(InputAction.CallbackContext ctx)
        {

        }

        private void Update()
        {
            if (isCasting)
            {
                StretchCastDistance();
            }
        }

        private void StretchCastDistance()
        {
            timeSinceCastStarted += Time.deltaTime;
            float t = castCurve.Evaluate(timeSinceCastStarted / timeToCastFully);

            castMagnitude = Mathf.Lerp(0f, maxCastDistance, t) + minCastDistance;

            Vector3 cast = castDirection * castMagnitude;
            Debug.DrawRay(transform.position, cast, Color.cyan);
        }
    }
}