using UnityEngine;

using TMPro;
using static MemoryFishing.Utilities.TextReaderUtils;

namespace MemoryFishing.UI.Dialogue
{
    public class TextReader : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textComponent;

        [Space]

        [SerializeField, Range(0f, 100f)] private float lettersPerSecond = 10f;
        private float timeSinceLetterAdded;

        private char[] textCharArray;
        private string currentText;
        private int charProgress;

        private bool IsTextFinished => charProgress == textCharArray.Length;
        private bool HasText => textCharArray != null && textCharArray.Length > 0;

        private bool noParse = false;

        public void ReadText(string text)
        {
            textCharArray = text.ToCharArray();

            string addText = AddCharOrTag(0, textCharArray, ref noParse);
            charProgress = addText.Length;
            currentText = addText;

            timeSinceLetterAdded = 0;
        }

        private void UpdateText(float delta)
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
                        return;
                    }

                    string addText = AddCharOrTag(charProgress, textCharArray, ref noParse);
                    currentText += addText;
                    charProgress += addText.Length;
                }

                timeSinceLetterAdded %= rate;
            }

            timeSinceLetterAdded += delta;

            textComponent.text = currentText;
        }

        private void Update()
        {
            if (HasText && !IsTextFinished)
            {
                UpdateText(Time.deltaTime);
            }
        }
    }
}