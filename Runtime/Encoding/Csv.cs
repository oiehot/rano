// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Rano.Encoding.Csv
{
    public struct CsvText
    {
        public string Text { get; }
        public CsvText(string text)
        {
            Text = text;
        }
    }

    public struct CsvLineText
    {
        public string Text { get; }
        public CsvLineText(string text)
        {
            Text = text;
        }
    }

    public interface ICsvImportable
    {
        void ImportCsv(CsvText csv);
    }

    public interface ICsvExportable
    {
        CsvText ExportCsv();
    }

    /// <summary>
    /// Csv 텍스트를 파싱하여 리스트로 만들어준다.
    /// </summary>
    /// <remarks>
    /// Copied from:
    ///   https://bravenewmethod.com/2014/09/13/lightweight-csv-reader-for-unity/#comment-7111
    ///   https://github.com/tikonen/blog/tree/master/csvreader
    /// </remarks>
    public sealed class CsvParser
    {
        private static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        private static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        private static char[] TRIM_CHARS = { '\"' };

        public static List<Dictionary<string, object>> Parse(CsvText csvText)
        {
            var list = new List<Dictionary<string, object>>();

            var lines = Regex.Split(csvText.Text, LINE_SPLIT_RE);
            if (lines.Length <= 1) return list;

            var header = Regex.Split(lines[0], SPLIT_RE);
            for (var i = 1; i < lines.Length; i++)
            {
                var values = Regex.Split(lines[i], SPLIT_RE);
                if (values.Length == 0 || values[0] == "") continue;

                var entry = new Dictionary<string, object>();
                for (var j = 0; j < header.Length && j < values.Length; j++)
                {
                    string value = values[j];
                    value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS);
                    value = value.Replace("\\n", "\n"); // TODO: 줄넘김 허용?
                    value = value.Replace("\\t", "\t"); // TODO: 탭 허용?
                    value = value.Replace("\\", "");
                    object finalvalue = value;

                    int n;
                    float f;
                    bool b;

                    // TODO: long Casting Problem
                    if (int.TryParse(value, out n))
                    {
                        finalvalue = n;
                    }
                    else if (float.TryParse(value, out f))
                    {
                        finalvalue = f;
                    }
                    else if (bool.TryParse(value, out b))
                    {
                        finalvalue = b;
                    }

                    entry[header[j]] = finalvalue;
                }
                list.Add(entry);
            }
            return list;
        }
    }
}