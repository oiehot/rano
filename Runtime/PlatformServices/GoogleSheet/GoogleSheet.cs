// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections.Generic;

namespace Rano.PlatformServices.GoogleDocuments
{
    public enum GoogleSheetExportFormat
    {
        CSV,
        TSV
    }

    public struct GoogleSheetId
    {
        public string Id;
        public string Gid;
        public string Range;
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
            return GetExportUrl(gsid.Id, gsid.Gid, gsid.Range, format);
        }
    }
}