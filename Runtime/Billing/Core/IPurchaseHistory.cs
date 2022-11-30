#nullable enable

using System;
using System.Collections.Generic;

namespace Rano.Billing
{
    public interface IPurchaseHistory
    {
        public Dictionary<string, InAppProduct> Products { get; }
        public event Action OnInitialized;
        public event Action OnUpdated;
        public void SetValidate(string productId, bool value);
        public bool IsPurchased(InAppProduct product);
        public bool IsPurchased(string productId);
        public void ClearHistory();
        public void LogStatus();
        public void LogProduct(string productId);
        public void LogProducts();
    }
}