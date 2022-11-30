#nullable enable

using System;
using System.Text;

namespace Rano.Encoding
{
    public static class Utils
    {
        public static string EscapeString(string text)
        {
            int index;
            int textLength = text.Length;
            char character;
            
            StringBuilder stringBuilder = new StringBuilder(textLength + 4);
            String tempString;

            for (index = 0; index < textLength; index += 1)
            {
                character = text[index];
                switch (character)
                {
                    case '\\':
                    case '"':
                        stringBuilder.Append('\\');
                        stringBuilder.Append(character);
                        break;
                    case '/':
                        stringBuilder.Append('\\');
                        stringBuilder.Append(character);
                        break;
                    case '\b':
                        stringBuilder.Append("\\b");
                        break;
                    case '\t':
                        stringBuilder.Append("\\t");
                        break;
                    case '\n':
                        stringBuilder.Append("\\n");
                        break;
                    case '\f':
                        stringBuilder.Append("\\f");
                        break;
                    case '\r':
                        stringBuilder.Append("\\r");
                        break;
                    default:
                        if (character < ' ')
                        {
                            tempString = "000" + String.Format("X", character);
                            stringBuilder.Append("\\u" + tempString.Substring(tempString.Length - 4));
                        }
                        else
                        {
                            stringBuilder.Append(character);
                        }
                        break;
                }
            }
            return stringBuilder.ToString();
        }
    }
}