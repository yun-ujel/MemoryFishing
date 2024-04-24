using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace DS.Inspectors
{
    using Utilities;
    using Data.Save;
    using ScriptableObjects;

    [CustomEditor(typeof(DSDialogue), true)]
    public class DSDialogueEditor : Editor
    {
        /* Dialogue Scriptable Objects */
        private SerializedProperty dialogueContainerProperty;
        private SerializedProperty dialogueGroupProperty;
        private SerializedProperty dialogueProperty;

        /* Filters */
        private SerializedProperty groupedDialoguesProperty;
        private SerializedProperty startingDialogueOnlyProperty;

        /* Indexes */
        private SerializedProperty selectedDialogueGroupIndexProperty;
        private SerializedProperty selectedDialogueIndexProperty;

        private void OnEnable()
        {
            dialogueContainerProperty = serializedObject.FindProperty("dialogueContainer");
            dialogueGroupProperty = serializedObject.FindProperty("dialogueGroup");
            dialogueProperty = serializedObject.FindProperty("dialogue");

            groupedDialoguesProperty = serializedObject.FindProperty("groupedDialogues");
            startingDialogueOnlyProperty = serializedObject.FindProperty("startingDialoguesOnly");

            selectedDialogueGroupIndexProperty = serializedObject.FindProperty("selectedDialogueGroupIndex");
            selectedDialogueIndexProperty = serializedObject.FindProperty("selectedDialogueIndex");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDialogueContainerArea();

            DSDialogueContainerSO dialogueContainer = (DSDialogueContainerSO)dialogueContainerProperty.objectReferenceValue;

            if (dialogueContainer == null)
            {
                StopDrawing("Select a Dialogue Container to see the rest of the Inspector");

                return;
            }

            DrawFiltersArea();

            bool useStartingDialogueOnly = startingDialogueOnlyProperty.boolValue;

            List<string> dialogueNames;
            string dialogueFolderPath = $"Assets/DialogueSystem/Dialogues/{dialogueContainer.FileName}";

            string dialogueInfoMessage;


            if (groupedDialoguesProperty.boolValue)
            {
                List<string> dialogueGroupNames = dialogueContainer.GetDialogueGroupNames();

                if (dialogueGroupNames.Count == 0)
                {
                    StopDrawing("There are no Dialogue Groups in this Dialogue Container.");

                    return;
                }

                DrawDialogueGroupArea(dialogueContainer, dialogueGroupNames);

                DSDialogueGroupSO dialogueGroup = (DSDialogueGroupSO)dialogueGroupProperty.objectReferenceValue;

                dialogueNames = dialogueContainer.GetGroupedDialogueNodeNames(dialogueGroup, useStartingDialogueOnly);
                dialogueFolderPath += $"/Groups/{dialogueGroup.GroupName}/Dialogues";
                dialogueInfoMessage = "There are no" + (useStartingDialogueOnly ? " Starting" : "") + " Dialogues in this Dialogue Group.";
            }
            else
            {
                dialogueNames = dialogueContainer.GetUngroupedDialogueNodeNames(useStartingDialogueOnly);
                dialogueFolderPath += $"/Global/Dialogues";
                dialogueInfoMessage = "There are no Ungrouped" + (useStartingDialogueOnly ? " Starting" : "") + " Dialogues in this Dialogue Container.";
            }

            if (dialogueNames.Count == 0)
            {
                StopDrawing(dialogueInfoMessage);

                return;
            }

            DrawDialogueArea(dialogueNames, dialogueFolderPath);

            serializedObject.ApplyModifiedProperties();

            DrawBaseInspector();
        }

        #region Draw Methods
        private void DrawDialogueContainerArea()
        {
            DSInspectorUtility.DrawHeader("Dialogue Container");

            dialogueContainerProperty.DrawPropertyField();

            EditorGUILayout.Space(4);
        }

        private void DrawFiltersArea()
        {
            DSInspectorUtility.DrawHeader("Filters");

            groupedDialoguesProperty.DrawPropertyField();
            startingDialogueOnlyProperty.DrawPropertyField();

            EditorGUILayout.Space(4);
        }
        private void DrawDialogueGroupArea(DSDialogueContainerSO dialogueContainer, List<string> dialogueGroupNames)
        {
            DSInspectorUtility.DrawHeader("Dialogue Group");

            /* "Old" Property is a property that has not been updated, as it has not been drawn yet */

            DSDialogueGroupSO oldDialogueGroup = (DSDialogueGroupSO)dialogueGroupProperty.objectReferenceValue;

            bool isOldDialogueGroupNull = oldDialogueGroup == null;

            UpdateIndexOnNamesListUpdate
            (
                dialogueGroupNames,
                selectedDialogueGroupIndexProperty,
                selectedDialogueGroupIndexProperty.intValue,
                isOldDialogueGroupNull,
                isOldDialogueGroupNull ? "" : oldDialogueGroup.GroupName
            );

            selectedDialogueGroupIndexProperty.intValue = DSInspectorUtility.DrawPopup("Dialogue Group", selectedDialogueGroupIndexProperty.intValue, dialogueGroupNames.ToArray());

            string selectedDialogueGroupName = dialogueGroupNames[selectedDialogueGroupIndexProperty.intValue];
            DSDialogueGroupSO selectedDialogueGroup = DSSaveUtility.LoadAsset<DSDialogueGroupSO>($"Assets/DialogueSystem/Dialogues/{dialogueContainer.FileName}/Groups/{selectedDialogueGroupName}", selectedDialogueGroupName);

            dialogueGroupProperty.objectReferenceValue = selectedDialogueGroup;

            DSInspectorUtility.DrawDisabledFieldsAround(() => dialogueGroupProperty.DrawPropertyField());

            EditorGUILayout.Space(4);
        }

        private void DrawDialogueArea(List<string> dialogueNames, string dialogueFolderPath)
        {
            DSInspectorUtility.DrawHeader("Dialogue");

            DSDialogueSO oldDialogue = (DSDialogueSO)dialogueProperty.objectReferenceValue;
            bool isOldDialogueNull = oldDialogue == null;

            UpdateIndexOnNamesListUpdate
            (
                dialogueNames,
                selectedDialogueIndexProperty,
                selectedDialogueIndexProperty.intValue,
                isOldDialogueNull,
                isOldDialogueNull ? "" : oldDialogue.DialogueName
            );

            selectedDialogueIndexProperty.intValue = DSInspectorUtility.DrawPopup("Dialogue", selectedDialogueIndexProperty.intValue, dialogueNames.ToArray());

            string selectedDialogueName = dialogueNames[selectedDialogueIndexProperty.intValue];
            DSDialogueSO selectedDialogue = DSSaveUtility.LoadAsset<DSDialogueSO>(dialogueFolderPath, selectedDialogueName);

            dialogueProperty.objectReferenceValue = selectedDialogue;

            DSInspectorUtility.DrawDisabledFieldsAround(() => dialogueProperty.DrawPropertyField());
        }

        private void StopDrawing(string reason, MessageType messageType = MessageType.Info)
        {
            EditorGUILayout.HelpBox(reason, messageType);

            EditorGUILayout.Space(4);

            EditorGUILayout.HelpBox("Please select a Dialogue for this component to run properly at runtime", MessageType.Warning);

            serializedObject.ApplyModifiedProperties();

            DrawBaseInspector();
        }

        private void DrawBaseInspector()
        {

        }
        #endregion

        #region Index Methods
        private void UpdateIndexOnNamesListUpdate(List<string> optionNames, SerializedProperty indexProperty, int oldSelectedPropertyIndex, bool isOldPropertyNull, string oldPropertyName)
        {
            if (isOldPropertyNull)
            {
                indexProperty.intValue = 0;

                return;
            }

            bool oldIndexIsOutsideOfNamesList = oldSelectedPropertyIndex > optionNames.Count - 1;
            bool oldNameIsDifferentToSelectedName = oldPropertyName != optionNames[oldSelectedPropertyIndex];

            if (oldIndexIsOutsideOfNamesList || oldNameIsDifferentToSelectedName)
            {
                if (optionNames.Contains(oldPropertyName))
                {
                    indexProperty.intValue = optionNames.IndexOf(oldPropertyName);
                }
                else
                {
                    indexProperty.intValue = 0;
                }
            }
        }
        #endregion
    }
}