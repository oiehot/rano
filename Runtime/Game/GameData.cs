// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

#if false

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rano;
using GooglePlayGames.BasicApi.SavedGame;

namespace YOUR_PROJECT
{
    [System.Serializable]
    public class GameDataBlock
    {
        public int bombPieceCount;
        public int bombCount;
        public float maxTimeCount;
        public string timestamp;
    }
    
    public class GameData : IGameData
    {
        CloudSave cloud;
        public ReactiveProperty<int> bombPieceCountMax;
        public ReactiveProperty<int> bombPieceCount;
        public ReactiveProperty<int> bombCount;
        public ReactiveProperty<float> timeCount;
        public ReactiveProperty<float> maxTimeCount;
        public DateTime lastTimestamp;
                
        public GameData()
        {
            cloud = new CloudSave();

            // 기본값
            bombPieceCountMax = new ReactiveProperty<int>(0);
            bombPieceCount = new ReactiveProperty<int>(0);
            bombCount = new ReactiveProperty<int>(0);
            timeCount = new ReactiveProperty<float>(0.0f);
            maxTimeCount = new ReactiveProperty<float>(0.0f);
            lastTimestamp = new DateTime(1970,1,1,0,0,0); // Default DateTime
        }

        public void SetDefaultValues()
        {
            this.bombPieceCountMax.Value = Constants.GamePlay.defaultBombPieceMax;
            this.bombPieceCount.Value = 0;
            this.bombCount.Value = Constants.GamePlay.defaultBombCount;
            this.timeCount.Value = 0.0f;
            this.maxTimeCount.Value = 0.0f;
            this.lastTimestamp = new DateTime(1970,1,1,0,0,0); // Default DateTime
        }

        public void LoadLocal()
        {
            GameDataBlock data;
            DateTime timestamp;
            
            try
            {
                data = Rano.File.LocalSave.LoadFromJson<GameDataBlock>("data.json");
            }
            catch (Exception e)
            {
                throw new Exception($"Load GameData from Local Failed: {e.Message}");
            }

            try
            {
                timestamp = System.DateTime.Parse(data.timestamp);
            }
            catch
            {
                throw new Exception("Unable to obtain timestamp value from data");
            }
            
            // 로컬 데이터가 더 최신이면 그것을 적용한다.
            if (timestamp > this.lastTimestamp)
            {
                this.bombPieceCount.Value = data.bombPieceCount;
                this.bombCount.Value = data.bombCount;
                this.maxTimeCount.Value = data.maxTimeCount;
                this.lastTimestamp = timestamp;
                Log.Important("Load GameData from Local: Success");
            }
            else if (timestamp == this.lastTimestamp)
            {
                Log.Info("Load GameData from Local:: Same(Skip)");
            }            
            else
            {
                throw new Exception("The Local data is older and will not be loaded");
            }
        }

        public void LoadCloud()
        {
            if (GameManager.Instance.socialManager.IsSigned)
            {
                cloud.LoadFromJson<GameDataBlock>("data.json", (status, data) => {
                    if (status == SavedGameRequestStatus.Success)
                    {
                        // 데이터 유무확인
                        if (data == null)
                        {
                            Log.Warning("There is no data stored in the cloud");
                            throw new Exception("There is no data stored in the cloud");
                        }

                        // 데이터에서 타임스탬프 얻기
                        DateTime timestamp;
                        try
                        {
                            timestamp = System.DateTime.Parse(data.timestamp);
                        }
                        catch
                        {
                            Log.Warning("Unable to obtain timestamp value from data");
                            throw new Exception("Unable to obtain timestamp value from data");
                        }

                        // 클라우드 데이터가 더 최신이면 그것을 적용한다.
                        if (timestamp > this.lastTimestamp)
                        {
                            this.bombPieceCount.Value = data.bombPieceCount;
                            this.bombCount.Value = data.bombCount;
                            this.maxTimeCount.Value = data.maxTimeCount;                           
                            this.lastTimestamp = timestamp;               
                            Log.Important("Load GameData from Cloud: Success");
                        }
                        else if (timestamp == this.lastTimestamp)
                        {
                            Log.Info("Load GameData from Cloud: Skip(Same)");
                        }
                        else
                        {
                            Log.Warning("The cloud data is older and will not be loaded");
                            throw new Exception("The cloud data is older and will not be loaded");
                        }
                    }
                    else if (status == SavedGameRequestStatus.AuthenticationError)
                    {
                        Log.Warning("Load GameData from Cloud Failed: Authentication Error");
                        throw new Exception("Load GameData from Cloud Failed: Authentication Error");
                    }
                    else if (status == SavedGameRequestStatus.InternalError)
                    {
                        Log.Warning("Load GameData from Cloud Failed: Internal Error");
                        throw new Exception("Load GameData from Cloud Failed: Internal Error");
                    }
                    else
                    {
                        Log.Warning("Load GameData from Cloud Failed: Unknown Reason");
                        throw new Exception("Load GameData from Cloud Failed: Unknown Reason");
                    }
                });
            }
            else
            {
                Log.Warning("Unable to load data because the social is unavailable");
                throw new Exception("Unable to load data because the social is unavailable");
            }
        }
                
        public void Save()
        {
            GameDataBlock data = new GameDataBlock();
            data.bombPieceCount = this.bombPieceCount.Value;
            data.bombCount = this.bombCount.Value;
            data.maxTimeCount = this.maxTimeCount.Value;
            data.timestamp = Rano.DateTimeUtils.CurrentDateTimeString();

            // Save to Local
            try
            {
                Rano.File.LocalSave.SaveToJson<GameDataBlock>("data.json", data);
                Log.Important("Save GameData to Local: Success");
            }
            catch (Exception e)
            {
                throw new Exception($"Save GameData to Local Failed: {e.Message}");
            }

            // Network Test
            if (!GameManager.Instance.networkManager.connected)
            {
                throw new Exception("Internet not available. Save to Cloud Failed");
            }

            // Save To Cloud
            if (GameManager.Instance.socialManager && GameManager.Instance.socialManager.IsSigned)
            {
                cloud.SaveToJson<GameDataBlock>("data.json", data, (status, metadata) => {
                    if (status == SavedGameRequestStatus.Success)
                    {
                        Log.Important("Save GameData to Cloud: Success");
                    }
                    else if (status == SavedGameRequestStatus.AuthenticationError)
                    {
                        Log.Warning("Save GameData to Cloud Failed: Authentication Error");
                        throw new Exception("Save GameData to Cloud Failed: Authentication Error");
                    }
                    else if (status == SavedGameRequestStatus.InternalError)
                    {
                        Log.Warning("Save GameData to Cloud Failed: Internal Error");
                        throw new Exception("Save GameData to Cloud Failed: Internal Error");
                    }
                    else
                    {
                        Log.Warning("Save GameData to Cloud Failed: Unknown Reason");
                        throw new Exception("Save GameData to Cloud Failed: Unknown Reason");
                    }
                });
            }
            else
            {
                throw new Exception("You are not logged in to social. Save to Cloud Failed");
            }
        }
    }
}

#endif