#nullable enable

using System;
using Rano.SaveSystem;

namespace Rano.Review
{
    [Serializable]
    public class ReviewManagerData
    {
        public int openReviewCount;
        public DateTime lastOpenReviewDateTime;
    }

    public abstract partial class ReviewManager : ISaveLoadable
    {
        public void ClearState()
        {
            _openReviewCount = 0;
            _lastOpenReviewDateTime = DateTime.MinValue;
        }
        
        public void DefaultState()
        {
            _openReviewCount = 0;
            _lastOpenReviewDateTime = DateTime.MinValue;
        }
        
        public object CaptureState()
        {
            ReviewManagerData state = new ReviewManagerData
            {
                openReviewCount = _openReviewCount,
                lastOpenReviewDateTime = _lastOpenReviewDateTime
            };
            return state;
        }
        
        public void ValidateState(object state)
        {
            ReviewManagerData data = (ReviewManagerData) state; 
            if (data.openReviewCount < 0)
            {
                throw new StateValidateException("openReviewCount가 0이하일 수는 없음");
            }
        }
        
        public void RestoreState(object state)
        {
            var data = (ReviewManagerData) state;
            _openReviewCount = data.openReviewCount;
            _lastOpenReviewDateTime = data.lastOpenReviewDateTime;
        }
    }
}