using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Gameplay;
using DS.Enumerations;
using DS.ScriptableObjects;

namespace MemoryFishing.UI.Dialogue
{
    public class DialogueController : PlayerController
    {
        public class OnOpenDialogueEventArgs : System.EventArgs { }
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
        public class OnCloseDialogueEventArgs : System.EventArgs { }

        [Space, SerializeField] private TextReader textReader;
        [SerializeField] private DialogueOptionController optionController;

        private DSDialogueSO currentDialogue;

        public event System.EventHandler<OnOpenDialogueEventArgs> OnOpenDialogueEvent;
        public event System.EventHandler<OnStartDialogueEventArgs> OnStartDialogueEvent;
        public event System.EventHandler<OnCloseDialogueEventArgs> OnCloseDialogueEvent;
    
        public bool DialogueWindowOpen { get; private set; }

        public override void SubscribeToInputActions()
        {
            playerInput.actions["UI/Submit"].performed += ReceiveSubmitInput;
            playerInput.actions["UI/LeftClick"].performed += ReceiveSubmitInput;
        }

        public override void UnsubscribeFromInputActions()
        {
            playerInput.actions["UI/Submit"].performed -= ReceiveSubmitInput;
            playerInput.actions["UI/LeftClick"].performed -= ReceiveSubmitInput;
        }

        public override void Start()
        {
            base.Start();
            CloseWindow();
            textReader.OnTextFinishedEvent += OnTextFinished;
            optionController.OnOptionSelectEvent += OnOptionSelected;
        }

        private void OnOptionSelected(object sender, DialogueOptionController.OnOptionSelectEventArgs args)
        {
            GoToNextDialogue(args.SelectedOptionIndex);
        }

        private void OnTextFinished(object sender, TextReader.OnTextFinishedEventArgs args)
        {
            Debug.Log("Text Finished");

            if (currentDialogue.DialogueType == DSDialogueType.MultipleChoice)
            {
                optionController.LoadOptions(currentDialogue);
            }
        }

        private void ReceiveSubmitInput(InputAction.CallbackContext ctx)
        {
            if (!DialogueWindowOpen) { return; }

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

        public void ReadDialogue(DSDialogueSO dialogue)
        {
            if (!DialogueWindowOpen)
            {
                OpenWindow();
            }

            currentDialogue = dialogue;
            OnStartDialogueEvent?.Invoke(this, new(dialogue));
        }

        public void GoToNextDialogue(int selection)
        {
            if (currentDialogue.IsFinalDialogue())
            {
                CloseWindow();

                return;
            }

            DSDialogueSO choice = currentDialogue.GetChoice(selection);
            ReadDialogue(choice);
        }

        private void OpenWindow()
        {
            DialogueWindowOpen = true;
            OnOpenDialogueEvent?.Invoke(this, new());

            playerInput.SwitchCurrentActionMap("UI");
        }

        private void CloseWindow()
        {
            DialogueWindowOpen = false;
            OnCloseDialogueEvent?.Invoke(this, new());

            playerInput.SwitchCurrentActionMap("Player");
        }
    }
}