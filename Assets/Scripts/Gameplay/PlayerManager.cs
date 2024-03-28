using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Gameplay.Enumerations;

namespace MemoryFishing.Gameplay
{
    public class PlayerManager : PlayerController
    {
        [SerializeField] private BoatMovement boatMovment;
        [SerializeField] private GameObject fishingHandler;

        public PlayerState State { get; private set; }

        public override void SubscribeToInputActions()
        {
            base.SubscribeToInputActions();

            playerInput.actions["Player/ToggleFishing"].performed += ToggleFishingInput;
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

            boatMovment.enabled = true;
            fishingHandler.SetActive(false);
        }

        public void SwitchToFishingState()
        {
            State = PlayerState.Fishing;

            boatMovment.enabled = false;
            fishingHandler.SetActive(true);
        }
    }
}