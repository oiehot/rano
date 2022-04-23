using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace Rano.PlatformServices.Billing
{
    public sealed class UnityPurchaseService : IStoreListener
    {
        #region Private Fields

        private readonly PurchaseManager _purchaseManager;
        private IStoreController _controller;
        private IExtensionProvider _extensions;
        private PurchaseServiceState _state = PurchaseServiceState.NotInitialized;
        
        #endregion
        
        #region Properties
        
        public PurchaseServiceState State => _state;
        
        #endregion
        
        #region Constructors

        public UnityPurchaseService(PurchaseManager purchaseManager)
        {
            _purchaseManager = purchaseManager;
        }
        
        #endregion
        
        #region Action Methods
        
        public void Initialize()
        {
            if (_purchaseManager.RawProducts.Length <= 0) throw new Exception("원시상품들이 없으면 초기화할 수 없습니다.");
            if (_state != PurchaseServiceState.NotInitialized) throw new Exception($"이미 초기화가 되었습니다.");
            _state = PurchaseServiceState.Initializing;
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            
            // 원시상품들의 정보를 초기화시에 사용한다.
            foreach (InAppProductSO rawProduct in _purchaseManager.RawProducts)
            {
                builder.AddProduct(
                    rawProduct.Id,
                    rawProduct.Type.ConvertToProductType(),
                    new IDs {
                        {rawProduct.AndroidId, GooglePlay.Name},
                        {rawProduct.IosId, AppleAppStore.Name}
                    }
                );
            }
            UnityPurchasing.Initialize(this, builder);
        }

        public void Purchase(string productId)
        {
            if (_state != PurchaseServiceState.Available) throw new Exception($"구매가능한 상태가 아닙니다. (currentState:{_state})");
            _controller.InitiatePurchase(productId);
        }

        public void UpdateStatus()
        {
            if (_state != PurchaseServiceState.Available) throw new Exception($"업데이트 할 수 있는 상태가 아닙니다. (state:{_state})");
            var products = _controller.products.all;
            var projectDefinitions = new HashSet<ProductDefinition>();
            foreach (var product in products)
            {
                var d = new ProductDefinition(product.definition.id, product.definition.type);
                projectDefinitions.Add(d);
            }
            _state = PurchaseServiceState.Updating;
            _controller.FetchAdditionalProducts(projectDefinitions, OnUpdateStatusSuccess, OnUpdateStatusFailed);
        }

        /// <summary>
        /// 구매를 복구합니다 (iOS 전용)
        /// </summary>
        /// <remarks>안드로이드에서는 초기화시 복구할 상품들에 대해 ProcessPurchase가 자동으로 실행됩니다.</remarks>
        public void RestoreAllPurchases()
        {
            if (_state != PurchaseServiceState.Available) throw new Exception($"구매복구 할 수 있는 상태가 아닙니다. (state:{_state})");
            #if (!UNITY_IOS)
            _extensions.GetExtension<IAppleExtensions>().RestoreTransactions(OnRestoreTransactions);
            #else
            Log.Warning("iOS에서만 구매복구를 할 수 있습니다");
            #endif
        }
        
        private void SendProductsToManager()
        {
            Log.Info("StoreController.Products => Manager.LatestProducts");
            
            // TODO: before = _manager.LatestProducts;
            
            _purchaseManager.LatestProducts.Clear();
            foreach (var product in _controller.products.all)
            {
                InAppProduct inAppProduct = product.ConvertToInAppProduct();
                
                // 구매여부를 확인한다.
                if (inAppProduct.type == InAppProductType.Consumable)
                    inAppProduct.SetPurchase(false);
                
                // TODO: Check Validation

                _purchaseManager.LatestProducts[inAppProduct.id] = inAppProduct;
            }
            
            // TODO: latest = _manager.LatestProducts;
            // TODO: Compare(before, latest) => Callback OnRefunded, OnRemoved
        }
        
        #endregion

        #region Event Methods

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            UnityEngine.Debug.Assert(controller != null && extensions != null);
            _controller = controller;
            _extensions = extensions;
            _state = PurchaseServiceState.Available;
            // TODO: Check IsPurchased each products!
            SendProductsToManager();
            Log.Info($"구매서비스 초기화됨.");
        }
        
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Log.Warning($"구매서비스 초기화 실패 ({error})");
            _state = PurchaseServiceState.InitializeFailed;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            switch (failureReason)
            {
                default:
                    Log.Warning($"구매 실패 ({product}, {failureReason})");
                    break;
            }
        }
        
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            Log.Info($"구매 결과:");
            purchaseEvent.purchasedProduct.LogStatus();
            
            // 검사
            bool validPurchase = true; // Presume valid for platforms with no R.V.

            // Unity IAP's validation logic is only included on these platforms.
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
            // Prepare the validator with the secrets we prepared in the Editor
            // obfuscation window.
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
                AppleTangle.Data(), Application.bundleIdentifier);

            try {
                // On Google Play, result has a single product ID.
                // On Apple stores, receipts contain multiple products.
                var result = validator.Validate(e.purchasedProduct.receipt);
                // For informational purposes, we list the receipt(s)
                Debug.Log("Receipt is valid. Contents:");
                foreach (IPurchaseReceipt productReceipt in result) {
                    Debug.Log(productReceipt.productID);
                    Debug.Log(productReceipt.purchaseDate);
                    Debug.Log(productReceipt.transactionID);
                }
            } catch (IAPSecurityException) {
                Debug.Log("Invalid receipt, not unlocking content");
                validPurchase = false;
            }
#endif

            if (validPurchase) {
                // 구매완료된 상품의 정보를 PurchaseManager.LatestProduct로 업데이트 한다.
                Product purchasedProduct = purchaseEvent.purchasedProduct;
                string productId = purchasedProduct.definition.id;
                InAppProduct inAppProduct = purchasedProduct.ConvertToInAppProduct();
                if (inAppProduct.type != InAppProductType.Consumable)
                    inAppProduct.SetPurchase(true);
                _purchaseManager.LatestProducts[productId] = inAppProduct;
            
                // 즉시 구매처리 완료.
                _purchaseManager.OnPurchaseComplete?.Invoke(productId);
                return PurchaseProcessingResult.Complete;
            }
            else
            {
                throw new Exception("검증되지 않은 구매");
            }
        }
        
        private void OnUpdateStatusSuccess()
        {
            Log.Info("상태 업데이트에 성공했습니다.");
            foreach (var product in _controller.products.all) {
                Log.Info($"* {product.definition.id}");
            }
            SendProductsToManager();
            _state = PurchaseServiceState.Available;
        }

        private void OnUpdateStatusFailed(InitializationFailureReason reason)
        {
            Log.Warning($"상태 업데이트에 실패했습니다 ({reason})");
            _state = PurchaseServiceState.UpdateFailed;
        }

        private void OnRestoreTransactions(bool result)
        {
            if (result)
            {
                // This does not mean anything was restored,
                // merely that the restoration process succeeded.
                Log.Info("구매복구에 성공했습니다.");
                _purchaseManager.OnRestoreAllPurchasesComplete?.Invoke();
            }
            else
            {
                Log.Warning("구매복구에 실패했습니다.");
                _purchaseManager.OnRestoreAllPurchasesFailed?.Invoke();
            }
        }

        #endregion
                
    }
}