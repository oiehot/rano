#nullable enable

using System;

namespace Rano.GoogleSheet
{
    public enum EGoogleSheetExportFormat
    {
        CSV,
        TSV
    }

    [Serializable]
    public struct SGoogleSheetId
    {
        public string id;
        public string gid;
        public string range;

        public string? CsvExportUrl => GetExportUrl(EGoogleSheetExportFormat.CSV);
        public string? TsvExportUrl => GetExportUrl(EGoogleSheetExportFormat.TSV);
        
        /// <summary>
        /// 추출 URL을 얻는다.
        /// </summary>
        public string? GetExportUrl(EGoogleSheetExportFormat format)
        {
            if (string.IsNullOrEmpty(id)) return null;
            if (string.IsNullOrEmpty(gid)) return null;
            
            string url;
            url = $"https://docs.google.com/spreadsheets/d/{id}/export?";
            url += $"gid={gid}";
            url += $"&format={format.ToString().ToLower()}";
            if (string.IsNullOrEmpty(range) == false)
            {
                url += $"&range={range}";
            }
            return url;
        }
    }
}