#nullable enable

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Rano.Encoding
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
    /// 헤더 이름에 자료형이 써진 TypeCsv 텍스트를 파싱하여 리스트로 만들어준다.
    /// </summary>
    public static class TypeCsvParser
    {
        private static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        private static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        private static char[] TRIM_CHARS = { '\"' };
        private static char[] HEADER_TRIM_CHARS = { ' ', ':', '\u0020' };
        private static NumberStyles NUMBER_STYLE = NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;

        public static List<Dictionary<string, object>> Parse(CsvText csvText)
        {
            List<Dictionary<string,object>> rows = new List<Dictionary<string, object>>();

            // 줄들을 읽는다.
            var lines = Regex.Split(csvText.Text, LINE_SPLIT_RE);
            if (lines.Length <= 1) return rows;

            // 헤더를 읽는다.
            var header = Regex.Split(lines[0], SPLIT_RE);

            // 헤더를 제외한 Row줄 하나하나 분석한다.
            for (var i = 1; i < lines.Length; i++)
            {
                var rowValues = Regex.Split(lines[i], SPLIT_RE);
                if (rowValues.Length == 0 || rowValues[0] == "") continue;

                // 셀의 데이터를 얻고 Key:Value 형태로 보관한다.
                var row = new Dictionary<string, object>();
                for (var j = 0; j < header.Length && j < rowValues.Length; j++)
                {
                    string headerFullname = header[j];

                    // 헤더명이 #으로 시작하면 그 Column은 무시한다.
                    if (headerFullname.StartsWith("#") == true) continue;

                    // 헤더명에서 데이터 타입을 얻는다.
                    var headerTokens = headerFullname.Split(HEADER_TRIM_CHARS, StringSplitOptions.RemoveEmptyEntries);
                    string headerName = headerTokens[0];
                    
                    if (headerTokens.Length != 2)
                    {
                        Log.Warning($"열({headerName}) 에 자료형이 정상적으로 지정되지 않았음");
                        continue;
                    }

                    string headerTypeName = headerTokens[1].ToLower();
                    if (headerTypeName == "")
                    {
                        Log.Warning($"열({headerName}) 에 자료형이 비어있음");
                        continue;
                    }

                    string value = rowValues[j];
                    value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS); // 좌우의 '"'를 제거한다.
                    value = value.Replace("\\n", "\n"); // \\n을 \n으로 교체한다.
                    value = value.Replace("\\t", "\t"); // \\t를 \t로 교체한다.
                    //value = value.Replace("\\", "");
                    object finalValue; // = value;

                    // 타입명에 따라 파싱을 시도한다.
                    switch (headerTypeName)
                    {
                        case "byte":
                            finalValue = byte.Parse(value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign);
                            break;

                        case "sbyte":
                            finalValue = sbyte.Parse(value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign);
                            break;

                        case "short":
                            finalValue = short.Parse(value, NUMBER_STYLE);
                            break;

                        case "ushort":
                            finalValue = ushort.Parse(value, NUMBER_STYLE);
                            break;

                        case "int":
                            finalValue = int.Parse(value, NUMBER_STYLE);
                            break;

                        case "uint":
                            finalValue = uint.Parse(value, NUMBER_STYLE);
                            break;

                        case "long":
                            finalValue = long.Parse(value, NUMBER_STYLE);
                            break;

                        case "ulong":
                            finalValue = ulong.Parse(value, NUMBER_STYLE);
                            break;

                        case "bool":
                            finalValue = bool.Parse(value);
                            break;

                        case "float":
                            finalValue = float.Parse(value, NUMBER_STYLE);
                            break;

                        case "hexcolor":
                            UnityEngine.Color color;
                            if (!UnityEngine.ColorUtility.TryParseHtmlString(value, out color))
                            {
                                Log.Warning($"컬러값의 파싱에 실패 ({value})");
                                continue;
                            }
                            finalValue = color;
                            break;

                        case "str":
                        case "string":
                            finalValue = value;
                            break;

                        default:
                            Log.Warning($"열에 잘못된 자료형이 지정됨 ({headerTypeName})");
                            continue;
                    }

                    row[headerName] = finalValue;
                }

                // 정리된 Key:Value 값들을 저장한다.
                rows.Add(row);
            }
            return rows;
        }
    }
}