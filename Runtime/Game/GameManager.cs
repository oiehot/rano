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
using Rano.Admob;
using DG.Tweening;

namespace YOUR_PROJECT
{
    public class GameManager : Singleton<GameManager>
    {
        #pragma warning disable // Supress warning
        bool paused = false;
        bool focused = false;
        #pragma warning restore // Restore warning

        public GameData Data;
        public AudioListener audioListener {get; private set;}        
        public NetworkManager Network {get; private set;}
        public SocialManager Social {get; private set;}
        public PurchaseController Purchasing {get; private set;}
        public LocalizationManager Localization {get; private set;}
        public SoundManager Sound {get; private set;}

        void Awake()
        {
            // 📡 Network
            this.Network = this.gameObject.AddComponent<NetworkManager>();
                // ex) GameManager.Instance.Network.IsConnected()

            // 🌏 Localization
            this.Localization = this.gameObject.AddComponent<LocalizationManager>();
            /*
            LocalizationLanguage english = new LocalizationLanguage(SystemLanguage.English);
            LocalizationLanguage korean  = new LocalizationLanguage(SystemLanguage.Korean);
            LocalizationLanguage chinese = new LocalizationLanguage(SystemLanguage.Chinese);
            LocalizationFont englishFont = new LocalizationFont(Resources.Load<Font>("Fonts/EN/TinyBoxNormal"), 1.0f, 1.0f);
            LocalizationFont koreanFont  = new LocalizationFont(Resources.Load<Font>("Fonts/KR/BMDOHYEON"), 1.25f, 0.65f);
            this.Localization.AddFont(english, englishFont);
            this.Localization.AddFont(korean, koreanFont);
            this.Localization.SetLanguage(this.Localization.GetSystemLanguage());
            // managthis.Localizationer.SetLanguage(chinese); // TODO: Temporary
            // this.Localization.SetLanguage(english); // TODO: Temporary
            // this.Localization.SetLanguage(korean); // TODO: Temporary
            this.Localization.LoadFromLocalizationDataTsv($"Sheets/LocalizationData", this.Localization.GetCurrentLanguage());
            */
            
            // 👩‍👩‍👧‍👦 Social
            this.Social = this.gameObject.AddComponent<SocialManager>();
            
            // 📦 Purchasing
            this.Purchasing = this.gameObject.AddComponent<PurchaseManager>();

            // 📣 Admob
            this.Admob = this.gameObject.AddComponent<AdmobManager>();
            /*
            this.Admob.appId = Constants.Admob.appId;
            FreeBombRewardAdManager reward = gameObject.AddComponent<FreeBombRewardAdManager>();
            reward.androidAdUnitId     = Constants.Admob.FreeBombReward.androidAdUnitId;
            reward.androidTestAdUnitId = Constants.Admob.FreeBombReward.androidTestAdUnitId;
            reward.iosAdUnitId         = Constants.Admob.FreeBombReward.iosAdUnitId;
            reward.iosTestAdUnitId     = Constants.Admob.FreeBombReward.iosTestAdUnitId;
            reward.otherAdUnitId       = Constants.Admob.FreeBombReward.otherAdUnitId;
            reward.otherTestAdUnitId   = Constants.Admob.FreeBombReward.otherTestAdUnitId;
            reward.deviceId            = Constants.Admob.FreeBombReward.deviceId;

            // 보상형 광고 설정 (Continue)
            ContinueRewardAdManager continueReward = gameObject.AddComponent<ContinueRewardAdManager>();
            continueReward.androidAdUnitId     = Constants.Admob.ContinueReward.androidAdUnitId;
            continueReward.androidTestAdUnitId = Constants.Admob.ContinueReward.androidTestAdUnitId;
            continueReward.iosAdUnitId         = Constants.Admob.ContinueReward.iosAdUnitId;
            continueReward.iosTestAdUnitId     = Constants.Admob.ContinueReward.iosTestAdUnitId;
            continueReward.otherAdUnitId       = Constants.Admob.ContinueReward.otherAdUnitId;
            continueReward.otherTestAdUnitId   = Constants.Admob.ContinueReward.otherTestAdUnitId;
            continueReward.deviceId            = Constants.Admob.ContinueReward.deviceId;
            */

            // 🔊 Sound
            this.audioListener = this.gameObject.AddComponent<AudioListener>();
            AudioListener.volume = Constants.Sound.mainVolume;          
            this.Sound = this.gameObject.AddComponent<SoundManager>();
            // this.Sound.AddLayer("Music", 0.35f);
            // this.Sound.AddLayer("Sounds", 1.0f);

            // Dotween
            Log.Important("Setup DOTween");
            DOTween.Init();

            // 📺 Screenshot
            // this.gameObject.AddComponent<Rano.ScreenshotController>();

            // 💾 GameData
            this.Data = new GameData();
        }

