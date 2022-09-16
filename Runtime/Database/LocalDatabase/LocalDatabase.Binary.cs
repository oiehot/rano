#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Rano.IO;

namespace Rano.Database
{
    public sealed partial class LocalDatabase : ILocalDatabase
    {
        public bool LoadFromBinary(byte[] bytes)
        {
            Log.Warning("이진직렬화 사용은 보안과 예외처리에서 문제가 발생할 수 있습니다.");
            
            object? data;
            Dictionary<string, object> dict;
            
            Log.Info($"로드 중... ({bytes.Length} bytes)");
            
            // 세이브 데이터를 오브젝트로 변환한다.
            data = Rano.Encoding.Binary.ConvertBinaryToObject(bytes);
            if (data == null)
            {
                Log.Warning($"바이트 데이터를 오브젝트로 변환하는데 실패");
                return false;
            }

            // 오브젝트를 사전으로 변환한다.
            try
            {
                dict = (Dictionary<string, object>)data;
            }
            catch (Exception e)
            {
                Log.Warning($"오브젝트를 사전으로 변환하는데 실패");
                Log.Exception(e);
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
            
            Log.Info($"로드 성공 ({bytes.Length} bytes)");
            return true;
        }

        public bool LoadFromBinaryFile(string filePath)
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
            byte[]? bytes;
            try
            {
                bytes = Rano.IO.LocalFile.ReadBytes(filePath);
            }
            catch (Exception e)
            {
                Log.Warning($"읽기 오류 ({filePath})");
                Log.Exception(e);
                return false;
            }

            // 읽어온 데이터로 로드
            if (bytes != null)
            {
                Log.Info($"읽기 성공 ({filePath}, {bytes.Length} bytes)");
                return LoadFromBinary(bytes);
            }
            else
            {
                Log.Warning($"읽기 실패 ({filePath})");
                return false;
            }
        }

        private Dictionary<string, object> GetDictionaryArchive()
        {
            // 데이터베이스 시스템 정보를 업데이트한다.
            object systemData = CaptureSystemData();
            _dict[SYSTEM_DATA_KEY] = systemData;
            return _dict;
        }
        
        /// <summary>
        /// 데이터베이스 전체를 바이너리 데이터로 리턴한다.
        /// </summary>
        public byte[]? GetBinaryArchive()
        {
            Log.Warning("이진직렬화 사용은 보안과 예외처리에서 문제가 발생할 수 있습니다.");
            Log.Todo("압축 및 암호화 필요.");
            Dictionary<string, object> dict = GetDictionaryArchive();
            byte[]? bytes = Rano.Encoding.Binary.ConvertObjectToBinary(dict);
            return bytes;
        }
        
        public bool SaveAsBinaryFile(string filePath)
        {
            Log.Info($"저장 중... ({filePath})");
            Log.Todo("파일스트림을 이용해서 저장해야함.");
            
            string tmpPath = $"{filePath}.tmp";
            byte[]? bytes = GetBinaryArchive();
            if (bytes == null)
            {
                Log.Warning($"이진 직렬화 실패");
                return false;
            }

            // 데이터를 파일에 쓰기.
            if (Rano.IO.LocalFile.WriteBytes(tmpPath, bytes) == false)
            {
                Log.Warning($"파일 쓰기 실패 ({tmpPath})");
                return false;
            }
            
            // 임시파일을 정식파일로 이동.
            LocalFile.Move(tmpPath, filePath, true);

            Log.Info($"저장 완료 ({filePath}, {bytes.Length} bytes)");
            return true;
        }
    }
}