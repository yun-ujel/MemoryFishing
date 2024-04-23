public static class TextReaderExtensions
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
        string tagContents = AddTag(index, fullCharArray, charAtIndex);
        string fullTag = $"<{tagContents}>";

        int lastIndex = fullTag.Length + index;

        bool noparseTagEnd = tagContents == "/noparse";
        if (noParse && !noparseTagEnd)
        {
            textToAdd += charAtIndex;
            return textToAdd;
        }

        noParse = tagContents == "noparse";
        textToAdd += fullTag;

        textToAdd += fullTag;

        if (lastIndex >= fullCharArray.Length)
        {
            return textToAdd;
        }

        return textToAdd + AddCharOrTag(lastIndex, fullCharArray, ref noParse);
    }

    private static string AddTag(int index, char[] textCharArray, char first)
    {
        string tagContents = "";

        for (int i = index + 1; i < textCharArray.Length; i++)
        {
            char current = textCharArray[i];

            if (current == '>')
            {
                break;
            }

            tagContents += current;
        }

        return tagContents;
    }
}
