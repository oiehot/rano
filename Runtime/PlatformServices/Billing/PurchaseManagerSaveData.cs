using System;
using System.Collections.Generic;

namespace Rano.PlatformServices.Billing
{
    [Serializable]
    public struct PurchaseManagerSaveData
    {
        public Dictionary<string, InAppProduct> products;
        public DateTime lastUpdateDateTime;
    }
}