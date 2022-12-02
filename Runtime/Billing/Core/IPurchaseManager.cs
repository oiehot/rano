using System;

namespace Rano.Billing
{
    public interface IPurchaseManager
    {
        InAppProductSO[] RawProducts { get; set; }
       
        bool IsInitialized { get; }
        
        public event Action<InAppProduct[]> OnInitialized;
        public event Action OnInitializeFailed;
        public event Action<InAppProduct[]> OnUpdateSuccess;
        public event Action OnUpdateFailed;
        public event Action<string> OnPurchaseCompleted;
        public event Action<string, string> OnPurchaseFailed;
        public event Action<InAppProduct> OnValidatePurchaseSuccess;
        public event Action<string> OnValidatePurchaseFailed;
        
#if UNITY_IOS
        public event Action OnRestoreAllPurchasesComplete;
        public event Action OnRestoreAllPurchasesFailed;
#endif

        void Initialize();
        void UpdateProducts();
        void Purchase(string productId);
        void RestoreAllPurchases();
        InAppProduct GetProduct(string productId);
        InAppProduct[] GetProducts();

        void LogStatus();
        void LogProduct(string productId);
        void LogProducts();
    }
}