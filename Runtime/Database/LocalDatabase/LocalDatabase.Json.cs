#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Rano.IO;
using Rano.Encoding;

namespace Rano.Database
{
    public sealed partial class LocalDatabase : ILocalDatabase
    {
        public string? GetJsonArchive()
        {
            Dictionary<string,object> dict = GetDictionaryArchive();
            string? jsonString = Json.ConvertObjectToString(dict);
            return jsonString;
        }

        public bool SaveAsJsonFile(string filePath)
        {
            Log.Info($"저장 중... ({filePath})");
            Log.Todo("파일스트림을 이용해서 저장해야함.");
            
            string tmpPath = $"{filePath}.tmp";
            string? jsonString = GetJsonArchive();
            if (jsonString == null)
            {
                Log.Warning($"Json 직렬화 실패");
                return false;
            }

            if (LocalFile.WriteString(tmpPath, jsonString) == false)
            {
                Log.Warning($"파일 쓰기 실패 ({tmpPath})");
                return false;
            }
            
            // 임시파일을 정식파일로 이동.
            LocalFile.Move(tmpPath, filePath, true);

            Log.Info($"저장 완료 ({filePath})");
            return true;
        }

        public bool LoadFromJson(string jsonString)
        {
            Dictionary<string,object>? dict;
            
            Log.Info($"로드 중...");
            
            // 세이브 데이터를 오브젝트로 변환한다.
            dict = Json.ConvertStringToObject<Dictionary<string,object>>(jsonString);
            if (dict == null)
            {
                Log.Warning($"Json 데이터를 오브젝트로 역직렬화 하는데 실패");
                return false;
            }

            // 기존 사전을 비우고 로드한다.
            _dict.Clear();
            _dict = dict;
            
            // 데이터베이스 시스템 정보를 복구한다.
            if (_dict.TryGetValue(SYSTEM_DATA_KEY, out object systemData) == true)
            {
                RestoreSystemData(systemData);
            }
            
            Log.Info($"로드 성공");
            return true;
        }
        
        public bool LoadFromJsonFile(string filePath)
        {
            // 파일 체크
            bool fileExists;
            try
            {
                fileExists = System.IO.File.Exists(filePath);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return false;
            }
            if (fileExists == false)
            {
                Log.Info($"파일을 찾을 수 없음 ({filePath})");
                return false;
            }

            // 파일 읽기
            Log.Info($"읽는 중... ({filePath})");
            string? jsonString;
            try
            {
                jsonString = LocalFile.ReadString(filePath);
            }
            catch (Exception e)
            {
                Log.Warning($"읽기 오류 ({filePath})");
                Log.Exception(e);
                return false;
            }

            // 읽어온 데이터로 로드
            if (jsonString != null)
            {
                Log.Info($"읽기 성공 ({filePath})");
                return LoadFromJson(jsonString);
            }
            else
            {
                Log.Warning($"읽기 실패 ({filePath})");
                return false;
            }
        }
    }
}