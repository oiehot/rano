#if false

#nullable enable

using System;
using System.Threading.Tasks;

namespace Rano.Review
{
    public interface IReviewManager
    {
        public bool IsInitialized { get; }
        public bool Initialize();
        public bool CanReview();
        public Task<bool> RequestReviewAsync();
    }
}

#endif