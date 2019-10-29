using System.Text; // StringBuilder

public static class StringExtensionMethods
{
    public static string Repeat(this string source, int multiplier)
    {
        if (multiplier <= 0) {
            return "";
        }
        else {
            StringBuilder sb = new StringBuilder(multiplier * source.Length);
            for (int i = 0; i < multiplier; i++)
            {
                sb.Append(source);
            }
            return sb.ToString();
        }
    }
}