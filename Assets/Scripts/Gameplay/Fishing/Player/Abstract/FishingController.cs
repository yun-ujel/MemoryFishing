using UnityEngine;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;
using MemoryFishing.Gameplay.Enumerations;
using MemoryFishing.Utilities;

namespace MemoryFishing.Gameplay.Fishing.Player
{
    [RequireComponent(typeof(PlayerFishingManager))]
    public abstract class FishingController : PlayerController
    {
        protected PlayerFishingManager fishingManager;
        protected FishingState State
        {
            get => fishingManager.State;
            set => fishingManager.State = value;
        }
        protected Vector3 BobberPos
        {
            get => fishingManager.BobberPos;
            set => fishingManager.BobberPos = value;
        }

        public override void Start()
        {
            playerInput = GeneralUtils.GetPlayerInput();

            fishingManager = GetComponent<PlayerFishingManager>();
            fishingManager.OnDisableFishingEvent += OnDisableFishing;
            fishingManager.OnEnableFishingEvent += OnEnableFishing;
        }

        protected virtual void OnEnableFishing(object sender, OnEnableFishingEventArgs args)
        {
            SubscribeToInputActions();
        }

        protected virtual void OnDisableFishing(object sender, OnEnableFishingEventArgs args)
        {
            UnsubscribeFromInputActions();
        }

        public abstract override void SubscribeToInputActions();

        public abstract override void UnsubscribeFromInputActions();
    }
}