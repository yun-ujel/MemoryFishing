using UnityEngine;

using MemoryFishing.Gameplay.Fishing.Player;
using MemoryFishing.Gameplay.Fishing.Player.EventArgs;
using MemoryFishing.Gameplay.Inventory;
using MemoryFishing.UI.Dialogue;

using DS;
using DS.ScriptableObjects;
using MemoryFishing.Gameplay.Fishing.Fish;
using UnityEngine.UI;

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
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private PlayerFishingManager fishingManager;

        [Space]

        [SerializeField] private InventoryManager inventoryManager;
        [SerializeField] private DialogueController dialogueController;

        [Space]

        [SerializeField] private CutsceneManager cutsceneManager;

        [Header("Items")]
        [SerializeField] private CaughtItem[] catches;

        [Header("UI")]
        [SerializeField] private RawImage screenBlackout;
        [SerializeField] private Color blackoutColor = new Color(0f, 0f, 0f, 0.2f);

        [Space, SerializeField] private RawImage itemIcon;
        [SerializeField] private float fadeTime = 0.5f;

        private int currentItem;
        private float counter = 1f;
        private bool displayItems = false;
        private bool dialogueInitiated;

        private void Start()
        {
            fishingManager.OnCatchFishEvent += OnCatchFish;
            dialogueController.OnCloseDialogueEvent += OnCloseDialogue;
        }

        private void OnCloseDialogue(object sender, DialogueController.OnCloseDialogueEventArgs args)
        {
            if (!dialogueInitiated)
            {
                return;
            }

            playerManager.SwitchToPreviousState();

            dialogueInitiated = false;

            displayItems = false;
            counter = 0f;
        }

        private void OnCatchFish(object sender, OnCatchFishEventArgs args)
        {
            if (currentItem == catches.Length - 1)
            {
                TriggerCutscene();
                return;
            }

            if (currentItem >= catches.Length)
            {
                return;
            }

            if (args.Fish.GetType() == typeof(PaulFish))
            {
                return;
            }

            PickupNextItem();
        }

        private void PickupNextItem()
        {
            inventoryManager.PickupItem(catches[currentItem].Item);
            dialogueController.ReadDialogue(catches[currentItem].DialogueSO);
            dialogueInitiated = true;

            playerManager.SwitchToEmptyState();

            itemIcon.texture = catches[currentItem].Item.Texture;
            itemIcon.color = Color.white;

            currentItem++;

            displayItems = true;
            counter = 0f;
        }

        private void TriggerCutscene()
        {
            playerManager.SwitchToEmptyState();

            cutsceneManager.StartCutscene();
            cutsceneManager.OnCutsceneFinishedEvent += OnCutsceneFinished;
        }

        private void OnCutsceneFinished(object sender, System.EventArgs args)
        {
            playerManager.SwitchToPreviousState();
            PickupNextItem();

            cutsceneManager.OnCutsceneFinishedEvent -= OnCutsceneFinished;
        }

        private void Update()
        {
            counter += Time.deltaTime;
            float t = counter / fadeTime;

            if (!displayItems)
            {
                screenBlackout.color = Color.Lerp(blackoutColor, Color.clear, t);
                itemIcon.color = Color.Lerp(Color.white, Color.clear, t);

                return;
            }

            screenBlackout.color = Color.Lerp(Color.clear, blackoutColor, t);
            itemIcon.color = Color.Lerp(Color.clear, Color.white, t);
        }
    }
}