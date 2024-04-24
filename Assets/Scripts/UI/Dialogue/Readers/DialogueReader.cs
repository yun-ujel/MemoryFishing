using UnityEngine;

namespace MemoryFishing.UI.Dialogue
{
    public abstract class DialogueReader : MonoBehaviour
    {
        [SerializeField] private DialogueController dialogueController;

        protected virtual void Start()
        {
            if (dialogueController != null)
            {
                dialogueController.OnStartDialogueEvent += OnStartDialogue;
                return;
            }

            Debug.LogWarning($"Dialogue Controller not assigned for {this}");
        }

        public abstract void OnStartDialogue(object sender, DialogueController.OnStartDialogueEventArgs args);
    }
}