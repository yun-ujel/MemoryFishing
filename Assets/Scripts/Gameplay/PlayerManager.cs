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
        public PlayerState PreviousState { get; private set; }
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
            SetPreviousState(State);
            State = PlayerState.Boat;

            boatMovement.ReceiveInputs = true;

            fishingManager.DisableFishing();
        }

        public void SwitchToFishingState()
        {
            SetPreviousState(State);
            State = PlayerState.Fishing;

            boatMovement.ReceiveInputs = false;
            boatMovement.SetMoveInput(Vector2.zero);

            fishingManager.EnableFishing();
        }

        public void SwitchToEmptyState()
        {
            SetPreviousState(State);
            State = PlayerState.None;

            boatMovement.ReceiveInputs = false;
            boatMovement.SetMoveInput(Vector2.zero);

            fishingManager.DisableFishing();
        }

        public void SwitchToPreviousState()
        {
            Debug.Log($"Switch to {PreviousState}");

            switch (PreviousState)
            {
                case PlayerState.Boat:
                    SwitchToBoatState();
                    break;
                case PlayerState.Fishing:
                    SwitchToFishingState();
                    break;
                case PlayerState.None:
                    break;
                default:
                    break;
            }
        }

        private void SetPreviousState(PlayerState State)
        {
            if (State != PlayerState.None)
            {
                PreviousState = State;
            }
        }
    }
}