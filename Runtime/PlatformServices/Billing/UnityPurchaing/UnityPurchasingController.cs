using System;
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
        private CrossPlatformValidator _localValidator;
        
#if (!UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX))
        private readonly byte[] _appleTangleData;
        private readonly byte[] _appleStoreKitTestTangleData;
        private readonly byte[] _googlePlayTangleData;
#endif
        
        private PurchaseServiceState _state = PurchaseServiceState.NotInitialized;

        #endregion
        
        #region Properties
    
        public PurchaseServiceState State => _state;
        
        #endregion
        
        #region Constructors

#if (UNITY_EDITOR)
        public UnityPurchaseService(PurchaseManager purchaseManager)
        {
            _purchaseManager = purchaseManager;
        }
#elif (!UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX))
        public UnityPurchaseService(PurchaseManager purchaseManager, byte[] appleTangleData, byte[] appleStoreKitTestTangleData, byte[] googlePlayTangleData)
        {
            _purchaseManager = purchaseManager;
            _appleTangleData = appleTangleData;
            _appleStoreKitTestTangleData = appleStoreKitTestTangleData;
            _googlePlayTangleData = googlePlayTangleData;
            
            #if !DEBUG_STOREKIT_TEST
                _localValidator = new CrossPlatformValidator(
                    _googlePlayTangleData,
                    _appleTangleData,
                    Application.identifier);
            #else
                // When Apple Xcode12 StoreKit Test
                _localValidator = new CrossPlatformValidator(
                    _googlePlayTangleData,
                    _appleStoreKitTestTangleData,
                    Application.identifier);
            #endif
        }
#endif
        
        #endregion
        
        #region Action Methods
        
        public void Initialize()
        {
            if (_purchaseManager.RawProducts.Length <= 0)
            {
                Log.Warning("원시상품들이 없으면 초기화할 수 없습니다.");
                return;
            }

            if (_state != PurchaseServiceState.NotInitialized)
            {
                Log.Warning($"이미 초기화가 되었습니다.");
                return;
            }
            
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
            if (_state != PurchaseServiceState.Available)
            {
                Log.Warning($"구매가능한 상태가 아닙니다. (currentState:{_state})");
                return;
            }
            _controller.InitiatePurchase(productId);
        }

        public void UpdateStatus()
        {
            if (_state != PurchaseServiceState.Available)
            {
                Log.Warning($"업데이트 할 수 있는 상태가 아닙니다. (state:{_state})");
                return;
            }
            _state = PurchaseServiceState.Updating;
            var products = _controller.products.all;
            var projectDefinitions = new HashSet<ProductDefinition>();
            foreach (var product in products)
            {
                var d = new ProductDefinition(product.definition.id, product.definition.type);
                projectDefinitions.Add(d);
            }
            _controller.FetchAdditionalProducts(projectDefinitions, OnUpdateSuccess, OnUpdateFailed);
        }

        /// <summary>
        /// 구매를 복구합니다 (iOS 전용)
        /// </summary>
        /// <remarks>안드로이드에서는 초기화시 복구할 상품들에 대해 ProcessPurchase가 자동으로 실행됩니다.</remarks>
        public void RestoreAllPurchases()
        {
            if (_state != PurchaseServiceState.Available)
            {
                Log.Warning($"구매복구 할 수 있는 상태가 아닙니다. (state:{_state})");
                return;
            }
#if (!UNITY_IOS)
            _extensions.GetExtension<IAppleExtensions>().RestoreTransactions(OnRestoreTransactions);
#else
            Log.Warning("iOS에서만 구매복구를 할 수 있습니다");
#endif
        }
        
        /// <summary>
        /// 컨트롤러에 저장된 상품목록을 PurchaseManager로 싱크한다.
        /// </summary>
        private void SyncProductToPurchaseManager()
        {
            Dictionary<string, InAppProduct> inAppProducts = new();
            foreach (Product product in _controller.products.all)
            {
                InAppProduct inAppProduct = product.ConvertToInAppProduct();
                inAppProducts[inAppProduct.id] = inAppProduct;
            }
            _purchaseManager.SetProductsAsync(inAppProducts);
        }
        
        #endregion

        #region IStoreListener Event Methods

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            UnityEngine.Debug.Assert(controller != null && extensions != null);
            _controller = controller;
            _extensions = extensions;
            _state = PurchaseServiceState.Available;
            SyncProductToPurchaseManager();
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
                    Log.Warning($"구매 실패 (productId:{product.definition.id}, reason:{failureReason})");
                    break;
            }
        }
        
        /// <summary>
        /// 서비스에서 구매가 완료되었을 때 콜백된다.
        /// </summary>
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
#if (UNITY_EDITOR)
            // 에디터에서는 영수증 검증을 지원하지 않으므로 바로 구매완료 처리한다.
            var product = purchaseEvent.purchasedProduct;
            var productId = product.definition.id;
            OnPurchaseComplete(productId);
            return PurchaseProcessingResult.Complete;
