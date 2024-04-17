using UnityEngine;

using TMPro;

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

        public void ReadText(string text)
        {
            textCharArray = text.ToCharArray();

            currentText = AddCharOrTag(0, "", out charProgress, out _);

            timeSinceLetterAdded = 0;
        }

        private void UpdateText(float delta)
        {
            float rate = 1f / lettersPerSecond;
            int lettersToAdd = Mathf.FloorToInt(timeSinceLetterAdded / rate);

            if (lettersToAdd > 0)
            {
                bool addedTag = false;

                for (int i = 0; i < lettersToAdd; i++)
                {
                    charProgress++;

                    if (IsTextFinished)
                    {
                        textComponent.text = currentText;
                        return;
                    }

                    currentText = AddCharOrTag(charProgress, currentText, out charProgress, out addedTag);
                }
                
                if (!addedTag)
                {
                    timeSinceLetterAdded %= rate;
                }
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

        private string AddCharOrTag(int index, string text, out int lastIndex, out bool addedTag)
        {
            char first = textCharArray[index];
            lastIndex = textCharArray.Length - 1;

            if (first != '<')
            {
                text += first;
                lastIndex = index;

                addedTag = false;

                return text;
            }

            string tag = "" + first;

            for (int i = index + 1; i < textCharArray.Length; i++)
            {
                char current = textCharArray[i];
                tag += current;

                if (current == '>')
                {
                    lastIndex = i;
                    break;
                }
            }

            text += tag;

            addedTag = true;

            return text;
        }
    }
}