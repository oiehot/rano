#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rano.Auth;
using Firebase.Database;
using UnityEngine;

namespace Rano.Leaderboard
{
    /// <see cref="https://firebase.google.com/docs/database/unity/save-data?hl=ko&authuser=0"/>
    public sealed class LeaderboardManager : ManagerComponent
    {
        private const string USERS = "users";
        
        private IAuthManager? _auth;
        private FirebaseDatabase? _db;
        private DatabaseReference? _root;
        public bool IsInitialized => (_auth != null && _db != null && _root != null);
        public bool IsAuthentiacted => (_auth != null ? _auth.IsAuthenticated : false);
        public string? UserId => _auth?.UserId;
        public string? UserDisplayName => "OIEHOT"; // TODO: _auth?.UserDisplayName;

        /// <summary>
        /// 초기화 한다.
        /// </summary>
        /// <param name="authManager">UserId를 얻기 위해서 필요하다</param>
        /// <returns>초기화 결과</returns>
        public bool Initialize(IAuthManager authManager)
        {
            Log.Info("리더보드 초기화중...");
            _auth = authManager;
            _db = null;
            _root = null;
            
            try
            {
                #if UNITY_EDITOR
                    const string firebaseDatabaseUrl = "https://bigtree-56229591-default-rtdb.firebaseio.com";
                    Log.Todo("에디터 환경에서 실행하면 google-services.json를 통해 실시간 데이터베이스 주소를 얻지 못하는 문제가 발생함.");
                    Log.Todo($"다음 FirebaseDatabaseUrl을 사용하여 FirebaseDatabase 인스턴스를 얻음 ({firebaseDatabaseUrl})");
                    _db = FirebaseDatabase.GetInstance(firebaseDatabaseUrl);
                #else
                    _db = FirebaseDatabase.DefaultInstance;
                #endif
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }

            if (_db == null)
            {
                Log.Warning("실시간 데이터베이스 인스턴스를 얻는데 실패함");
                Log.Warning("리더보드 초기화 실패");
                return false;
            }

            try
            {
                _root = _db.RootReference;
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }

            if (_root == null)
            {
                Log.Warning("실시간 데이터베이스 루트 문서를 얻는데 실패함");
                Log.Warning("리더보드 초기화 실패");
                return false;
            }
            
            Log.Info("리더보드 초기화 성공");
            return true;
        }

        public async Task<bool> LogLeaderboardAsync(string leaderboardName, int count)
        {
            if (IsInitialized == false)
            {
                Log.Warning("초기화가 안되어 있어서 생략함.");
                return false;
            }

            if (IsValidKey(leaderboardName) == false)
            {
                Log.Warning($"잘못된 키 이름임 ({leaderboardName})");
                return false;
            }
                
            Query query = _root!.Child(USERS).OrderByChild("northpole_highscore").LimitToLast(count);
            // Task<DataSnapshot> dataSnapshot = await query.Reference.GetValueAsync();
            DataSnapshot result = await query.GetValueAsync();
            
            foreach (DataSnapshot dataSnapshot in result.Children)
            {
                string userDisplayName = dataSnapshot.Key;
                IDictionary dict = (IDictionary)dataSnapshot.Value;
                Log.Info($"User: {userDisplayName}, {leaderboardName}: {dict[leaderboardName]}");
                // JSON은 사전 형태이기 때문에 딕셔너리 형으로 변환
            }

            return true;
        }
        
        public async Task<bool> SetStringByDictionary(string leaderboardName, string value)
        {
            Debug.Assert(IsInitialized && IsAuthentiacted && IsValidKey(leaderboardName) && UserId != null);
            
            Dictionary<string, object> updates = new Dictionary<string, object>();
            Dictionary<string, object> data = new Dictionary<string, object>();
            
            data[leaderboardName] = value;
            updates[$"/users/{UserId}"] = data;
        
            try
            {
                await _root.UpdateChildrenAsync(updates);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return false;
            }
        
            return true;
        }
        
        
        public async Task<bool> SetString(string leaderboardName, string value)
        {
            Debug.Assert(IsInitialized && IsAuthentiacted && IsValidKey(leaderboardName) && UserId != null);

            try
            {
                await _root.Child(USERS).Child(UserId).Child("leaderboardName").SetValueAsync(value);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return false;
            }
            return true;
        }

        public async Task<bool> SetLong(string leaderboardName, long value)
        {
            Debug.Assert(IsInitialized && IsAuthentiacted && IsValidKey(leaderboardName) && UserId != null);

            try
            {
                await _root.Child(USERS).Child(UserId).Child("leaderboardName").SetValueAsync(value);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return false;
            }
            return true;
        }

        // public async Task<bool> SetRawJson(string key, string value)
        // {
        //     Debug.Assert(IsInitialized && IsAuthentiacted && IsValidKey(key) && UserId != null);
        //
        //     try
        //     {
        //         await _root!.Child(USERS).Child(UserId).Child(key).SetRawJsonValueAsync(value);
        //     }
        //     catch (Exception e)
        //     {
        //         Log.Exception(e);
        //         return false;
        //     }
        //
        //     return true;
        // }

        public bool IsValidKey(string key)
        {
            return true;
        }
        
    }
}
