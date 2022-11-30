using System;

namespace Rano.Billing
{
    public interface IPurchaseManager
    {
        bool IsInitialized { get; }
        
        InAppProductSO[] RawProducts { get; set; }
        public event Action<InAppProduct[]> onInitialized;
        public event Action onInitializeFailed;
        public event Action<InAppProduct[]> onUpdateSuccess;
        public event Action onUpdateFailed;
        public event Action<string> onPurchaseComplete;
        public event Action<string, string> onPurchaseFailed;
        public event Action<InAppProduct> onValidatePurchaseSuccess;
        public event Action<string> onValidatePurchaseFailed;
        public event Action onRestoreAllPurchasesComplete;
        public event Action onRestoreAllPurchasesFailed;

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