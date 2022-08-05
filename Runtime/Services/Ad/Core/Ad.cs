using System;
using UnityEngine;

namespace Rano.Services.Ad
{
    /// <summary>
    /// 이벤트 핸들러는 Admob 스레드에서 실행되므로 핸들러 안에서는 유니티 엔진 라이브러리를 사용해서는 안된다.
    /// 우리는 이 문제를 핸들러에서는 플래그를 세우고 메인스레드에서 작동되는 Update에서 사용자 이벤트 함수를 호출하는 식으로 해결했다.
    /// 다음 글을 참고할것: https://ads-developers.googleblog.com/2016/04/handling-android-ad-events-in-unity.html
    /// </summary>
    // TODO: 에드몹스레드에서 실행되는 콜백에서 UniTask 등을 이용하여 유니티스레드에서 자동으로 메서드를 실행하도록 수정. 업데이트에서 반복적으로 플래그를 체크할 필요는 없음.
    [RequireComponent(typeof(CanvasSorter))]
    public abstract class Ad : MonoBehaviour
    {
        private readonly object _lockObject = new object();
        private CanvasSorter _canvasSorter;
        
        private AdState _adState;
        private int _adLoadCount;
        private bool _adLoadedFlag;
        private bool _adOpeningFlag;
        private bool _adClosedFlag;
        private bool _adFailedToLoadFlag;
        private bool _adFailedToShowFlag;

        [Header("Ad Settings")]
        [SerializeField] private AdSO _adInfo;

        [Header("Settings")]
        [SerializeField] private bool _autoLoadOnAwake = true;
        [SerializeField] private bool _autoReload = true;

        protected object LockObject => _lockObject;
        public Action OnAdLoading { get; set; }
        public Action OnAdLoaded { get; set; }
        public Action OnAdOpening { get; set; }
        public Action OnAdClosed { get; set; }
        public Action OnAdFailedToLoad { get; set; }
        public Action OnAdFailedToShow { get; set; }
        public string AdName => _adInfo.adName;
        public string AdUnitId => _adInfo.UnitId;
        public AdState State => _adState;
        public bool IsAvailable => _adState == AdState.Available;
        public bool IsLoading => _adState == AdState.Loading;

        protected virtual void Awake()
        {
            _canvasSorter = this.GetRequiredComponent<CanvasSorter>();
        }

        protected virtual void OnEnable()
        {
            if (_autoLoadOnAwake == true)
            {
                Log.Info($"{AdName} - AutoLoadOnAwake 플래그가 켜져있어 자동으로 광고를 로드합니다.");
                LoadAd();
            }
        }

        protected virtual void OnDisable()
        {
            UnloadAd();
        }

        /// <<summary>
        /// Admob스레드에서 호출된 콜백에서 켜진 플래그를 체크하여 매칭되는 함수를 메인스레드에서 실행한다.
        /// </summary>
        protected virtual void Update()
        {
            if (_adLoadedFlag)
            {
                _adLoadedFlag = false;
                _adState = AdState.Available;
                Log.Info($"{AdName} - 광고 로드 완료");
                OnAdLoaded?.Invoke();
            }

            if (_adOpeningFlag)
            {
                _adOpeningFlag = false;
                _adState = AdState.Opening;
                // 전면보상광고 캔버스의 기본 SortingOrder 값은 0이다.
                // 게임에서 사용하는 캔버스의 기본 SortingOrder 값도 0이므로
                // 두 캔버스의 렌더링 우선순위가 애매해져서 광고 위에 게임UI가 그려질 수 있다.
                // 이런 문제점을 해결하기 위해서 미래 등록해둔 게임UI 캔버스들의 SortingOrder를
                // 일괄적으로 아래로 내려줘서 전면광고가 가장 위에 그려질 수 있도록 수정한다.
                _canvasSorter.MoveSortingOrder(-10);
                Log.Info($"{AdName} - 광고 시작");
                OnAdOpening?.Invoke();
            }

            if (_adClosedFlag)
            {
                _adClosedFlag = false;
                _adState = AdState.Closed;
                Log.Info($"{AdName} - 광고 닫힘");
                // 수정했던 모든 캔버스SortingOrder를 원래 위치로 돌려놓는다.
                _canvasSorter.ResetSortingOrder();
                OnAdClosed?.Invoke();
                UnloadAd();
                if (_autoReload == true)
                {
                    Log.Info($"{AdName} - AutoReload 플래그가 켜져있어 광고가 닫힘과 동시에 새 광고를 로드합니다.");
                    LoadAd();
                }
            }

            if (_adFailedToLoadFlag)
            {
                _adFailedToLoadFlag = false;
                _adState = AdState.NotLoaded;
                Log.Warning($"{AdName} - 광고 로드 실패");
                OnAdFailedToLoad?.Invoke();
            }

            if (_adFailedToShowFlag)
            {
                _adFailedToShowFlag = false;
                Log.Warning($"{AdName} - 광고 출력 실패");
                OnAdFailedToShow?.Invoke();
            }
        }
        
