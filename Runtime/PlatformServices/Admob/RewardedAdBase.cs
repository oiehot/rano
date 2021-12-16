// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine;

namespace Rano.PlatformServices.Admob
{
    /// <summary>
    /// 이벤트 핸들러는 Admob 스레드에서 실행되므로 핸들러 안에서는 유니티 엔진 라이브러리를 사용해서는 안된다.
    /// 우리는 이 문제를 핸들러에서는 플래그를 세우고 메인스레드에서 작동되는 Update에서 사용자 이벤트 함수를 호출하는 식으로 해결했다.
    /// 다음 글을 참고할것: https://ads-developers.googleblog.com/2016/04/handling-android-ad-events-in-unity.html
    /// </summary>
    [RequireComponent(typeof(CanvasSorter))]
    public class RewardedAdBase : MonoBehaviour
    {
        /// <summary>
        /// 총 광고 로드 횟수.
        /// </summary>
        protected int _adLoadCount = 0;

        /// <summary>
        /// 광고 객체.
        /// </summary>
        protected RewardedAd _ad;

        /// <summary>
        /// 광고 출력시 게임UI SortingOrder를 일괄적으로 변경하기 위해 이 컴포넌트가 필요함.
        /// </summary>
        private CanvasSorter _canvasSorter;

        /// <summary>
        /// 광고 유닛Id.
        /// </summary>
        protected string _adUnitId;

        private bool _adLoadedFlag = false;
        private bool _adOpeningFlag = false;
        private bool _adRewardFlag = false;
        private bool _adClosedFlag = false;
        private bool _adFailedToLoadFlag = false;
        private bool _adFailedToShowFlag = false;
        private int _adRewardAmount;
        private string _adRewardUnit;

        /// <summary>
        /// 광고 이름.
        /// </summary>
        [SerializeField] protected string _adName;

        /// <summary>
        /// 안드로이드용 광고유닛Id.
        /// </summary>
        [SerializeField] protected string _androidAdUnitId;

        /// <summary>
        /// iOS용 광고유닛Id.
        /// </summary>
        [SerializeField] protected string _iosAdUnitId;

        protected virtual void Awake()
        {
            _canvasSorter = this.GetRequiredComponent<CanvasSorter>();
        }

        protected virtual void Start()
        {

#if (UNITY_ANDROID && !DEVELOPMENT_BUILD)

            if (_androidAdUnitId != null)
            {
                _adUnitId = _androidAdUnitId;
            }
            else
            {
                throw new Exception($"{_adName} - 안드로이드 광고Id가 없음.");
            }

#elif (UNITY_ANDROID && DEVELOPMENT_BUILD)

            _adUnitId = "ca-app-pub-3940256099942544/5224354917"; // Android 테스트광고

#elif (UNITY_IOS && !DEVELOPMENT_BUILD)

            if (_iosAdUnitId != null)
            {
                _adUnitId = _iosAdUnitId;
            }
            else
            {
                throw new Exception("iOS 광고Id가 없음");
            }

#elif (UNITY_IOS && DEVELOPMENT_BUILD)

            _adUnitId = "ca-app-pub-3940256099942544/1712485313"; // iOS 테스트광고

#else

            _adUnitId = "ca-app-pub-3940256099942544/5224354917"; // Android 테스트광고

#endif
        }

        /// <<summary>
        /// Admob스레드에서 호출된 콜백에서 켜진 플래그를 체크하여 매칭되는 함수를 메인스레드에서 실행한다.
        /// </summary>
        protected virtual void Update()
        {
            if (_adLoadedFlag)
            {
                _adLoadedFlag = false;
                OnAdLoaded();
            }

            if (_adOpeningFlag)
            {
                _adOpeningFlag = false;
                // 전면보상광고 캔버스의 기본 SortingOrder 값은 0이다.
                // 게임에서 사용하는 캔버스의 기본 SortingOrder 값도 0이므로
                // 두 캔버스의 렌더링 우선순위가 애매해져서 광고 위에 게임UI가 그려질 수 있다.
                // 이런 문제점을 해결하기 위해서 미래 등록해둔 게임UI 캔버스들의 SortingOrder를
                // 일괄적으로 아래로 내려줘서 전면광고가 가장 위에 그려질 수 있도록 수정한다.
                _canvasSorter.MoveSortingOrder(-10);
                OnAdOpening();
            }

            if (_adClosedFlag)
            {
                _adClosedFlag = false;
                // 수정했던 모든 캔버스SortingOrder를 원래 위치로 돌려놓는다.
                _canvasSorter.ResetSortingOrder();
                OnAdClosed();
                _ad = null;
            }

            if (_adRewardFlag)
            {
                _adRewardFlag = false;
                OnAdReward(_adRewardAmount, _adRewardUnit);
                _adRewardAmount = 0;
                _adRewardUnit = null;
            }

            if (_adFailedToLoadFlag)
            {
                _adFailedToLoadFlag = false;
                OnAdFailedToLoad();
            }

            if (_adFailedToShowFlag)
            {
                _adFailedToShowFlag = false;
                OnAdFailedToShow();
            }
        }

        /// <summary>광고 로드가 완료될 때 실행된다.</summary>
        protected virtual void HandleAdLoaded(object sender, EventArgs args)
        {
            _adLoadedFlag = true;
            _adLoadCount++;
        }

        /// <summary>광고 로드에 실패할 때 실행된다.</summary>
        protected virtual void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            _adFailedToLoadFlag = true;
        }

        /// <summary>
        /// 광고가 표시될 때 기기 화면을 덮는다.
        /// 이때 필요한 경우 오디오 출력 또는 게임 루프를 일시중지하는 것이 좋다.
        /// </summary>
        protected virtual void HandleAdOpening(object sender, EventArgs args)
        {
            _adOpeningFlag = true;
        }

