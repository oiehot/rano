using System;
using VoxelBusters.EssentialKit;

namespace Rano.PlatformServices.Billing
{
    public class InAppProduct
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

        public bool IsPurchased => throw new NotImplementedException();

        public void LogStatus()
        {
            Log.Info($"* {ToString()}");
            
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
        }
        
        public override string ToString()
        {
            return $"{id}({nameof(InAppProduct)}";
        }
    }
}