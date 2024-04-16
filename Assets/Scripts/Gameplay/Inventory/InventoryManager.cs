using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;

namespace MemoryFishing.Gameplay.Inventory
{
    public class InventoryManager : PlayerController
    {
        public class OnGetItemEventArgs : System.EventArgs
        {
            public InventoryItem Item { get; private set; }
            public int Slot { get; private set; }

            public OnGetItemEventArgs(InventoryItem item, int slot)
            {
                Item = item;
                Slot = slot;
            }
        }

        public class OnInventoryOpenedEventArgs : System.EventArgs
        {

        }

        [SerializeField] private PlayerFishingManager fishingManager;

        private Inventory inventory;
        public Inventory Inventory => inventory;
        public bool InventoryOpen { get; private set; }

        public event System.EventHandler<OnInventoryOpenedEventArgs> OnInventoryOpenedEvent;
        public event System.EventHandler<OnInventoryOpenedEventArgs> OnInventoryClosedEvent;

        public event System.EventHandler<OnGetItemEventArgs> OnGetItemEvent;

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
            playerInput.actions["Player/Inventory"].performed += PlayerInventoryInput;
            playerInput.actions["UI/Inventory"].performed += UIInventoryInput;
        }

        public override void UnsubscribeFromInputActions()
        {
            playerInput.actions["Player/Inventory"].performed -= PlayerInventoryInput;
            playerInput.actions["UI/Inventory"].performed -= UIInventoryInput;
        }

        private void PlayerInventoryInput(InputAction.CallbackContext ctx)
        {
            InventoryOpen = true;
            OnInventoryOpenedEvent?.Invoke(this, new());

            playerInput.SwitchCurrentActionMap("UI");
        }
        private void UIInventoryInput(InputAction.CallbackContext ctx)
        {
            InventoryOpen = false;
            OnInventoryClosedEvent?.Invoke(this, new());

            playerInput.SwitchCurrentActionMap("Player");
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
            if (inventory.TryAddItem(item, out int slot))
            {
                OnGetItemEvent?.Invoke(this, new(item, slot));
                return;
            }

            Debug.Log("Can't fit item!");
        }
    }
}