#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Rano.Network;
using Rano.Encoding;

namespace Rano.GoogleSheet
{
    public abstract class UpdatableSheetSO<T> : ScriptableObject
    {
        [SerializeField] protected T[] _items = {};
        public T[] Items
        {
            get => _items;
            set => _items = value;
        }

        public int Length => _items.Length;
        public T GetByIndex(int index) => _items[index];
        
        /// <summary>
        /// 주어진 index가 1이라면 0 인덱스 항목을 리턴한다.
        /// </summary>
        public T GetByOneStartIndex(int index) => _items[index - 1];

#if UNITY_EDITOR
        protected abstract string Name { get; }
        protected abstract string ID { get; }
        protected abstract string Gid{ get; }
        protected abstract string Range { get; }
        
        /// <summary>
        /// 구글 시트의 값을 읽어 데이터를 업데이트 한다.
        /// </summary>
        public async Task<bool> UpdateFromGoogleSheetAsync()
        {
            Log.Info($"구글 시트 스크립터블 오브젝트 업데이트 중... ({Name})");
            SGoogleSheetId googleSheetId = new SGoogleSheetId
            {
                id = ID,
                gid = Gid,
                range = Range
            };
            string? csvExportUrl = googleSheetId.CsvExportUrl;
            
            if (string.IsNullOrEmpty(csvExportUrl))
            {
                Log.Warning("구글 시트 스크립터블 오브젝트 업데이트 실패 (Url을 얻지 못함)");
                return false;
            }

            string? csvString = await Http.GetStringAsync(csvExportUrl!);
            if (string.IsNullOrEmpty(csvString))
            {
                Log.Warning("구글 시트 스크립터블 오브젝트 업데이트 실패 (CSV 얻기 실패)");
                return false;
            }

            SCsvText csvText = new SCsvText(csvString!);

            Log.Info("구글 시트로 부터 얻은 CSV 파싱 중...");
            List<Dictionary<string, object>> rows = TypeCsvParser.Parse(csvText);

            Log.Info("파싱된 데이터로 스크립터블 오브젝트 업데이트 중...");

            bool updateResult;
            try
            {
                updateResult = await UpdateFromRowsAsync(rows);
            }
            catch (Exception e)
            {
                Log.Warning("구글 시트 스크립터블 오브젝트 업데이트 실패 (예외 발생)");
                Log.Exception(e);
                return false;
            }
            if (updateResult)
            {
                Log.Info("구글 시트 스크립터블 오브젝트 업데이트 성공");
                return true;
            }
            else
            {
                Log.Warning("구글 시트 스크립터블 오브젝트 업데이트 실패");
                return false;
            }
        }
        
        /// <summary>
        /// 사전 데이터 (ColumnName:Key) 이를 통해 데이터를 업데이트 한다.
        /// </summary>
        protected abstract Task<bool> UpdateFromRowsAsync(List<Dictionary<string,object>> rows);
#endif
    }
}