#nullable enable

using System;
using System.Collections.Generic;
using Rano.IO;
using Rano.IO.Compression;
using Rano.Encoding;

namespace Rano.Database
{
    public sealed partial class LocalDatabase
    {
        /// <summary>
        /// 내부 사전 데이터를 Json으로 변환하고 압축 및 암호화 처리된 바이너리로 리턴한다.
        /// </summary>
        /// <remarks>
        /// (1) 사전 > Json 문자열 > (2) 바이너리 > (3) 압축 > (4) 암호화
        /// </remarks>
        public byte[]? GetArchive()
        {
            // (1) Dictionary => JsonString
            Dictionary<string,object> dict = GetDictionaryArchive();
            string? jsonString = Json.ConvertObjectToString(dict);
            if (jsonString == null)
            {
                Log.Warning($"저장 할 데이터를 json으로 직렬화 하는데 실패함");
                return null;
            }
            
            // (2) JsonString => Bytes
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
            
            // (3) Bytes => CompressedBytes
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
            
            // (4) CompressedBytes => EncryptedBytes
            // TODO: CompressedBytes => EncryptedBytes

            return compressedBytes;
        }

        /// <summary>
        /// 압축 및 암호화 처리된 Json 바이너리를 역순으로 해체하여 내부 사전으로 로드한다.
        /// </summary>
        /// <remarks>
        /// (1) 암호해제 > (2)압축해제 > (3) 바이너리 > Json 문자열로 변환 > (4) 사전으로 역직렬화
        /// </remarks>
        public bool LoadFromArchive(byte[] archiveBytes)
        {
            // (1) ArchiveBytes == EncryptedBytes => CompressedBytes
            byte[] compressedBytes = archiveBytes;
            
            // TODO: 세이브 데이터 복호화가 필요함.
            
            // (2) CompressedBytes => Bytes
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
            
            // (3) Bytes => JsonString
            string? jsonString;
            try
            {
                jsonString = System.Text.Encoding.UTF8.GetString(bytes);
            }
            catch (Exception e)
            {
                Log.Warning("로드 실패 (Bytes를 JsonString으로 변환하는데 실패)");
                Log.Exception(e);
                return false;
            }

            // (4) JsonString을 Dictionary로 역직렬화한다.
            Dictionary<string,object>? dict = Json.ConvertStringToObject<Dictionary<string, object>>(jsonString);
            if (dict == null)
            {
                Log.Warning("로드 실패 (Json 문자열을 사전으로 역직렬화 하는데 실패)");
                return false;
            }

            // 기존 사전을 비우고 로드한다.
            _dict.Clear();
            _dict = dict;
            
            // 데이터베이스 시스템 정보를 복구한다.
            if (_dict.TryGetValue(SYSTEM_DATA_KEY, out object systemData))
            {
                RestoreSystemData(systemData);
            }
            return true;
        }

        private bool SaveAsFile(string filePath)
        {
            // TODO: 파일스트림을 이용해서 저장해야함.
            
            Log.Info($"저장 중... ({filePath})");

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
            if (LocalFile.Move(tmpPath, filePath, true) == false)
            {
                Log.Warning($"임시 파일 이동 실패 ({tmpPath} => {filePath})");
                return false;
            }

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