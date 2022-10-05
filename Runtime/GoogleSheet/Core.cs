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

        public string CsvExportUrl => GetExportUrl(EGoogleSheetExportFormat.CSV);
        public string TsvExportUrl => GetExportUrl(EGoogleSheetExportFormat.TSV);
        
        /// <summary>
        /// 추출 URL을 얻는다.
        /// </summary>
        /// <remarks>
        /// https://docs.google.com/spreadsheets/d/{{ID}}/export?format=tsv&range=A14:D23
        /// https://docs.google.com/spreadsheets/d/{{ID}}/gviz/tq?tqx=out:csv&sheet=ItemDatas/// 
        /// </remarks>
        public string GetExportUrl(EGoogleSheetExportFormat format)
        {            
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