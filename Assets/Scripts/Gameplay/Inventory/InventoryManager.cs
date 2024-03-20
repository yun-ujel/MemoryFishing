using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;

namespace MemoryFishing.Gameplay.Inventory
{
    public class InventoryManager : PlayerController
    {
        [SerializeField] private PlayerFishingManager fishingManager;

        private Inventory inventory;
        public Inventory Inventory => inventory;

        private void Awake()
        {
            inventory = new Inventory(8);
        }

        public override void Start()
        {
            base.Start();

            fishingManager.OnCatchFishEvent += OnCatchFish;
        }

        public override void SubscribeToInputActions()
        {
            base.SubscribeToInputActions();

            playerInput.actions["Player/Inventory"].performed += ReceiveInventoryInput;
            playerInput.actions["UI/Inventory"].performed += ReceiveInventoryInput;
        }

        private void ReceiveInventoryInput(InputAction.CallbackContext ctx)
        {

        }

        private void OnCatchFish(object sender, OnCatchFishEventArgs args)
        {
            if (args.Item == null)
            {
                return;
            }

            PickupItem(args.Item);
        }

        private void PickupItem(InventoryItem item)
        {
            if (inventory.TryAddItem(item))
            {
                return;
            }

            Debug.Log("Can't fit item!");
        }
    }
}