using UnityEngine;

namespace DS
{
    using ScriptableObjects;
    public class DSDialogue : MonoBehaviour
    {
        /* Dialogue Scriptable Objects */
        [HideInInspector] public DSDialogueContainerSO dialogueContainer;

        [SerializeField,
        HideInInspector]
        protected DSDialogueGroupSO dialogueGroup;

        [HideInInspector] public DSDialogueSO dialogue;

        /* Filters */
        [SerializeField, HideInInspector] protected bool groupedDialogues;
        [SerializeField, HideInInspector] protected bool startingDialoguesOnly;

        /* Indexes */
        [SerializeField, HideInInspector] protected int selectedDialogueGroupIndex;
        [SerializeField, HideInInspector] protected int selectedDialogueIndex;

        public virtual void SetDSDialogue
        (
            DSDialogueContainerSO dialogueContainerSO,
            DSDialogueGroupSO dialogueGroupSO,
            DSDialogueSO dialogueSO,
            bool groupedDialogues,
            bool startingDialoguesOnly,
            int selectedDialogueGroupIndex,
            int selectedDialogueIndex
        )
        {
            dialogueContainer = dialogueContainerSO;
            dialogueGroup = dialogueGroupSO;
            dialogue = dialogueSO;

            this.groupedDialogues = groupedDialogues;
            this.startingDialoguesOnly = startingDialoguesOnly;

            this.selectedDialogueGroupIndex = selectedDialogueGroupIndex;
            this.selectedDialogueIndex = selectedDialogueIndex;
        }
    }
}