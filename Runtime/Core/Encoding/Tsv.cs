using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Rano.Encoding
{
    public static class Tsv
    {
        private static bool IsCommentLine(string text)
        {
            if (text.StartsWith("//")) return true;
            else return false;
        }

        private static bool IsNullLine(string text)
        {
            if (text.Length <= 0) return true;
            
            string a = text.Replace("\t", "");
            string b = a.Replace("\n", "");

            if (b.Length <= 0) return true;
            else return false;
        }

        public static string ToJson(
            string text,
            List<string> includeHeaders = null,
            List<string> excludeHeaders = null,
            Dictionary<string, string> renameHeaders = null)
        {
            StringBuilder builder = new StringBuilder();

            // Write JSON Header
            builder.Append("{\n");
            builder.Append($"\t\"items\":\n\t[\n");

            // Read Lines
            using (StringReader reader = new StringReader(text)) {
                string line;
                string[] headers;

                // Read Header
                string headerLine = reader.ReadLine();
                if (headerLine != null)
                {
                    headers = headerLine.Split('\t');
                }
                else
                {
                    throw new Exception("헤더 행이 없음");
                }

                int headerCount = headers.Length;
                if (headerCount <= 0)
                {
                    throw new Exception("헤더 데이터가 없음");
                }

                // Read Body
                bool firstItemPassed = false;
                while ((line = reader.ReadLine()) != null)
                {
                    if (Tsv.IsNullLine(line)) continue;                    
                    if (Tsv.IsCommentLine(line)) continue;

                    string[] tokens;
                    tokens = line.Split('\t');

                    // 빈 행이거나 컬럼 데이터가 불충분한 경우 다음 행
                    if (tokens.Length < headerCount) continue;

                    if (firstItemPassed)
                    {
                        builder.Append(", \n");
                    }
                    else
                    {
                        firstItemPassed = true;
                    }
                    builder.Append("\t\t{\n"); // Open Record

                    bool firstKeyPassed = false;
                    for(int i=0; i<headerCount; i++)
                    {
                        string header = headers[i];
                        string data = Utils.EscapeString(tokens[i]); // json 파싱에 문제가되는 문자의 경우 Escape 한다.
                        
                        if (header == "") continue;
                        if (data == "") continue;
                        if (excludeHeaders != null && excludeHeaders.Contains(header)) continue;
                        if (includeHeaders != null && !includeHeaders.Contains(header)) continue;

                        // TODO: 이미 헤더컬럼이 있는 케이스는 고려하지 않음.
                        if (renameHeaders != null && renameHeaders.ContainsKey(header))
                        {
                            header = renameHeaders[header];
                        }
                        
                        if (firstKeyPassed)
                        {
                            builder.Append(", \n");
                        }
                        else
                        {
                            firstKeyPassed = true;
                        }
                        builder.Append($"\t\t\t\"{header}\":");
                        builder.Append($" \"{data}\"");
                    }

                    builder.Append("\n\t\t}"); // Close Record
                }
            }

            // Write Json Footer
            builder.Append("\n");
            builder.Append("\t]\n");
            builder.Append("}\n");

            // Return Json
            return builder.ToString();

            // byte[] bytes = System.Text.Encoding.Default.GetBytes(builder.ToString());
            // return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
