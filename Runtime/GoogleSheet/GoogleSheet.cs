// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

namespace Rano
{
    public static class GoogleSheet
    {
        public enum ExportFormat
        {
            CSV,
            TSV
        }

        /// <summary>
        /// https://docs.google.com/spreadsheets/d/{{ID}}/export?format=tsv&range=A14:D23
        /// https://docs.google.com/spreadsheets/d/{{ID}}/gviz/tq?tqx=out:csv&sheet=ItemDatas
        /// </summary>
        /// <example>
        /// string url = GoogleSheetManager.Instance.GetUrl("MyID", "2004765863", GoogleSheetManager.ExportFormat.TSV);
        /// </example>
        public static string GetUrl(string id, string gid, string range=null, ExportFormat format=ExportFormat.TSV)
        {            
            string url;
            url = $"https://docs.google.com/spreadsheets/d/{id}/export?";
            url += $"gid={gid}";
            url += $"&format={format.ToString().ToLower()}";
            if (range != null)
            {
                url += $"&range={range}";
            }
            return url;
        }

        // public void Download(string url)
        // {
        //     StartCoroutine(nameof(CoDownload), url);
        // }

        // IEnumerator CoDownload(string url)
        // {
        //     UnityWebRequest www = UnityWebRequest.Get(url);
        //     yield return www.SendWebRequest();
        //     string data = www.downloadHandler.text;
        //     print(data);
        // }

        // void Start()
        // {
        //     string url = GoogleSheetManager.Instance.GetUrl("MyID", "2004765863");
        //     Log.Important(url);
        //     GoogleSheetManager.Instance.Download(url);
        // }
    }
}