        /// <summary>광고 로드가 완료될 때 실행된다.</summary>
        /// <remarks>에드몹 스레드에서 실행된다.</remarks>
        protected void HandleAdLoaded(object sender, EventArgs args)
        {
            lock (_lockObject)
            {
                _adLoadCount++;
                _adLoadedFlag = true;
            }
        }

        /// <summary>광고 로드에 실패할 때 실행된다.</summary>
        /// /// <remarks>에드몹 스레드에서 실행된다.</remarks>
        protected void HandleAdFailedToLoad(object sender, object args) // args:AdFailedToLoadEventArgs in admob
        {
            _adFailedToLoadFlag = true;
        }

        /// <summary>
        /// 광고가 표시될 때 기기 화면을 덮는다.
        /// 이때 필요한 경우 오디오 출력 또는 게임 루프를 일시중지하는 것이 좋다.
        /// </summary>
        /// <remarks>에드몹 스레드에서 실행된다.</remarks>
        protected void HandleAdOpening(object sender, EventArgs args)
        {
            _adOpeningFlag = true;
        }

        /// <summary>광고 표시에 실패할 때 실행된다.</summary>
        /// <remarks>에드몹 스레드에서 실행된다.</remarks>
        protected void HandleAdFailedToShow(object sender, object args) // args:AdErrorEventArgs in admob
        {
            _adFailedToShowFlag = true;
        }

        /// <summary>
        /// 사용자가 닫기 아이콘을 탭하거나 뒤로 버튼을 사용하여 광고를 닫을 때 실행된다.
        /// 보상을 받기 전에 콜백된다. 앱에서 오디오 출력 또는 게임 루프를 일시중지했을 때 이 메소드로 재개하면 편리하다.
        /// </summary>
        /// <remarks>에드몹 스레드에서 실행된다.</remarks>
        protected void HandleAdClosed(object sender, EventArgs args)
        {
            _adClosedFlag = true;
        }
        
        public void LoadAd()
        {
            switch (State)
            {
                case AdState.Loading:
                    Log.Info($"{AdName} - 광고가 로딩중이므로 새로 로드하지 않습니다.");
                    return;
                case AdState.Available:
                    Log.Info($"{AdName} - 광고가 이미 로드되어 있어서 새로 로드하지 않습니다.");
                    return;
            }

#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            Log.Info($"{AdName} - 광고 로드 시작 ({AdUnitId})");
#else
            Log.Info($"{AdName} -  광고 로드 시작");
#endif
            _adState = AdState.Loading;
            OnAdLoading?.Invoke();
            LoadAdClient();
        }

        /// <summary>
        /// 로드된 광고를 출력한다.
        /// </summary>
        public void ShowAd()
        {
            Log.Info($"{AdName} - 광고 출력.");
            if (IsAvailable)
            {
                ShowClientAd();
            }
            else
            {
                Log.Warning($"{AdName} - 광고가 로드되어 있지않아 표시할 수 없음.");
                HandleAdFailedToShow(null, null);
            }
        }

        private void UnloadAd()
        {
            Log.Info($"{AdName} - 광고 언로딩");
            UnloadAdClient();
            _adState = AdState.NotLoaded;
        }
        
        protected abstract void LoadAdClient();
        protected abstract void UnloadAdClient();
        protected abstract void ShowClientAd();
    }
}