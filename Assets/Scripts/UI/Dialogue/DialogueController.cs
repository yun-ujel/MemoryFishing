using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Gameplay;
using DS.Enumerations;
using DS.ScriptableObjects;
using DS;

namespace MemoryFishing.UI.Dialogue
{
    public class DialogueController : PlayerController
    {
        public class OnStartDialogueEventArgs : System.EventArgs
        {
            public OnStartDialogueEventArgs(DSDialogueSO dialogue)
            {
                Dialogue = dialogue;
            }

            public DSDialogueSO Dialogue { get; private set; }
            public string Title => Dialogue.Title;
            public string Text => Dialogue.Text;
        }

        [SerializeField] private TextReader textReader;
        [SerializeField] private DSDialogue dialogueHolder;

        private DSDialogueSO currentDialogue;
        public event System.EventHandler<OnStartDialogueEventArgs> OnStartDialogueEvent;

        public override void SubscribeToInputActions()
        {
            playerInput.actions["UI/Submit"].performed += ReceiveSubmitInput;
        }

        public override void UnsubscribeFromInputActions()
        {
            playerInput.actions["UI/Submit"].performed -= ReceiveSubmitInput;
        }

        private void ReceiveSubmitInput(InputAction.CallbackContext ctx)
        {
            if (textReader.HasText && textReader.IsTextFinished)
            {
                if (currentDialogue.DialogueType == DSDialogueType.SingleChoice)
                {
                    GoToNextDialogue(0);
                }

                return;
            }

            textReader.ForceFinishText();
        }

        public override void Start()
        {
            base.Start();
            ReadDialogue(dialogueHolder.dialogue);
        }

        public void ReadDialogue(DSDialogueSO dialogue)
        {
            currentDialogue = dialogue;

            OnStartDialogueEvent?.Invoke(this, new(dialogue));
        }

        public void GoToNextDialogue(int selection)
        {
            if (currentDialogue.IsFinalDialogue)
            {
                return;
            }

            DSDialogueSO choice = currentDialogue.GetChoice(selection);
            ReadDialogue(choice);
        }
    }
}