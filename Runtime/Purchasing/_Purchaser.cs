// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

#if false

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using Rano;

namespace AFO2
{
    public class InAppProduct
    {
        public string id; // ex) com.oiehot.afo2.iap.bomb1
        public string appleAppStoreId;
        public string googlePlayId;
        public ProductType type;
        public UnityEvent onPurchaseComplete;
        public UnityEvent onPurchaseFailed;

        public InAppProduct(string id, string type, UnityEvent onPurchaseComplete, UnityEvent onPurchaseFailed)
        {
            this.id = id;
            if (type == "Consumable")
            {
                this.type = ProductType.Consumable;
            }
            else if (type == "NonConsumable")
            {
                this.type = ProductType.NonConsumable;
            }
            else if (type == "Subscription")
            {
                this.type = ProductType.Subscription;
            }
            else
            {
                throw new Exception($"{type} is invalid ProductType");
            }

            this.onPurchaseComplete = onPurchaseComplete;
            this.onPurchaseFailed = onPurchaseFailed;
        }
    }

    public partial class Purchaser : MonoBehaviour, IStoreListener
    {
        private IStoreController storeController;
        private IExtensionProvider storeExtensionProvider;
        private List<InAppProduct> products;

        void Awake()
        {
            this.products = List<InAppProduct>();
        }

        void Start()
        {
            // 초기화되지 않았다면
            if (this.storeController == null)
            {
                // 초기화
                InitializePurchasing();
            }
        }

        public void AddProduct(InAppProduct product)
        {
            this.products.Add(product);
        }
        
        private bool IsInitialized()
        {
            return this.storeController != null && this.storeExtensionProvider != null;
        }        
        
        public void InitializePurchasing()
        {
            if (this.IsInitialized())
            {
                return;
            }

            Log.Important("InitializePurchasing Start");
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            Log.Info("Add Product IDs");
            foreach (InAppProduct product in this.products)
            {
                builder.AddProduct(product.id, product.type);
            }

            // NonConsumable Example:
            // builder.AddProduct("com.oiehot.afo2.iap.noad", ProductType.NonConsumable);

            // Subscription Example:
            // builder.AddProduct("com.oiehot.afo2.iap.subscription",
            //     ProductType.Subscription,
            //     new IDs(){
            //         { "com.oiehot.afo2.iap.subscription.apple", AppleAppStore.Name },
            //         { "com.oiehot.afo2.iap.subscription.google", GooglePlay.Name }
            //     }
            // );

            Log.Info("UnityPurchasing Initialize Start");
            UnityPurchasing.Initialize(this, builder);
        }

        public void BuyProduct(string id)
        {
            if (this.IsInitialized())
            {
                Product product = this.storeController.products.WithID(id);
                if (product != null && product.availableToPurchase)
                {
                    Log.Important($"BuyProduct in async: {product.definition.id}");
                    this.storeController.InitiatePurchase(product); // 이후 ProcessPurchase 나 OnPurchaseFailed 가 실행됨.
                }
                else
                {
                    Log.Warning($"Not purchasing product: {id}");
                }
            }
            else
            {
                Log.Warning($"BuyProduct Failed: {id}");
            }
        }

        public void RestorePurchases()
        {
            if (!this.IsInitialized())
            {
                Log.Warning("RestorePurchases Failed. Not Initialized");
                return;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                Log.Important("RestorePurchases Started");
                var apple = this.storeExtensionProvider.GetExtension<IAppleExtensions>();
                apple.RestoreTransactions((result) => {
                    Log.Info($"RestorePurchases Continuing: {result} / If no further messages, no purchases available to restore");
                });
            }
            else
            {
                Debug.Log($"RestorePurchases Failed. Not supported on this platform: {Application.platform}");
            }
        }

        #region IStoreListener

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Log.Info("OnInitialized");
            this.storeController = controller;
            this.storeExtensionProvider = extensions;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Log.Warning($"OnInitializeFailed: {error}");
        }

        /// <summary>
        /// 제품구매가 성공하면 호출된다. 해당 제품 구입시 보상처리
        /// StringComparision.Ordinal: 서수정렬비교
        /// </summary>
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            string id = args.purchasedProduct.definition.id;
            
            /// TODO: 여기에서 해당제품 구매시 로직을 추가하면 된다.
            if (String.Equals(id, "com.oiehot.afo2.iap.bomb1", StringComparison.Ordinal))
            {
                Log.Info($"ProcessPurchase: product:{id}");
                // ex) GameManager.Instance.data.bombCount.Value += 100;
            }
            else
            {
                Debug.Log($"ProcessPurchase: Unrecognized product {id}");
            }

            // Return a flag indicating whether this product has completely been received, or if the application needs 
            // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
            // saving purchased products to the cloud, and when that save is delayed. 
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Log.Warning($"OnPurchaesFailed: product:{product.definition.storeSpecificId}, reason:{failureReason}");
        }

        #endregion
    }
}

#endif