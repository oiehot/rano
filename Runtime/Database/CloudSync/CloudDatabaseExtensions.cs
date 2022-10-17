#nullable enable

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Firebase.Firestore;

namespace Rano.Database.CloudSync
{
    public static class CloudDatabaseExtensions
    {
        /// <summary>
        /// 특정 사용자의 최근 데이터베이스 수정 날짜를 가져온다.
        /// </summary>
        /// <remarks>UTC DateTime 값을 리턴한다.</remarks>
        public static async Task<DateTime?> GetLastModifiedDateTimeAsync(this ICloudDatabase cloud, string userId)
        {
            // 데이터베이스로 부터 시스템 사전을 얻어온다.
            Dictionary<string, object>? resultDict = await cloud.ReadDocumentAsync(Constants.USERS_COLLECTION_NAME, userId);
            if (resultDict == null) return null;
            
            // 시스템 사전에서 마지막 저장 데이터를 얻는다.
            if (resultDict.TryGetValue(Constants.LAST_MODIFIED_TIMESTAMP_KEY, out object timeStampObject))
            {
                // 오브젝트를 Timestamp를 거쳐 DateTime으로 변환한다.
                DateTime dateTime;                
                try
                {
                    Timestamp timestamp = (Timestamp)timeStampObject;
                    dateTime = timestamp.ToDateTime().ToUniversalTime();                    
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                    return null;
                }
                return dateTime;
            }
            return null;
        }
    }
}