        void OnEnable()
        {
            Log.Info("GameManager OnEnable");
            this.Network.OnNetworkConnected += this.OnNetworkConnected;
            this.Network.OnNetworkDisconnected += this.OnNetworkDisconnected;
            this.Network.Resume();
            this.Social.OnSignInSuccess += this.OnSignInSuccess;
            this.Social.OnSignInFailed += this.OnSignInFailed;
        }

        void OnDisable()
        {
            Log.Info("GameManager OnDisable");
            this.Social.OnSignInFailed -= this.OnSignInFailed;
            this.Social.OnSignInSuccess -= this.OnSignInSuccess;
            this.Network.Pause();
            this.Network.OnNetworkConnected = this.OnNetworkConnected;
            this.Network.OnNetworkDisconnected -= this.OnNetworkDisconnected;
        }

        public void OnNetworkConnected()
        {
            Log.Info("Network Connected");
            // TODO: Init 콜백이 있는가?
            // TODO: n 초 후에 > 로그인 여부 체크 > 로그인이 안되어 있다면 > 로그인 시도
            this.Social.Init();
            this.Social.SignIn();
            // 네트워크가 연결되면 바로 광고를 로드한다.
            // FreeBombRewardAdManager.Instance.CreateAndLoadAd();
            // ContinueRewardAdManager.Instance.CreateAndLoadAd();
        }

        public void OnNetworkDisconnected()
        {
            Log.Warning("Network Disconnected");
        }

        void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                Log.Info("Game Paused");
                this.paused = true;
                // 여기서 저장요청을 하더라도 앱이 먼저 끝나기 때문에
                // 저장완료를 확인할 수 없다. 따라서 리스크를 줄이기 위해
                // 이 시점에서 클라우드 세이브를 하지 않는다.
                // this.SaveGame();
            }
            else
            {
                Log.Info("Game Resumed"); 
                this.paused = false;
                // // TODO: 소셜이 로그인되어있지 않다면 다시 로그인한다.
                // // TODO: 로그인이 다시 되면서 클라우드 데이터를 로그인한다.
                // // TODO: 이 코드가 필요한지 확실하지 않음.
                // if(!this.socialManager.IsSigned)
                // {
                //     Log.Warning("Game Resume successfully completed. However, you are logged out of your social network. Re-login to social");
                //     this.socialManager.SignIn();
                // }
            }
        }

        void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                this.focused = true;
            }
            else {
                this.focused = false;
            }
        }

        /// <summary>
        /// iOS 에서 Player Settings > Exit on Suspend 를 설정하지 않았다면
        /// OnApplicationQuit 가 호출되지 않는다. 대신 OnApplicationPause가 호출된다.
        /// iOS 앱은 보통 Quit 하지 않고 Suspend 된다.
        /// </summary>
        void OnApplicationQuit()
        {
            Log.Info("Game Quit");
        }

        void Start()
        {
            this.Network.Init();
            this.Network.Run();

            // 일단 로컬 데이터를 로드한다.
            // 이후 소셜에 로그인 되면 OnSignInSuccess가 호출되며 클라우드 데이터를 로드한다.
            try
            {
                data.LoadLocal();
            }
            catch (Exception e)
            {
                Log.Warning(e.Message);
                Log.Warning("Use Default GameData");
                data.SetDefaultValues();
            }
        }

        public void SaveGame()
        {
            try
            {
                data.Save();
            }
            catch (Exception e)
            {
                Log.Warning(e.Message);
            }
        }

        public void OnSignInSuccess()
        {
            // 소셜 로그인에 성공하면 즉시 클라우드로 부터 데이터를 로드한다.
            // 단, 클라우드와 로컬의 저장 날짜를 비교하여 로드 할지 말지를 결정한다.
            try
            {
                data.LoadCloud();
            }
            catch (Exception e)
            {
                Log.Warning(e.Message);
            }
        }

        public void OnSignInFailed()
        {
            // Pass
            Log.Warning("Social SignIn Failed. Cloud data was not used");
        }
    }
}

#endif