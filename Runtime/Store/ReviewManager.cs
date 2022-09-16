#if false

// TODO: EssentialKit 플러그인을 사용하지 않게되어 새로 작성해야함.

using System;
using UnityEngine;
using VoxelBusters.EssentialKit;
using Rano.SaveSystem;

namespace Rano.Store
{
    [Serializable]
    public struct SReviewManagerData
    {
        public int askReviewCount;
        public int openReviewCount;
        public DateTime lastAskReviewDateTime;
        public DateTime lastOpenReviewDateTime;
    }
    
    /// <summary>
    /// 정책준수하에 리뷰요청 다이얼로그을 띄우거나 리뷰 페이지를 열어준다.
    /// </summary>
    public class ReviewManager : ManagerComponent, IRateMyAppController, ISaveLoadable
    {
        private bool _askReviewRequested = false;
        private int _askReviewCount = 0;
        private int _openReviewCount = 0;
        private DateTime _lastAskReviewDateTime;
        private DateTime _lastOpenReviewDateTime;
        
        // iOS 정책상 1년에 3회까지만 리뷰요청을 할 수 있으므로 122일 단위시간을 사용 (1년의 1/3)
        private TimeSpan _askReviewTimeSpan = new TimeSpan(days: 122, hours: 0, minutes: 0, seconds: 0);
        
        // TODO: 유니티 국제화 패키지를 사용하여 텍스트의 국제화가 필요함.
        [SerializeField] private string _title = "Rate My App";
        [SerializeField] private string _description = "If you enjoy test";
        [SerializeField] private string _okButtonLabel = "OK";
        [SerializeField] private string _cancelButtonLabel = "Cancel";
        [SerializeField] private string _remindLaterButtonLabel = "Remind Me Later";
        [SerializeField] private bool _useRemindMeLater = true;

        public Action OnClickOk { get; set; }
        public Action OnClickCancel { get; set; }
        public Action OnClickRemindLater { get; set; }        
        public Action OnClosePopup { get; set; }

        public bool HasReviewOpened => (_openReviewCount > 0);
        public bool HasReviewAsked => (_askReviewCount > 0);
        
