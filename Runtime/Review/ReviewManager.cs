#nullable enable

using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Rano.Review
{
    public abstract partial class ReviewManager : ManagerComponent
    {
        private int _openReviewCount;
        private DateTime _lastOpenReviewDateTime;
        // TODO: 이 값을 Initialize나 ConfigSO에서 설정할 수 있도록 할것, iOS 정책상 1년에 3회까지만 리뷰요청을 할 수 있으므로 122일 단위시간을 사용 (1년의 1/3)
        private TimeSpan _openReviewTimeSpan = new TimeSpan(days: 122, hours: 0, minutes: 0, seconds: 0);
        
        public int OpenReviewCount => _openReviewCount;
        public bool HasReviewOpened => (_openReviewCount > 0);
        public DateTime LastOpenReviewDateTime => _lastOpenReviewDateTime;
        
        /// <summary>
        /// 리뷰 정책을 리셋한다.
        /// </summary>
        public void ResetReviewPolicy()
        {
            Log.Info("리뷰 제한정책 체크를 위한 변수들을 리셋합니다");
            _openReviewCount = 0;
            _lastOpenReviewDateTime = DateTime.MinValue;
        }

        /// <summary>
        /// 정책 준수 하에 리뷰 가능 여부를 리턴한다.
        /// </summary>
        public bool CanReview()
        {
            if ((_lastOpenReviewDateTime + _openReviewTimeSpan) <= DateTime.UtcNow)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 리뷰를 요청한다.
        /// </summary>
        /// <remarks>요청이 가능한지 확인하고 진행한다. 성공하면 요청 횟수가 증가하고 시간이 기록된다.</remarks>
        public async Task<bool> RequestReviewAsync()
        {
            if (CanReview() == false)
            {
                Log.Info("리뷰 요청 실패 (기간 또는 횟수 제한으로 거부)");
                return false;
            }

            bool result = await RequestReviewInternalAsync();
            if (result)
            {
                _lastOpenReviewDateTime = DateTime.UtcNow;
                _openReviewCount++;
            }

            return result;
        }
        
        /// <summary>
        /// 초기화 여부를 리턴한다.
        /// </summary>
        public abstract bool IsInitialized { get; }
        
        /// <summary>
        /// 초기화 한다.
        /// </summary>
        public abstract bool Initialize();

        /// <summary>
        /// 실제 플랫폼 리뷰를 요청한다.
        /// </summary>
        public abstract Task<bool> RequestReviewInternalAsync();
        
        /// <summary>
        /// 특정 앱의 웹 페이지 주소를 얻는다.
        /// </summary>
        /// <remarks>URL을 리턴하므로 Escape처리를 확실해 해줘야 한다.</remarks>
        public abstract string GetWebPageUrl(string id);
        
        /// <summary>
        /// 특정 앱의 앱 스토어 주소를 얻는다.
        /// </summary>
        /// <remarks>URL을 리턴하므로 Escape처리를 확실해 해줘야 한다.</remarks>
        public abstract string GetAppStoreUrl(string id);

        public void OpenWebPage(string id)
        {
            Application.OpenURL(GetWebPageUrl(id));
        }

        public void OpenAppStore(string id)
        {
            Application.OpenURL(GetAppStoreUrl(id));
        }
    }
}