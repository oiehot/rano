using System;

namespace Rano.Billing
{
    [Serializable]
    public class InAppProduct : ICloneable
    {
        public bool enabled;

        public string id;
        public string storeSpecificId;
        public InAppProductType type;

        public string localizedTitle;
        public string localizedDescription;
        public string localizedPriceString;
        public decimal localizedPrice;
        public string isoCurrencyCode;

        public string receipt;
        public bool hasReceipt;
        public bool availableToPurchase;
        public string transactionId;

        public int purchaseCount;
        public bool validated;
        public DateTime lastValidateDateTime;

        public void LogStatus()
        {
            Log.Info($"* {id}({nameof(InAppProduct)})");
            Log.Info($"  id: {id}");
            Log.Info($"  enabled: {enabled}");
            Log.Info($"  type: {type}");
            Log.Info($"  storeSpecificId: {storeSpecificId}");
            Log.Info($"  localizedTitle: {localizedTitle}");
            Log.Info($"  localizedDescription: {localizedDescription}");
            Log.Info($"  localizedPriceString: {localizedPriceString}");
            Log.Info($"  localizedPrice: {localizedPrice}");
            Log.Info($"  isoCurrencyCode: {isoCurrencyCode}");
            Log.Info($"  receipt: {receipt}");
            Log.Info($"  hasReceipt: {hasReceipt}");
            Log.Info($"  availableToPurchase: {availableToPurchase}");
            Log.Info($"  transactionID: {transactionId}");
            Log.Info($"  purchaseCount: {purchaseCount}");
            Log.Info($"  validated: {validated}");
            Log.Info($"  lastValidateDateTime: {lastValidateDateTime}");
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override string ToString()
        {
            return $"{id}({nameof(InAppProduct)})";
        }
    }
}