        /// <summary>
        /// 정책준수하에 AskReview 다이얼로그를 출력할 수 있는지 여부.
        /// </summary>
        public bool CanAskReview {
            get
            {
                if ((_lastAskReviewDateTime + _askReviewTimeSpan) <= DateTime.UtcNow)
                {
                    return true;
                }
                return false;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            
            Log.Todo("Apply internationalized text to the review request dialog");

            // 기존 설정을 가져온다.
            RateMyAppSettings beforeSettings =
                VoxelBusters.EssentialKit.EssentialKitSettings.Instance.ApplicationSettings.RateMyAppSettings;

            // 새 다이얼로그 설정을 만든다.
            RateMyAppConfirmationDialogSettings dialogSettings = new RateMyAppConfirmationDialogSettings(
                canShow: true,
                title: _title,
                description: _description,
                okButtonLabel: _okButtonLabel,
                cancelButtonLabel: _cancelButtonLabel,
                remindLaterButtonLabel: _remindLaterButtonLabel,
                canShowRemindMeLaterButton: _useRemindMeLater
            );

            // 새 설정을 만든다.
            RateMyAppSettings newSettings = new VoxelBusters.EssentialKit.RateMyAppSettings(
                isEnabled: true,
                dialogSettings: dialogSettings,
                defaultValidatorSettings: beforeSettings.DefaultValidatorSettings
            );

            // 설정을 업데이트한다.
            VoxelBusters.EssentialKit.EssentialKitSettings.Instance.ApplicationSettings.RateMyAppSettings = newSettings;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            OnClickOk += HandleOnClickOk;
            OnClickCancel += HandleOnCancel;
            OnClickRemindLater += HandleOnRemindLater;
            OnClosePopup += HandleOnClose;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            OnClickOk -= HandleOnClickOk;
            OnClickCancel -= HandleOnCancel;
            OnClickRemindLater -= HandleOnRemindLater;
            OnClosePopup -= HandleOnClose;
        }
        
        private void HandleOnClickOk()
        {
            Log.Info("Review request accepted");
            _openReviewCount++;
            _lastOpenReviewDateTime = DateTime.UtcNow;    
            // VoxelBusters.EssentialKit.RateMyApp 에 의해서 자동적으로 열린다.
        }
        
        private void HandleOnCancel()
        {
            Log.Info("Review request denied");
        }
        
        private void HandleOnRemindLater()
        {
            Log.Info("A review request will be notified later");
        }
        
        private void HandleOnClose()
        {
            Log.Info("The review request dialogue is closed");
            _askReviewRequested = false;
        }

        /// <summary>
        /// 리뷰를 할지 여부를 사용자에게 묻는다.
        /// CanAskReview를 통해 정책을 준수하는지 여부를 먼저 확인하고 실행하길 바람.
        /// </summary>
        [ContextMenu("Ask Review")]
        public void AskReview()
        {
            Log.Info("Opens the review request dialog");
            _askReviewCount++;
            _lastAskReviewDateTime = DateTime.UtcNow;
            _askReviewRequested = true;
        }

        /// <summary>
        /// 리뷰창을 바로 띄운다.
        /// </summary>
        [ContextMenu("Show Review")]
        public void ShowReview()
        {
            _openReviewCount++;
            _lastOpenReviewDateTime = DateTime.UtcNow;
            VoxelBusters.EssentialKit.Utilities.RequestStoreReview();
        }
        
        #region Implementation of ISaveLoadable

        public void ClearState()
        {
            _askReviewCount = 0;
            _openReviewCount = 0;
            _lastAskReviewDateTime = DateTime.MinValue;
            _lastOpenReviewDateTime = DateTime.MinValue;
        }
        
        public void DefaultState()
        {
            _askReviewCount = 0;
            _openReviewCount = 0;
            _lastAskReviewDateTime = DateTime.MinValue;
            _lastOpenReviewDateTime = DateTime.MinValue;
        }
        
        public object CaptureState()
        {
            SReviewManagerData state = new SReviewManagerData
            {
                askReviewCount = _askReviewCount,
                openReviewCount = _openReviewCount,
                lastAskReviewDateTime = _lastAskReviewDateTime,
                lastOpenReviewDateTime = _lastOpenReviewDateTime
            };
            return state;
        }
        
        public void ValidateState(object state)
        {
            SReviewManagerData data = (SReviewManagerData) state;
            if (data.askReviewCount < 0)
            {
                throw new StateValidateException("askReviewCount가 0이하일 수는 없음");
            }            
            if (data.openReviewCount < 0)
            {
                throw new StateValidateException("openReviewCount가 0이하일 수는 없음");
            }
        }
        
        public void RestoreState(object state)
        {
            var data = (SReviewManagerData) state;
            _askReviewCount = data.askReviewCount;
            _openReviewCount = data.openReviewCount;
            _lastAskReviewDateTime = data.lastAskReviewDateTime;
            _lastOpenReviewDateTime = data.lastOpenReviewDateTime;
        }
        
        #endregion
        
        #region Implementation of IRateMyAppController
        
        /// <summary>
        /// VoxelBusters.EssentialKit.RateMyApp 컴포넌트에 의해서 Ok버튼 선택시 호출된다.
        /// </summary>
        public void DidClickOnOkButton()
        {
            OnClickOk?.Invoke();
            OnClosePopup?.Invoke();
        }

        /// <summary>
        /// VoxelBusters.EssentialKit.RateMyApp 컴포넌트에 의해서 Cancel버튼 선택시 호출된다.
        /// </summary>
        public void DidClickOnCancelButton()
        {
            OnClickCancel?.Invoke();
            OnClosePopup?.Invoke();
        }
        
        /// <summary>
        /// VoxelBusters.EssentialKit.RateMyApp 컴포넌트에 의해서 RemindLater버튼 선택시 호출된다.
        /// </summary>
        public void DidClickOnRemindLaterButton()
        {
            OnClickRemindLater?.Invoke();
            OnClosePopup?.Invoke();
        }

        /// <summary>
        /// 현재 리뷰를 할 수 있는 상황인지를 리턴한다. true를 리턴하면
        /// VoxelBusters.EssentialKit.RateMyApp 싱글톤이 확인하고 광고를 띄운다.
        /// 이 메서드는 VoxelBusters.EssentialKit.RateMyApp 싱글톤의 Update에서 매번 호출된다.
        /// </summary>
        /// <returns>평가 창을 지금 띄울것인지 여부</returns>
        public bool CanShowRateMyApp()
        {
            // VoxelBusters.EssentialKit.RateMyApp.AskForReviewNow();
            return _askReviewRequested;
        }
        
        #endregion
    }
}

#endif