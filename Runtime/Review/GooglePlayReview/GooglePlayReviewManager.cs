#nullable enable

using System;
using System.Collections;
using System.Threading.Tasks;
using Google.Play.Common;
using Google.Play.Review;
using UnityEngine;

namespace Rano.Review.GooglePlayReview
{
    /// <summary>
    /// 구글 플레이 리뷰 전반을 관리한다.
    /// </summary>
    /// <seealso href="https://developer.android.com/guide/playcore/in-app-review/unity">API</seealso>
    public sealed class GooglePlayReviewManager : Rano.Review.ReviewManager
    {
        private enum ERequestState
        {
            None = 0,
            Working,
            Failed,
            Success
        }

        private ERequestState _requestState;
        private Google.Play.Review.ReviewManager? _reviewManager;
        
        public override bool IsInitialized => _reviewManager != null;
        
        public override bool Initialize()
        {
            Log.Info("초기화 중...");
            _reviewManager = new Google.Play.Review.ReviewManager();
            Log.Info("초기화 완료");
            return true;
        }
        
        public override async Task<bool> RequestReviewInternalAsync()
        {
            Coroutine coroutine = StartCoroutine(nameof(RequestReviewCoroutine));
            while (_requestState == ERequestState.Working)
            {
                await Task.Yield();
            }

            bool result = _requestState switch
            {
                ERequestState.Success => true,
                ERequestState.Failed => false,
                _ => false
            };
            
            _requestState = ERequestState.None;
            return result;
        }

        private IEnumerator RequestReviewCoroutine()
        {
            Log.Info("리뷰 요청 중...");
            
            _requestState = ERequestState.Working;
            
            // 초기화 여부 체크.
            if (IsInitialized == false)
            {
                Log.Warning("리뷰 요청 실패 (초기화 되어 있지 않음)");
                _requestState = ERequestState.Failed;
                yield break;
            }
            
            // ReviewFlow를 요청하여 PlayReviewInfo 객체를 얻는다.
            PlayAsyncOperation<PlayReviewInfo, ReviewErrorCode>? requestFlowOperation;
            try
            {
                requestFlowOperation = _reviewManager!.RequestReviewFlow();
            }
            catch (Exception e)
            {
                Log.Warning("리뷰 요청 실패 (RequestReviewFlow 중 예외가 발생)");
                Log.Exception(e);
                _requestState = ERequestState.Failed;
                yield break;
            }

            // 전달 받은 RequestFlowOperation이 비어있는지 체크.
            if (requestFlowOperation == null)
            {
                Log.Warning("리뷰 요청 실패 (RequestFlowOperation가 비어 있음)");
                _requestState = ERequestState.Failed;
                yield break;
            }
            
            // RequestFlowOperation이 완료 될 때까지 대기.
            yield return requestFlowOperation;
            
            // RequestFlowOperation이 실패 한 경우에 요청 실패 처리.
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                Log.Warning("리뷰 요청 실패 (RequestFlowOperation이 실패)");
                _requestState = ERequestState.Failed;
                yield break;
            }

            // PlayReviewInfo 객체 얻기.
            PlayReviewInfo? playReviewInfo;
            try
            {
                playReviewInfo = requestFlowOperation.GetResult();
            }
            catch (Exception e)
            {
                Log.Warning("리뷰 요청 실패 (PlayReviewInfo를 얻는 중 예외 발생)");
                Log.Exception(e);
                _requestState = ERequestState.Failed;
                yield break;
            }
            
            // 전달 받은 PlayReviewInfo가 비어 있는 경우:
            if (playReviewInfo == null)
            {
                Log.Warning("리뷰 요청 실패 (PlayReviewInfo가 비어 있음)");
                _requestState = ERequestState.Failed;
                yield break;
            }
            
            // 인앱 리뷰 흐름을 시작한다.
            PlayAsyncOperation<VoidResult, ReviewErrorCode>? launchFlowOperation;
            try
            {
                launchFlowOperation = _reviewManager.LaunchReviewFlow(playReviewInfo);
            }
            catch (Exception e)
            {
                Log.Warning("리뷰 요청 실패 (LaunchReviewFlow 중 예외 발생");
                Log.Exception(e);
                _requestState = ERequestState.Failed;
                yield break;
            }

            // 전달 받은 LaunchFlowOperation이 비어 있는지 확인.
            if (launchFlowOperation == null)
            {
                Log.Warning("리뷰 요청 실패 (LaunchFlowOperation이 비어 있음");
                _requestState = ERequestState.Failed;
                yield break;
            }
            
            // LaunchFlowOperation 완료시 까지 대기
            yield return launchFlowOperation;

            // PlayReviewInfo 리셋
            playReviewInfo = null;

            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                Log.Warning("리뷰 요청 실패 (LaunchFlowOperation 실패)");
                _requestState = ERequestState.Failed;
                yield break;
            }
            
            Log.Info("리뷰 요청 완료");
            _requestState = ERequestState.Success;

            // 흐름이 완료되었습니다.
            // API는 사용자가 검토했는지 여부 또는 검토 대화 상자가 표시되었는지 여부를 나타내지 않습니다.
            // 따라서 결과에 관계없이 앱 흐름을 계속합니다.
        }
        
        public override string GetWebPageUrl(string id)
        {
            // https://play.google.com/store/apps/details?id=com.oiehot.afo2
            // https://play.google.com/store/apps/details?id=com.oiehot.afo2&hl=en_US&gl=US
            id = Rano.Network.URLHelper.EscapeURL(id);
            return $"https://play.google.com/store/apps/details?id={id}";
        }
        
        public override string GetAppStoreUrl(string id)
        {
            id = Rano.Network.URLHelper.EscapeURL(id);
            return $"market://details?id={id}";
        }
    }
}