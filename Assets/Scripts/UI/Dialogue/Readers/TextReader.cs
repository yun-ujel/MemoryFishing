using UnityEngine;

using TMPro;
using static MemoryFishing.Utilities.TextReaderUtils;

namespace MemoryFishing.UI.Dialogue
{
    public class TextReader : DialogueReader
    {
        public class OnTextFinishedEventArgs : System.EventArgs
        {

        }

        public event System.EventHandler<OnTextFinishedEventArgs> OnTextFinishedEvent;

        [SerializeField] private TextMeshProUGUI textComponent;

        [Space]

        [SerializeField, Range(0f, 100f)] private float lettersPerSecond = 10f;
        private float timeSinceLetterAdded;

        [SerializeField, Range(0f, 1f)] private float delayOnTextFinished = 0.2f;

        private char[] textCharArray;
        private string targetText;
        private string currentText;
        private int charProgress;

        public bool IsTextFinished => charProgress >= textCharArray.Length;
        public bool HasText => textCharArray != null && textCharArray.Length > 0;

        private bool noParse = false;

        public void ReadText(string text)
        {
            textCharArray = text.ToCharArray();
            targetText = text;

            string addText = AddCharOrTag(0, textCharArray, ref noParse);
            charProgress = addText.Length;
            currentText = addText;

            timeSinceLetterAdded = 0;
        }

        private void UpdateText()
        {
            float rate = 1f / lettersPerSecond;
            int lettersToAdd = Mathf.FloorToInt(timeSinceLetterAdded / rate);

            if (lettersToAdd > 0)
            {
                for (int i = 0; i < lettersToAdd; i++)
                {
                    if (IsTextFinished)
                    {
                        textComponent.text = currentText;
                        OnTextFinishedEvent?.Invoke(this, new OnTextFinishedEventArgs());
                        return;
                    }

                    string addText = AddCharOrTag(charProgress, textCharArray, ref noParse);
                    currentText += addText;
                    charProgress += addText.Length;
                }

                timeSinceLetterAdded %= rate;
            }

            textComponent.text = currentText;
        }

        private void Update()
        {
            if (HasText && !IsTextFinished)
            {
                UpdateText();
            }

            timeSinceLetterAdded += Time.deltaTime;
        }

        public void ForceFinishText()
        {
            if (IsTextFinished && HasText && timeSinceLetterAdded < delayOnTextFinished)
            {
                return;
            }

            currentText = targetText;
            charProgress = textCharArray.Length;
            timeSinceLetterAdded = delayOnTextFinished;

            textComponent.text = currentText;

            OnTextFinishedEvent?.Invoke(this, new OnTextFinishedEventArgs());
        }

        public override void OnStartDialogue(object sender, DialogueController.OnStartDialogueEventArgs args)
        {
            ReadText(args.Text);
        }
    }
}