using UnityEngine;

using MemoryFishing.Gameplay.Fishing.Enumerations;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;

namespace MemoryFishing.Gameplay.Fishing.Player
{
    public class PlayerFishingManager : MonoBehaviour
    {
        [Space, SerializeField] private Transform bobber;
        public FishingState State { get; set; } = FishingState.None;
        public Vector3 BobberPos
        {
            get => bobber.position;
            set => bobber.position = value;
        }

        public event System.EventHandler<OnStartReelingEventArgs> OnStartReelingEvent;
        public event System.EventHandler<OnEndReelingEventArgs> OnEndReelingEvent;

        public event System.EventHandler<OnCastBobberEventArgs> OnCastBobberEvent;
        public event System.EventHandler<OnBobberLandEventArgs> OnBobberLandEvent;
        public event System.EventHandler<OnRecallBobberEventArgs> OnRecallBobberEvent;

        public void StartReelingEvent(OnStartReelingEventArgs args)
        {
            OnStartReelingEvent?.Invoke(this, args);
        }

        public void EndReelingEvent(OnEndReelingEventArgs args)
        {
            OnEndReelingEvent?.Invoke(this, args);
        }

        public void CastBobberEvent(OnCastBobberEventArgs args)
        {
            OnCastBobberEvent?.Invoke(this, args);
        }

        public void BobberLandEvent(OnBobberLandEventArgs args)
        {
            OnBobberLandEvent?.Invoke(this, args);
        }

        public void RecallBobberEvent(OnRecallBobberEventArgs args)
        {
            OnRecallBobberEvent?.Invoke(this, args);
        }
    }
}