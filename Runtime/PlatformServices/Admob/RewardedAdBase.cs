using System;
using GoogleMobileAds.Api;
using UnityEngine;

namespace Rano.PlatformServices.Admob
{
    public enum AdLoadStatus
    {
        None,
        Loading,
        Loaded
    }

    /// <summary>
    /// 이벤트 핸들러는 Admob 스레드에서 실행되므로 핸들러 안에서는 유니티 엔진 라이브러리를 사용해서는 안된다.
    /// 우리는 이 문제를 핸들러에서는 플래그를 세우고 메인스레드에서 작동되는 Update에서 사용자 이벤트 함수를 호출하는 식으로 해결했다.
    /// 다음 글을 참고할것: https://ads-developers.googleblog.com/2016/04/handling-android-ad-events-in-unity.html
    /// </summary>
    [RequireComponent(typeof(CanvasSorter))]
    public sealed class RewardedAdBase : MonoBehaviour
    {
        private readonly object _lockObject = new object();

        private RewardedAd _ad;
        private CanvasSorter _canvasSorter;
        
        private bool _adLoadedFlag = false;
        private bool _adOpeningFlag = false;
        private bool _adRewardFlag = false;
        private bool _adClosedFlag = false;
        private bool _adFailedToLoadFlag = false;
        private bool _adFailedToShowFlag = false;
        private int _adRewardAmount;
        private string _adRewardUnit;
        private int _adLoadCount = 0;
        
        [Header("Ad Settings")]
        [SerializeField] private AdSO _adInfo;

        [Header("Settings")]
        [SerializeField] private bool _autoLoadOnAwake = true;
        [SerializeField] private bool _autoReload = true;

        public Action onAdLoading;
        public Action onAdLoaded;
        public Action onAdOpening;
        public Action onAdClosed;
        public Action<int, string> onAdReward;
        public Action onAdFailedToLoad;
        public Action onAdFailedToShow;

        public string AdName => _adInfo.adName;
        public string AdUnitId => _adInfo.UnitId;
        
        public AdLoadStatus LoadStatus
        {
            get
            {
                if (_ad == null) return AdLoadStatus.None;
                if (IsLoading) return AdLoadStatus.Loading;
                if (IsLoaded) return AdLoadStatus.Loaded;
                throw new Exception("광고로드 상태를 알 수 없음");
            }
        }
        public bool IsLoading => (_ad != null && !_ad.IsLoaded());
        public bool IsLoaded => (_ad != null && _ad.IsLoaded());

        void Awake()
        {
            _canvasSorter = this.GetRequiredComponent<CanvasSorter>();
            if (_autoLoadOnAwake == true)
            {
                Log.Info($"{AdName} - AutoLoadOnAwake 플래그가 켜져있어 자동으로 광고를 로드합니다.");
                LoadAd();
            }
        }

        /// <<summary>
        /// Admob스레드에서 호출된 콜백에서 켜진 플래그를 체크하여 매칭되는 함수를 메인스레드에서 실행한다.
        /// </summary>
        // TODO: 에드몹스레드에서 실행되는 콜백에서 UniTask 등을 이용하여 유니티스레드에서 자동으로 메서드를 실행하도록 수정.
        // TODO: 업데이트에서 반복적으로 플래그를 체크할 필요는 없음.
        void Update()
        {
            if (_adLoadedFlag)
            {
                _adLoadedFlag = false;
                Log.Info($"{AdName} - 광고 로드 완료");
                onAdLoaded?.Invoke();
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
                Log.Info($"{AdName} - 광고 시작");
                onAdOpening?.Invoke();
            }

            if (_adClosedFlag)
            {
                _adClosedFlag = false;
                Log.Info($"{AdName} - 광고 닫힘");
                // 수정했던 모든 캔버스SortingOrder를 원래 위치로 돌려놓는다.
                _canvasSorter.ResetSortingOrder();
                onAdClosed?.Invoke();
                _ad = null;
                if (_autoReload == true)
                {
                    Log.Info($"{AdName} - AutoReload 플래그가 켜져있어 광고가 닫힘과 동시에 새 광고를 로드합니다.");
                    LoadAd();
                }
            }

            if (_adRewardFlag)
            {
                _adRewardFlag = false;
                Log.Info($"{AdName} - 광고 보상받음 ({_adRewardAmount} {_adRewardUnit})");
                onAdReward?.Invoke(_adRewardAmount, _adRewardUnit);
                _adRewardAmount = 0;
                _adRewardUnit = null;
            }

            if (_adFailedToLoadFlag)
            {
                _adFailedToLoadFlag = false;
                Log.Warning($"{AdName} - 광고 로드 실패");
                onAdFailedToLoad?.Invoke();
            }

            if (_adFailedToShowFlag)
            {
                _adFailedToShowFlag = false;
                Log.Warning($"{AdName} - 광고 출력 실패");
                onAdFailedToShow?.Invoke();
            }
        }