        /// <summary>광고 표시에 실패할 때 실행된다.</summary>
        protected virtual void HandleAdFailedToShow(object sender, AdErrorEventArgs args)
        {
            _adFailedToShowFlag = true;
        }

        /// <summary>
        /// 사용자가 닫기 아이콘을 탭하거나 뒤로 버튼을 사용하여 광고를 닫을 때 실행된다.
        /// 보상을 받기 전에 콜백된다. 앱에서 오디오 출력 또는 게임 루프를 일시중지했을 때 이 메소드로 재개하면 편리하다.
        /// </summary>
        protected virtual void HandleAdClosed(object sender, EventArgs args)
        {
            _adClosedFlag = true;
        }

        /// <summary>
        /// 보상형 광고를 완수했을 때 호출된다.
        /// </summary>
        protected virtual void HandleUserEarnedReward(object sender, Reward reward)
        {
            int rewardAmount = 0;
            string rewardUnit = reward.Type;
            
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                rewardAmount = (int)reward.Amount / 10; // 테스트 유닛 아이디에서는 10조각을 받는다. 따라서 10으로 나눠서 하나만 받도록 수정한다.
#else
                rewardAmount = (int)reward.Amount;
#endif
            if (rewardAmount <= 0 )
            {
                throw new Exception($"{_adName} - 보상 개수가 1미만일 수가 없음.");
            }
            _adRewardAmount = rewardAmount;
            _adRewardUnit = rewardUnit;
            _adRewardFlag = true;
        }

        /// <summary>
        /// 광고를 로드한다.
        /// <remark>로드가 완료되면 onAdLoaded가 콜백된다.</remark>
        /// </summary>
        public void LoadAd()
        {
            Log.Info($"{_adName} - 광고 로드.");
            if (IsAdLoaded())
            {
                Log.Info($"{_adName} - 광고가 이미 로드되어 있어서 새로 만들지 않음.");
                return;
            }

            // RewardedAd는 일회용 객체다.
            // 보상형 광고가 표시된 후에는 이 객체를 사용해 다른 광고를 로드할 수 없다.
            // 다른 보상형 광고를 요청하려면 RewardedAd 객체를 만들어야 한다.
            _ad = new RewardedAd(_adUnitId);
            _ad.OnAdLoaded += HandleAdLoaded;
            _ad.OnAdFailedToLoad += HandleAdFailedToLoad;
            _ad.OnAdOpening += HandleAdOpening;
            _ad.OnAdFailedToShow += HandleAdFailedToShow;
            _ad.OnUserEarnedReward += HandleUserEarnedReward;
            _ad.OnAdClosed += HandleAdClosed;

            AdRequest request = new AdRequest.Builder().Build();
            _ad.LoadAd(request);
        }

        /// <summary>
        /// 광고 로드여부를 리턴한다.
        /// </summary>
        /// <returns>광고 로드 여부</returns>
        public bool IsAdLoaded()
        {
            if (_ad != null)
            {
                return _ad.IsLoaded();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 로드된 광고를 출력한다.
        /// </summary>
        public void ShowAd()
        {
            Log.Info($"{_adName} - 광고 출력.");
            if (_ad.IsLoaded())
            {
                _ad.Show();
            }
            else
            {
                Log.Warning($"{_adName} - 광고가 로드되어 있지않아 표시할 수 없음.");
                HandleAdFailedToShow(null, null);
            }
        }

        /// <summary>
        /// 광고를 로드하고 완료되면 바로 출력한다.
        /// </summary>
        public void LoadAndShowAd()
        {
            StartCoroutine(nameof(CoLoadAndShowAd));
        }

        /// <summary>
        /// 광고를 로드하고 완료되면 바로 출력하는 코루틴.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CoLoadAndShowAd()
        {
            LoadAd();

            // 로딩이 완료될때 까지 대기.
            while (!_ad.IsLoaded())
            {
                yield return YieldCache.WaitForSeconds(0.2f);
            }

            ShowAd();
        }

        /// <summary>
        /// 광고가 로드될 때 호출됨.
        /// </summary>
        protected virtual void OnAdLoaded()
        {
            Log.Info($"{_adName} - 광고 로드됨.");
        }

        /// <summary>
        /// 광고가 출력되기 시작할 때 호출됨.
        /// </summary>
        protected virtual void OnAdOpening()
        {
            Log.Info($"{_adName} - 광고 시작.");
        }


        /// <summary>
        /// 광고가 닫혔을 때 호출됨. 광고중간에 닫아서 보상을 받지않더라도 호출된다.
        /// OnAdReward보다 먼저 호출된다.
        /// </summary>
        protected virtual void OnAdClosed()
        {
            Log.Info($"{_adName} - 광고 닫힘.");
        }

        /// <summary>
        /// 광고를 전부보고 보상받을 때 호출된다.
        /// </summary>
        /// <param name="rewardAmount"></param>
        protected virtual void OnAdReward(int amount, string unit)
        {
            Log.Info($"{_adName} - 광고 보상받음. ({amount} {unit})");
        }

        /// <summary>
        /// 광고 로드실패시 호출된다.
        /// </summary>
        protected virtual void OnAdFailedToLoad()
        {
            Log.Warning($"{_adName} - 광고 로드실패.");
        }

        /// <summary>
        /// 광고 출력실패시 호출된다.
        /// </summary>
        protected virtual void OnAdFailedToShow()
        {
            Log.Warning($"{_adName} - 광고 출력실패.");
        }
    }
}