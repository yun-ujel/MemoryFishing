using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Gameplay.Enumerations;
using MemoryFishing.Gameplay.Fishing.Player;

namespace MemoryFishing.Gameplay
{
    public class PlayerManager : PlayerController
    {
        [SerializeField] private BoatMovement boatMovement;
        [SerializeField] private PlayerFishingManager fishingManager;

        public PlayerState State { get; private set; }

        public override void Start()
        {
            base.Start();

            SwitchToBoatState();
        }

        public override void SubscribeToInputActions()
        {
            playerInput.actions["Player/ToggleFishing"].performed += ToggleFishingInput;
        }

        public override void UnsubscribeFromInputActions()
        {
            playerInput.actions["Player/ToggleFishing"].performed -= ToggleFishingInput;
        }

        private void ToggleFishingInput(InputAction.CallbackContext ctx)
        {
            if (State == PlayerState.Fishing)
            {
                SwitchToBoatState();
                return;
            }

            if (State == PlayerState.Boat)
            {
                SwitchToFishingState();
                return;
            }
        }

        public void SwitchToBoatState()
        {
            State = PlayerState.Boat;

            boatMovement.ReceiveInputs = true;
            fishingManager.DisableFishing();
        }

        public void SwitchToFishingState()
        {
            State = PlayerState.Fishing;

            boatMovement.ReceiveInputs = false;
            fishingManager.EnableFishing();
        }
    }
}