using System;
using UnityEngine;
using VoxelBusters.EssentialKit;

namespace Rano.PlatformServices.Social
{
    /// <summary>
    /// 리뷰요청 창을 연다.
    /// </summary>
    public class ReviewController : MonoBehaviour, IRateMyAppController
    {
        private bool _askReviewRequested;
        
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

        void Awake()
        {
            Log.Info("Apply internationalized text to the review request dialog");

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

        void OnEnable()
        {
            OnClickOk += HandleOnClickOk;
            OnClickCancel += HandleOnCancel;
            OnClickRemindLater += HandleOnRemindLater;
            OnClosePopup += HandleOnClose;
        }

        void OnDisable()
        {
            OnClickOk -= HandleOnClickOk;
            OnClickCancel -= HandleOnCancel;
            OnClickRemindLater -= HandleOnRemindLater;
            OnClosePopup -= HandleOnClose;
        }
        
        private void HandleOnClickOk()
        {
            Log.Info("Review request accepted");
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
        /// </summary>
        [ContextMenu("Ask Review")]
        public void AskReview()
        {
            _askReviewRequested = true;
        }

        /// <summary>
        /// 리뷰창을 바로 띄운다.
        /// </summary>
        [ContextMenu("Show Review")]
        public void ShowReview()
        {
            VoxelBusters.EssentialKit.Utilities.RequestStoreReview();
        }
        
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