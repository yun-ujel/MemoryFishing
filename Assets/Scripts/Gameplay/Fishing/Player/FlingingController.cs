using UnityEngine;
using UnityEngine.InputSystem;
using MemoryFishing.Utilities;
using MemoryFishing.Gameplay.Fishing.Fish;
using MemoryFishing.Gameplay.Fishing.Enumerations;

namespace MemoryFishing.Gameplay.Fishing.Player
{
    public class FlingingController : FishingController
    {
        [Header("References")]
        [SerializeField] private PlayerDirection direction;
        [SerializeField] private ReelingController reelingController;

        [Header("Settings")]
        [SerializeField, Range(0f, 50f)] private float flingForce;
        [SerializeField, Range(0f, 5f)] private float flingDuration = 0.5f;

        [Space]

        [SerializeField, Range(0f, 10f)] private float catchFishRadius = 2f;

        private FishBehaviour fish;
        private Rigidbody fishBody;

        private bool flingAvailable, isFlinging;
        private float flingCounter;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.DrawWireDisc(Vector3.zero, Vector3.up, catchFishRadius);
        }
#endif

        #region Initialisation Methods

        public override void Start()
        {
            base.Start();

            reelingController.OnEndReelingEvent += OnEndReeling;
        }

        public override void SubscribeToInputActions()
        {
            base.SubscribeToInputActions();

            playerInput.actions["Player/Fling"].performed += ReceiveFlingInput;
        }

        #endregion

        #region Event Methods

        private void ReceiveFlingInput(InputAction.CallbackContext ctx)
        {
            if (!flingAvailable || fishBody == null)
            {
                return;
            }

            StartFling();
        }

        private void OnEndReeling(object sender, ReelingController.OnEndReelingEventArgs args)
        {
            State = FishingState.Exhausted;

            if (args.FishBehaviour != fish)
            {
                fish = args.FishBehaviour;
                fishBody = fish.GetComponent<Rigidbody>();
            }

            flingAvailable = true;
        }

        #endregion

        #region Private Methods

        private void Update()
        {
            UpdateFlingCounter();
        }

        private void UpdateFlingCounter()
        {
            if (!isFlinging)
            {
                return;
            }

            BobberPos = fishBody.position;

            if (flingCounter < 0f || FishInRange(transform.position, fishBody.position))
            {
                EndFling();
                return;
            }

            flingCounter -= Time.deltaTime;
        }

        private void StartFling()
        {
            Vector3 flingDir = direction.GetLookDirection(fishBody.position);
            fishBody.velocity = flingDir * flingForce;

            flingCounter = flingDuration;

            isFlinging = true;
            flingAvailable = false;
        }

        private void EndFling()
        {
            isFlinging = false;

            if (FishInRange(transform.position, fishBody.position))
            {
                CatchFish();
                return;
            }

            reelingController.StartReeling(fish);
        }

        private void CatchFish()
        {
            fishBody.velocity = Vector3.zero;
            State = FishingState.None;
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