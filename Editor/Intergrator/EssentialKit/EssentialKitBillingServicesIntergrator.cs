#if USE_ESSENTIAL_KIT_BILLING_SERVICES

using System;
using System.Collections.Generic;
using Rano.Services.Billing;
using VoxelBusters.CoreLibrary.NativePlugins;
using VoxelBusters.EssentialKit;

namespace Rano.Editor.Intergrator.EssentialKit
{
    public static class EssentialKitBillingServicesIntergrator
    {
        public static BillingServicesSettingsSO GetBillingServicesSettings()
        {
            BillingServicesSettingsSO settings = AssetDatabaseHelper.GetScriptableObject<BillingServicesSettingsSO>();
            return settings;
        }
        
        /// <summary>
        /// BillingSettingsSO, InAppProductSO 데이터를 BillingServicesSettings에 통합시킨다.
        /// </summary>
        public static void Intergrate()
        {
            Log.Info($"Intergrating EssentialKit BillingServices...", caller: false);
            
            // EssentialKitSettings 를 얻는다.
            EssentialKitSettings essentialKitSettings = EssentialKitIntergrator.GetEssentialKitSettings();
            if (essentialKitSettings == null)
            {
                Log.Info($"{typeof(EssentialKitSettings).FullName} 스크립터블 오브젝트가 없으므로 설정통합을 생략합니다.", caller:false);
                return;
            }
            
            // BillingServicesSettingsSO를 얻는다.
            BillingServicesSettingsSO billingServicesSettings = GetBillingServicesSettings();
            if (billingServicesSettings == null)
            {
                Log.Info($"{typeof(BillingServicesSettingsSO).FullName} 스크립터블 오브젝트가 없으므로 설정통합을 생략합니다.", caller:false);
                return;
            }

            // 새 설정을 만든다.
            var beforeSettings = essentialKitSettings.BillingServicesSettings;
            var newSettings = new BillingServicesUnitySettings(
                beforeSettings.IsEnabled,
                GetBillingProductDefinitions().ToArray(),
                billingServicesSettings.MaintainPurchaseHistory,
                billingServicesSettings.AutoFinishTransactions,
                billingServicesSettings.VerifyTransactionReceipts,
                beforeSettings.IosProperties,
                new BillingServicesUnitySettings.AndroidPlatformProperties(billingServicesSettings.AndroidPublicKey)
            );
            
            // 설정을 업데이트한다.
            essentialKitSettings.BillingServicesSettings = newSettings;
        }

        /// <summary>
        /// InAppProductSO 데이터를 통합한 BillingProductDefinition 리스트를 리턴한다. 
        /// </summary>
        private static List<BillingProductDefinition> GetBillingProductDefinitions()
        {
            var productList = new List<BillingProductDefinition>();
            var iaps = AssetDatabaseHelper.GetScriptableObjects<InAppProductSO>();
            foreach (var iap in iaps)
            {
                var platformIdOverrides = new NativePlatformConstantSet(
                    iap.IosId,
                    null,
                    iap.AndroidId
                );
                BillingProductType billingProductType;
                switch (iap.Type)
                {
                    case InAppProductType.Consumable:
                        billingProductType = BillingProductType.Consumable;
                        break;
                    case InAppProductType.NonConsumable:
                        billingProductType = BillingProductType.NonConsumable;
                        break;
                    default:
                        throw new Exception($"Incompatible in-app product type ({iap.Type})");
                }

                var product = new BillingProductDefinition(
                    iap.Id,
                    iap.Id,
                    platformIdOverrides,
                    billingProductType,
                    iap.Title,
                    iap.Description,
                    null,
                    null
                );
                productList.Add(product);
            }

            return productList;
        }
    }
}
#endif