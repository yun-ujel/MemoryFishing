using UnityEngine;

using TMPro;

namespace MemoryFishing.UI.Dialogue
{
    public class TitleReader : DialogueReader
    {
        [SerializeField] private TextMeshProUGUI textComponent;

        public override void SubscribeToDialogueEvents()
        {
            dialogueController.OnStartDialogueEvent += OnStartDialogue;
        }

        private void OnStartDialogue(object sender, DialogueController.OnStartDialogueEventArgs args)
        {
            textComponent.text = args.Title;
        }
    }
}