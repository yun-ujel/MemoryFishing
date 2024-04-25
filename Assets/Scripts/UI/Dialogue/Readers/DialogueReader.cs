using UnityEngine;

namespace MemoryFishing.UI.Dialogue
{
    public abstract class DialogueReader : MonoBehaviour
    {
        [SerializeField] protected DialogueController dialogueController;

        protected virtual void Start()
        {
            if (dialogueController != null)
            {
                SubscribeToDialogueEvents();
                return;
            }

            Debug.LogWarning($"Dialogue Controller not assigned for {this}");
        }

        public abstract void SubscribeToDialogueEvents();
    }
}