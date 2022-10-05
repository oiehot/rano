#nullable enable

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using Rano.GoogleSheet;
using Rano.Network;
using Rano.Encoding;

namespace Rano.Editor.GoogleSheet
{
    public abstract class UpdatableGoogleSheetSOEditor<T> : UnityEditor.Editor where T : ScriptableObject
    {
        protected T _data;
        
        protected abstract string ID { get; }
        protected abstract string Gid{ get; }
        protected abstract string Range { get; }

        void OnEnable()
        {
            _data = target as T;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Google Sheet", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("ID", ID);
            EditorGUILayout.LabelField("Gid", Gid);
            EditorGUILayout.LabelField("Range", Range);
            
            if (GUILayout.Button("Update from GoogleSheet"))
            {
                UpdateFromGoogleSheetAsync().ContinueWith((task) =>
                {
                    T so = _data as T;
                    if (so != null)
                    {
                        // 수정 표시를 해야 Save 시 저장됨.
                        EditorUtility.SetDirty(so);
                    }
                    else
                    {
                        Log.Warning("수정 표시(Mark dirty) 실패");
                    }
                });
                
            }
        }
        
        /// <summary>
        /// 구글 시트의 값을 읽어 데이터를 업데이트 한다.
        /// </summary>
        private async Task<bool> UpdateFromGoogleSheetAsync()
        {
            Log.Info("스크립터블 오브젝트 업데이트 중...");
            SGoogleSheetId googleSheetId = new SGoogleSheetId
            {
                id = ID,
                gid = Gid,
                range = Range
            };
            string csvExportUrl = googleSheetId.CsvExportUrl;

            string? csvString = await Http.GetStringAsync(csvExportUrl);
            if (csvString == null)
            {
                Log.Warning("스크립터블 오브젝트 업데이트 실패 (CSV 얻기 실패)");
                return false;
            }

            CsvText csvText = new CsvText(csvString);

            Log.Info("구글 시트로 부터 얻은 CSV 파싱 중...");
            List<Dictionary<string, object>> rows = TypeCsvParser.Parse(csvText);

            Log.Info("파싱된 데이터로 스크립터블 오브젝트 업데이트 중...");

            bool updateResult = await UpdateFromRowsAsync(rows);
            if (updateResult == true)
            {
                Log.Info("스크립터블 오브젝트 업데이트 성공");
                return true;
            }
            else
            {
                Log.Warning("스크립터블 오브젝트 업데이트 실패");
                return false;
            }
        }
        
        /// <summary>
        /// 사전 데이터 (ColumnName:Key) 이를 통해 데이터를 업데이트 한다.
        /// </summary>
        protected abstract Task<bool> UpdateFromRowsAsync(List<Dictionary<string,object>> rows);
        
    }
}