        /// <summary>광고 로드가 완료될 때 실행된다.</summary>
        /// <remarks>에드몹 스레드에서 실행된다.</remarks>
        private void HandleAdLoaded(object sender, EventArgs args)
        {
            lock (_lockObject)
            {
                _adLoadCount++;
                _adLoadedFlag = true;
            }
        }

        /// <summary>광고 로드에 실패할 때 실행된다.</summary>
        /// /// <remarks>에드몹 스레드에서 실행된다.</remarks>
        private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            _adFailedToLoadFlag = true;
        }

        /// <summary>
        /// 광고가 표시될 때 기기 화면을 덮는다.
        /// 이때 필요한 경우 오디오 출력 또는 게임 루프를 일시중지하는 것이 좋다.
        /// </summary>
        /// <remarks>에드몹 스레드에서 실행된다.</remarks>
        private void HandleAdOpening(object sender, EventArgs args)
        {
            _adOpeningFlag = true;
        }

        /// <summary>광고 표시에 실패할 때 실행된다.</summary>
        /// <remarks>에드몹 스레드에서 실행된다.</remarks>
        private void HandleAdFailedToShow(object sender, AdErrorEventArgs args)
        {
            _adFailedToShowFlag = true;
        }

        /// <summary>
        /// 사용자가 닫기 아이콘을 탭하거나 뒤로 버튼을 사용하여 광고를 닫을 때 실행된다.
        /// 보상을 받기 전에 콜백된다. 앱에서 오디오 출력 또는 게임 루프를 일시중지했을 때 이 메소드로 재개하면 편리하다.
        /// </summary>
        /// <remarks>에드몹 스레드에서 실행된다.</remarks>
        private void HandleAdClosed(object sender, EventArgs args)
        {
            _adClosedFlag = true;
        }

        /// <summary>
        /// 보상형 광고를 완수했을 때 호출된다.
        /// </summary>
        /// <remarks>에드몹 스레드에서 실행된다.</remarks>
        private void HandleUserEarnedReward(object sender, Reward reward)
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
                throw new Exception($"{AdName} - 보상 개수가 1미만일 수가 없음.");
            }

            lock (_lockObject)
            {
                _adRewardAmount = rewardAmount;
                _adRewardUnit = rewardUnit;
                _adRewardFlag = true;
            }
        }

        /// <summary>
        /// 광고를 로드한다.
        /// <remark>로드가 완료되면 onAdLoaded가 콜백된다.</remark>
        /// </summary>
        public void LoadAd()
        {
            if (LoadStatus == AdLoadStatus.Loading)
            {

                Log.Info($"{AdName} - 광고가 로딩중이므로 새로 로드하지 않습니다.");
                return;
            }

            if (_ad != null && _ad.IsLoaded() == true)
            {
                Log.Info($"{AdName} - 광고가 이미 로드되어 있어서 새로 로드하지 않습니다.");
                return;
            }

            
#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            Log.Info($"{AdName} - 광고 로드 시작 ({AdUnitId})");
#else
            Log.Info($"{AdName} -  광고 로드 시작");
#endif

            // RewardedAd는 일회용 객체다.
            // 보상형 광고가 표시된 후에는 이 객체를 사용해 다른 광고를 로드할 수 없다.
            // 다른 보상형 광고를 요청하려면 RewardedAd 객체를 만들어야 한다.
            _ad = new RewardedAd(AdUnitId);
            _ad.OnAdLoaded += HandleAdLoaded;
            _ad.OnAdFailedToLoad += HandleAdFailedToLoad;
            _ad.OnAdOpening += HandleAdOpening;
            _ad.OnAdFailedToShow += HandleAdFailedToShow;
            _ad.OnUserEarnedReward += HandleUserEarnedReward;
            _ad.OnAdClosed += HandleAdClosed;

            onAdLoading?.Invoke();

            AdRequest request = new AdRequest.Builder().Build();
            _ad.LoadAd(request);
        }

        /// <summary>
        /// 로드된 광고를 출력한다.
        /// </summary>
        public void ShowAd()
        {
            Log.Info($"{AdName} - 광고 출력.");
            if (_ad.IsLoaded())
            {
                _ad.Show();
            }
            else
            {
                Log.Warning($"{AdName} - 광고가 로드되어 있지않아 표시할 수 없음.");
                HandleAdFailedToShow(null, null);
            }
        }
    }
}