#elif (!UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX))
            // 영수증을 검증한다.
            _state = PurchaseServiceState.Validating;
            ValidatePurchaseAsync(purchaseEvent.purchasedProduct);
            return PurchaseProcessingResult.Pending;
#else
            throw new Exception("현재 실행중인 플랫폼에서는 구매를 진행할 수 없습니다");
#endif
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
        
        #region ValidatePurchase Event Methods

        private async void ValidatePurchaseAsync(Product purchasedProduct)
        {
#if (UNITY_EDITOR)
            // Pass
#elif (!UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX))
            IPurchaseReceipt[] validateReceipts;
            string rawReceipt = purchasedProduct.receipt;
            if (LocalValidateReceipt(rawReceipt, out validateReceipts))
            {
                Log.Info($"검증된 영수증입니다 ({validateReceipts.Length})");
                foreach (IPurchaseReceipt receipt in validateReceipts)
                {
                    OnValidateSuccess(receipt.productID);
                }
            }
            else
            {
                OnValidateFailed(rawReceipt);
            }
#else
            throw new Exception("현재 실행중인 플랫폼에서는 영수증 검증을 할 수 없습니다.");
#endif
            _state = PurchaseServiceState.Available;
        }

        public bool LocalValidateReceipt(string rawReceipt, out IPurchaseReceipt[] validateReceipts)
        {
#if (UNITY_EDITOR)
            Log.Warning("에디터 플랫폼에서는 정상적인 영수증 검증을 할 수 없습니다. 통과된 것으로 처리합니다.");
            validateReceipts = null;
            return true;
#elif (!UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX))
            try
            {
                // 구글플레이에서는 제품하나의 ID가 들어가 있지만 애플스토어에서는 여러 제품을 포함한다.
                validateReceipts = _localValidator.Validate(rawReceipt);
                return true;
            }
            catch (IAPSecurityException)
            {
                validateReceipts = null;
                return false;
            }
#else
            throw new Exception("현재 실행중인 플랫폼에서는 영수증 검증을 할 수 없습니다.");
#endif
        }
        
        public bool LocalValidateReceipt(string rawReceipt)
        {
#if (UNITY_EDITOR)
            Log.Warning("에디터 플랫폼에서는 정상적인 영수증 검증을 할 수 없습니다. 통과된 것으로 처리합니다.");
            return true;
#elif (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX)
            try
            {
                _localValidator.Validate(rawReceipt);
            }
            catch (IAPSecurityException)
            {
                return false;
            }
            return true;
#else
            throw new Exception("현재 실행중인 플랫폼에서는 영수증 검증을 할 수 없습니다.");
#endif
        }

        private void OnValidateSuccess(string productId)
        {
            Log.Info($"구매영수증이 검증되었습니다 ({productId})");
            Product product = _controller.products.WithID(productId);
            _controller.ConfirmPendingPurchase(product);
            OnPurchaseComplete(productId);
        }

        private void OnValidateFailed(string rawReceipt)
        {
            Log.Warning($"구매영수증 검증에 실패했습니다 ({rawReceipt})");
        }
        
        private void OnPurchaseComplete(string productId)
        {
            Log.Info($"구매 완료되었습니다 ({productId})");
            
            // 구매가 완료된 상품의 정보를 로컬에 업데이트한다.
            var product = _controller.products.WithID(productId);
            InAppProduct inAppProduct = product.ConvertToInAppProduct();
            
            // 이미 영수증이 검증되어 호출된 함수이므로, 구매상태를 바로 결정한다.
            switch (inAppProduct.type)
            {
                case InAppProductType.NonConsumable:
                    inAppProduct.purchaseState = PurchaseState.Purchased;
                    break;
                case InAppProductType.Consumable:
                case InAppProductType.Subscription:
                default:
                    inAppProduct.purchaseState = PurchaseState.Unknown;
                    break;
            }
            _purchaseManager.SetProduct(inAppProduct);
            
            // 클라이언트 구매완료 이벤트 라이즈
            _purchaseManager.OnPurchaseComplete?.Invoke(productId);
        }
        #endregion
        
        #region UpdateStatus Event Methods
        
        private void OnUpdateSuccess()
        {
            Log.Info("상태 업데이트에 성공했습니다.");
            SyncProductToPurchaseManager();
            _state = PurchaseServiceState.Available;
        }

        private void OnUpdateFailed(InitializationFailureReason reason)
        {
            Log.Warning($"상태 업데이트에 실패했습니다 ({reason})");
            _state = PurchaseServiceState.UpdateFailed;
        }

        #endregion
    }
}