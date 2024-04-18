using UnityEngine;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;
using MemoryFishing.Gameplay.Enumerations;

namespace MemoryFishing.Gameplay.Fishing.Player
{
    public class PlayerFishingManager : MonoBehaviour
    {
        [Space, SerializeField] private Transform bobber;
        public bool FishingEnabled { get; private set; }
        public FishingState State { get; set; } = FishingState.None;
        public Vector3 BobberPos
        {
            get => bobber.position;
            set => bobber.position = value;
        }

        public event System.EventHandler<OnStartFightingEventArgs> OnStartFightingEvent;
        public event System.EventHandler<OnEndFightingEventArgs> OnEndFightingEvent;

        public event System.EventHandler<OnStartWindUpEventArgs> OnStartWindUpEvent;
        public event System.EventHandler<OnCastBobberEventArgs> OnCastBobberEvent;
        public event System.EventHandler<OnBobberLandEventArgs> OnBobberLandEvent;
        public event System.EventHandler<OnFishApproachingEventArgs> OnFishApproachingEvent;
        public event System.EventHandler<OnRecallBobberEventArgs> OnRecallBobberEvent;

        public event System.EventHandler<OnCatchFishEventArgs> OnCatchFishEvent;

        public event System.EventHandler<OnEnableFishingEventArgs> OnDisableFishingEvent;
        public event System.EventHandler<OnEnableFishingEventArgs> OnEnableFishingEvent;

        public void EnableFishing()
        {
            Debug.Log("Enable Fishing");

            FishingEnabled = true;
            OnEnableFishingEvent?.Invoke(this, new());
        }

        private void Update()
        {
            if (State == FishingState.None)
            {
                BobberPos = transform.position;
            }
        }

        public void DisableFishing()
        {
            Debug.Log("Disable Fishing");

            FishingEnabled = false;
            OnDisableFishingEvent?.Invoke(this, new());
        }

        public void StartFightingEvent(OnStartFightingEventArgs args)
        {
            OnStartFightingEvent?.Invoke(this, args);
        }

        public void EndFightingEvent(OnEndFightingEventArgs args)
        {
            OnEndFightingEvent?.Invoke(this, args);
        }

        public void StartWindUpEvent(OnStartWindUpEventArgs args)
        {
            OnStartWindUpEvent?.Invoke(this, args);
        }

        public void CastBobberEvent(OnCastBobberEventArgs args)
        {
            OnCastBobberEvent?.Invoke(this, args);
        }

        public void BobberLandEvent(OnBobberLandEventArgs args)
        {
            OnBobberLandEvent?.Invoke(this, args);
        }

        public void FishApproachingEvent(OnFishApproachingEventArgs args)
        {
            OnFishApproachingEvent?.Invoke(this, args);
        }

        public void RecallBobberEvent(OnRecallBobberEventArgs args)
        {
            OnRecallBobberEvent?.Invoke(this, args);
        }

        public void CatchFishEvent(OnCatchFishEventArgs args)
        {
            OnCatchFishEvent?.Invoke(this, args);
        }
    }
}