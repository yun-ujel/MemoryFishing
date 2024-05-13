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
        public bool EnablePlayerStateSwitching { get; set; } = true;

        public override void SubscribeToInputActions()
        {
            playerInput.actions["Player/ToggleFishing"].performed += ToggleFishingInput;
            playerInput.actions["Player/Cancel"].performed += OnPressCancel;
        }

        public override void UnsubscribeFromInputActions()
        {
            playerInput.actions["Player/ToggleFishing"].performed -= ToggleFishingInput;
            playerInput.actions["Player/Cancel"].performed -= OnPressCancel;
        }

        private void OnPressCancel(InputAction.CallbackContext ctx)
        {
            if (!EnablePlayerStateSwitching)
            {
                return;
            }

            if (State == PlayerState.Fishing)
            {
                SwitchToBoatState();
                return;
            }
        }

        private void ToggleFishingInput(InputAction.CallbackContext ctx)
        {
            if (!EnablePlayerStateSwitching)
            {
                return;
            }

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
            boatMovement.SetMoveInput(Vector2.zero);

            fishingManager.EnableFishing();
        }

        public void SwitchToEmptyState()
        {
            State = PlayerState.None;

            boatMovement.ReceiveInputs = false;
            boatMovement.SetMoveInput(Vector2.zero);

            fishingManager.DisableFishing();
        }
    }
}