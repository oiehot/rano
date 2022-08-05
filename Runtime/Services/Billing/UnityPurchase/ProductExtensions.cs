using System;
using UnityEngine.Purchasing;

namespace Rano.Services.Billing.UnityPurchase
{
    public static class ProductExtensions
    {
        public static ProductType ConvertToProductType(this InAppProductType inAppProductType)
        {
            ProductType result = inAppProductType switch
            {
                InAppProductType.Consumable => ProductType.Consumable,
                InAppProductType.NonConsumable => ProductType.NonConsumable,
                InAppProductType.Subscription => ProductType.Subscription,
                _ => throw new Exception($"변환할 수 없는 상품타입 ({inAppProductType})")
            };
            return result;
        }
        
        public static InAppProductType ConvertToInAppProductType(this ProductType productType)
        {
            InAppProductType result = productType switch
            {
                ProductType.Consumable => InAppProductType.Consumable,
                ProductType.NonConsumable => InAppProductType.NonConsumable,
                ProductType.Subscription => InAppProductType.Subscription,
                _ => throw new Exception($"변환할 수 없는 상품타입 ({productType})")
            };
            return result;
        }
        
        public static InAppProduct ConvertToInAppProduct(this Product product)
        {
            var result = new InAppProduct();
            
            result.id = product.definition.id;
            result.enabled = product.definition.enabled;
            result.type = product.definition.type.ConvertToInAppProductType();
            result.storeSpecificId = product.definition.storeSpecificId;
            result.localizedTitle = product.metadata.localizedTitle;
            result.localizedDescription = product.metadata.localizedDescription;
            result.localizedPriceString = product.metadata.localizedPriceString;
            result.localizedPrice = product.metadata.localizedPrice;
            result.isoCurrencyCode = product.metadata.isoCurrencyCode;
            result.receipt = product.receipt;
            result.hasReceipt = product.hasReceipt;
            result.availableToPurchase = product.availableToPurchase;
            result.transactionId = product.transactionID;

            return result;
        }

        public static void LogStatus(this Product product)
        {
            Log.Info($"* {product.definition.id} ({nameof(Product)})");
            Log.Info($"  id: {product.definition.id}");
            Log.Info($"  enabled: {product.definition.enabled}");
            Log.Info($"  type: {product.definition.type}");
            Log.Info($"  storeSpecificId: {product.definition.storeSpecificId}");
            Log.Info($"  localizedTitle: {product.metadata.localizedTitle}");
            Log.Info($"  localizedDescription: {product.metadata.localizedDescription}");
            Log.Info($"  localizedPriceString: {product.metadata.localizedPriceString}");
            Log.Info($"  localizedPrice: {product.metadata.localizedPrice}");
            Log.Info($"  isoCurrencyCode: {product.metadata.isoCurrencyCode}");
            Log.Info($"  receipt: {product.receipt}");
            Log.Info($"  hasReceipt: {product.hasReceipt}");
            Log.Info($"  availableToPurchase: {product.availableToPurchase}");
            Log.Info($"  transactionID: {product.transactionID}");
        }
    }
}