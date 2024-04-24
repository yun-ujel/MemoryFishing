using UnityEngine;
using UnityEngine.InputSystem;

using System.Collections.Generic;

using MemoryFishing.Gameplay;
using DS.Enumerations;
using DS.ScriptableObjects;
using DS;

namespace MemoryFishing.UI.Dialogue
{
    public class DialogueController : PlayerController
    {
        [SerializeField] private TextReader textReader;
        [SerializeField] private DSDialogue dialogue;

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

        }

        public override void Start()
        {
            base.Start();
            ReadDialogue(dialogue.dialogue);
        }

        public void ReadDialogue(DSDialogueSO dialogue)
        {
            List<string> quotes = new List<string>();
            AddDialoguesToQuoteList(dialogue, quotes);

            for(int i = 0; i < quotes.Count; i++)
            {
                Debug.Log(quotes[i]);
            }
        }

        private void AddDialoguesToQuoteList(DSDialogueSO firstDialogue, List<string> quotes)
        {
            quotes.Add(firstDialogue.Text);

            if (firstDialogue.DialogueType == DSDialogueType.MultipleChoice)
            {
                return;
            }

            DSDialogueSO choice = firstDialogue.GetChoice(0);
            if (choice != null)
            {
                AddDialoguesToQuoteList(choice, quotes);
            }
        }
    }
}