using System;
using System.Collections.Generic;

namespace Rano.PlatformServices.Billing
{
    public interface IPurchaseHistory
    {
        Dictionary<string, InAppProduct> Products { get; }
        
        Action OnInitialized { get; set; }
        Action OnUpdated { get; set; }

        void SetValidate(string productId, bool value);
        bool IsPurchased(string productId);
        void ClearHistory();

        void LogStatus();
        void LogProduct(string productId);
        void LogProducts();
    }
}