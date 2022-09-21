using System;

namespace Rano.Billing
{
    public interface IPurchaseManager
    {
        bool IsInitialized { get; }
        
        InAppProductSO[] RawProducts { get; set; }
        Action<InAppProduct[]> onInitialized { get; set; }
        Action onInitializeFailed { get; set; }
        Action<InAppProduct[]> onUpdateSuccess { get; set; }
        Action onUpdateFailed { get; set; }
        Action<string> onPurchaseComplete { get; set; }
        Action<string, string> onPurchaseFailed { get; set; }
        Action<InAppProduct> onValidatePurchaseSuccess { get; set; }
        Action<string> onValidatePurchaseFailed { get; set; }
        Action onRestoreAllPurchasesComplete { get; set; }
        Action onRestoreAllPurchasesFailed { get; set; }

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