using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Rano.PlatformServices.GoogleDocuments
{
    public enum GoogleSheetExportFormat
    {
        CSV,
        TSV
    }

    [Serializable]
    public struct GoogleSheetId
    {
        [FormerlySerializedAs("Id")] public string id;
        [FormerlySerializedAs("Gid")] public string gid;
        [FormerlySerializedAs("Range")] public string range;
    }

    public static class GoogleSheet
    {
        /// <summary>
        /// 추출 URL을 얻는다.
        /// </summary>
        /// <remarks>
        /// https://docs.google.com/spreadsheets/d/{{ID}}/export?format=tsv&range=A14:D23
        /// https://docs.google.com/spreadsheets/d/{{ID}}/gviz/tq?tqx=out:csv&sheet=ItemDatas/// 
        /// </remarks>
        /// <example>
        /// string url = GoogleSheetManager.Instance.GetUrl("MyID", "2004765863", GoogleSheetManager.ExportFormat.TSV);
        /// </example>
        public static string GetExportUrl(string id, string gid, string range, GoogleSheetExportFormat format)
        {            
            string url;
            url = $"https://docs.google.com/spreadsheets/d/{id}/export?";
            url += $"gid={gid}";
            url += $"&format={format.ToString().ToLower()}";
            if (range != null && range != "")
            {
                url += $"&range={range}";
            }
            return url;
        }

        public static string GetExportUrl(GoogleSheetId gsid, GoogleSheetExportFormat format)
        {
            return GetExportUrl(gsid.id, gsid.gid, gsid.range, format);
        }
    }
}