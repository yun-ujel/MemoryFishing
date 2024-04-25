using UnityEngine;

namespace MemoryFishing.UI.Dialogue
{
    public class WindowReader : DialogueReader
    {
        [SerializeField] private Animator animator;

        public override void SubscribeToDialogueEvents()
        {
            dialogueController.OnOpenDialogueEvent += OnOpen;
            dialogueController.OnCloseDialogueEvent += OnClose;
        }

        private void OnClose(object sender, DialogueController.OnCloseDialogueEventArgs args)
        {
            animator.SetBool("Open", false);
        }

        private void OnOpen(object sender, DialogueController.OnOpenDialogueEventArgs args)
        {
            animator.SetBool("Open", true);
        }
    }
}