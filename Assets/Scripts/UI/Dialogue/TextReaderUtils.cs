using System.Collections.Generic;

namespace MemoryFishing.Utilities
{
    public static class TextReaderUtils
    {
        public static string AddCharOrTag(int index, char[] fullCharArray, ref bool noParse)
        {
            char charAtIndex = fullCharArray[index];
            string textToAdd = "";

            if (charAtIndex != '<')
            {
                textToAdd += charAtIndex;
                return textToAdd;
            }
            string tagContents = AddTag(index, fullCharArray, out List<string> attributes, out _);
            string fullTag = $"<{tagContents}>";

            int lastIndex = fullTag.Length + index;

            bool noparseTagEnd = tagContents == "/noparse";
            noParse &= !noparseTagEnd;

            if (noParse)
            {
                textToAdd += charAtIndex;
                return textToAdd;
            }

            noParse |= tagContents == "noparse";
            textToAdd += fullTag;

            bool spriteTag = attributes[0] == "sprite";

            if (lastIndex >= fullCharArray.Length || spriteTag)
            {
                return textToAdd;
            }

            return textToAdd + AddCharOrTag(lastIndex, fullCharArray, ref noParse);
        }

        private static string AddTag(int index, char[] textCharArray, out List<string> attributes, out Dictionary<string, string> values)
        {
            string fullTag = "";
            string currentAttribute = "";
            string currentValue = "";

            bool isValue = false;

            attributes = new List<string>();
            values = new Dictionary<string, string>();

            for (int i = index + 1; i < textCharArray.Length; i++)
            {
                char currentChar = textCharArray[i];                

                if (currentChar == '>')
                {
                    if (isValue)
                    {
                        values.Add(attributes[^1], currentValue);
                    }
                    else
                    {
                        attributes.Add(currentAttribute);
                    }
                    break;
                }

                fullTag += currentChar;

                if (isValue)
                {
                    currentValue += currentChar;

                    if (currentChar == ' ')
                    {
                        isValue = false;
                        values.Add(attributes[^1], currentValue);
                    }

                    continue;
                }

                if (currentChar == '=' || currentChar == ' ')
                {
                    attributes.Add(currentAttribute);
                    currentAttribute = "";

                    isValue = currentChar == '=';

                    if (!isValue)
                    {
                        values.Add(attributes[^1], "");
                    }

                    continue;
                }

                currentAttribute += currentChar;
            }

            //UnityEngine.Debug.Log($"Added Tag <{fullTag}>; First Attribute: {attributes[0]};");
            return fullTag;
        }
    }
}