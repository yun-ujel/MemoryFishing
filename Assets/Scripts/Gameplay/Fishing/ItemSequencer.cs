using UnityEngine;

using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;
using MemoryFishing.Gameplay.Inventory;
using MemoryFishing.UI.Dialogue;

using DS;
using DS.ScriptableObjects;

namespace MemoryFishing.Gameplay
{
    public class ItemSequencer : MonoBehaviour
    {
        [System.Serializable]
        public class CaughtItem
        {
            [field: SerializeField] public InventoryItem Item { get; set; }
            [field: SerializeField] public DSDialogue Dialogue { get; set; }

            public DSDialogueSO DialogueSO => Dialogue.dialogue;
        }

        [Header("References")]
        [SerializeField] private PlayerFishingManager fishingManager;

        [Space]

        [SerializeField] private InventoryManager inventoryManager;
        [SerializeField] private DialogueController dialogueController;

        [Header("Items")]
        [SerializeField] private CaughtItem[] catches;

        private int currentItem;

        private void Start()
        {
            fishingManager.OnCatchFishEvent += OnCatchFish;
        }

        private void OnCatchFish(object sender, OnCatchFishEventArgs args)
        {
            if (currentItem >= catches.Length)
            {
                return;
            }

            inventoryManager.PickupItem(catches[currentItem].Item);
            dialogueController.ReadDialogue(catches[currentItem].DialogueSO);
        }
    }
}