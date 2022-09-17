#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Rano.IO;
using Rano.IO.Compression;
using Rano.Encoding;

namespace Rano.Database
{
    public sealed partial class LocalDatabase : ILocalDatabase
    {
        public byte[]? GetArchive()
        {
            // 1) Dictionary => JsonString
            Dictionary<string,object> dict = GetDictionaryArchive();
            string? jsonString = Json.ConvertObjectToString(dict);
            if (jsonString == null)
            {
                Log.Warning($"저장 할 데이터를 json으로 직렬화 하는데 실패함");
                return null;
            }
            
            // 2) JsonString => Bytes
            Log.Todo("이진으로 변환시 압축 및 보안처리가 필요함");
            byte[]? bytes;
            try
            {
                bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
            }
            catch (Exception e)
            {
                Log.Warning($"json 문자열을 바이트 배열로 변환하는데 실패함");
                Log.Exception(e);
                return null;
            }
            Log.Info($"세이브 데이터 바이트 직렬화 성공 ({bytes.Length} bytes)");
            
            // 3) Bytes => CompressedBytes
            byte[]? compressedBytes = GZip.Compress(bytes);
            if (compressedBytes != null)
            {
                Log.Info($"세이브 데이터 압축 성공 ({compressedBytes.Length} bytes)");    
            }
            else
            {
                Log.Warning($"세이브 데이터 압축 실패");
                return null;
            }
            
            // 4) CompressedBytes => EncryptedBytes
            Log.Todo("CompressedBytes => EncryptedBytes");

            return compressedBytes;
        }

        public bool LoadFromArchive(byte[] archiveBytes)
        {
            string? jsonString;
            Dictionary<string,object>? dict;
            
            Log.Info($"로드 중...");
            
            // 1) ArchiveBytes == EncryptedBytes => CompressedBytes
            byte[]? compressedBytes = archiveBytes;
            Log.Todo("세이브 데이터 복호화가 필요함");
            // Log.Info($"세이브 데이터 복호화 성공 ({compressedBytes.Length} bytes)");
            
            // 2) CompressedBytes => Bytes
            
            Log.Info($"세이브 데이터 압축해제 중...");
            byte[]? bytes = GZip.Decompress(compressedBytes);
            if (bytes != null)
            {
                Log.Info($"세이브 데이터 압축해제 성공 ({bytes.Length} bytes)");    
            }
            else
            {
                Log.Warning($"세이브 데이터 압축해제 실패");
                return false;
            }
            
            // 3) Bytes => JsonString
            try
            {
                Log.Todo("이진에서 JsonString으로 변환시 압축 및 보안처리 해제가 필요함");
                jsonString = System.Text.Encoding.UTF8.GetString(bytes);
            }
            catch (Exception e)
            {
                Log.Warning("로드 실패 (Bytes를 JsonString으로 변환하는데 실패)");
                Log.Exception(e);
                return false;
            }

            // jsonString을 Dictionary로 역직렬화한다.
            dict = Json.ConvertStringToObject<Dictionary<string, object>>(jsonString);
            if (dict == null)
            {
                Log.Warning("로드 실패 (Json 문자열을 사전으로 역직렬화 하는데 실패)");
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

        private bool SaveAsFile(string filePath)
        {
            Log.Info($"저장 중... ({filePath})");
            Log.Todo("파일스트림을 이용해서 저장해야함.");
            
            string tmpPath = $"{filePath}.tmp";
            byte[]? bytes = GetArchive();
            if (bytes == null)
            {
                return false;
            }

            if (LocalFile.WriteBytes(tmpPath, bytes) == false)
            {
                Log.Warning($"파일 쓰기 실패 ({tmpPath})");
                return false;
            }
            
            // 임시파일을 정식파일로 이동.
            LocalFile.Move(tmpPath, filePath, true);

            Log.Info($"저장 완료 ({filePath}, {bytes.Length} bytes)");
            return true;
        }
        
        private bool LoadFromFile(string filePath)
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
                bytes = LocalFile.ReadBytes(filePath);
            }
            catch (Exception e)
            {
                Log.Warning($"읽기 오류 ({filePath})");
                Log.Exception(e);
                return false;
            }
            
            if (bytes != null)
            {
                Log.Info($"읽기 성공 ({filePath}, bytes:{bytes.Length})");
            }
            else
            {
                Log.Warning($"읽기 실패 ({filePath})");
                return false;
            }
            
            return LoadFromArchive(bytes);
        }
    }
}