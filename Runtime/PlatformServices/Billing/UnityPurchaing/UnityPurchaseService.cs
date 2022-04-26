using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using Debug = UnityEngine.Debug;

namespace Rano.PlatformServices.Billing
{
    public sealed class UnityPurchaseService : MonoBehaviour, IStoreListener
    {
        private PurchaseServiceState _state = PurchaseServiceState.NotInitialized;
        private PurchaseManager _purchaseManager;
        private IStoreController _controller;
        private IExtensionProvider _extensions;
        private IReceiptValidator _receiptValidator;
        
        public PurchaseServiceState State => _state;
        public bool IsAvailable => _state == PurchaseServiceState.Available; 
        
        private void Awake()
        {
            _purchaseManager = this.GetRequiredComponent<PurchaseManager>();
            #if (!UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX))
                _receiptValidator = this.GetRequiredComponent<LocalReceiptValidator>();
            #else
                _receiptValidator = this.GetRequiredComponent<TestReceiptValidator>();
            #endif
        }
        
        public void Initialize()
        {
            if (_purchaseManager.RawProducts.Length <= 0)
            {
                Log.Warning("원시상품들이 없으면 초기화할 수 없습니다.");
                return;
            }

            if (_state != PurchaseServiceState.NotInitialized)
            {
                Log.Warning($"이미 초기화가 되었습니다. (state:{_state})");
                return;
            }
            
            _state = PurchaseServiceState.Initializing;
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            
            // 원시상품들의 정보를 초기화시에 사용한다.
            foreach (InAppProductSO rawProduct in _purchaseManager.RawProducts)
            {
                Log.Info($"Add RawProduct ({rawProduct.Id})");
                try
                {
                    builder.AddProduct(
                        rawProduct.Id,
                        rawProduct.Type.ConvertToProductType(),
                        new IDs
                        {
                            {String.IsNullOrEmpty(rawProduct.AndroidId) ? rawProduct.Id : rawProduct.AndroidId, GooglePlay.Name},
                            {String.IsNullOrEmpty(rawProduct.IosId) ? rawProduct.Id : rawProduct.IosId, AppleAppStore.Name}
                        }
                    );
                }
                catch (Exception e)
                {
                    Log.Warning($"Add RawProduct Failed ({rawProduct.Id}");
                    #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
                        Debug.LogWarning(e);
                    #endif
                    _state = PurchaseServiceState.InitializeFailed;
                    return;
                }
            }

            try
            {
                UnityPurchasing.Initialize(this, builder);
            }
            catch (Exception e)
            {
                Log.Warning($"UnityPurchasing.Initialize Failed");
                #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
                    Debug.LogWarning(e);
                #endif
                _state = PurchaseServiceState.InitializeFailed;
            }
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

        /// <summary>
        /// 구매를 복구합니다 (iOS 전용)
        /// </summary>
        public void RestoreAllPurchases()
        {
            if (_state != PurchaseServiceState.Available)
            {
                Log.Warning($"구매복구 할 수 있는 상태가 아닙니다. (state:{_state})");
                return;
            }
            #if (UNITY_IOS)
                _extensions.GetExtension<IAppleExtensions>().RestoreTransactions(OnRestoreTransactions);
            #else
                // 안드로이드에서는 초기화시 복구할 상품들에 대해 ProcessPurchase가 자동으로 실행됩니다.
            #endif
        }
        
        private void OnRestoreTransactions(bool result)
        {
            if (result)
            {
                // This does not mean anything was restored, merely that the restoration process succeeded.
                Log.Info("구매복구에 성공했습니다.");
                _purchaseManager.OnRestoreAllPurchasesComplete?.Invoke();
            }
            else
            {
                Log.Warning("구매복구에 실패했습니다.");
                _purchaseManager.OnRestoreAllPurchasesFailed?.Invoke();
            }
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            UnityEngine.Debug.Assert(controller != null && extensions != null);
            _controller = controller;
            _extensions = extensions;

            InAppProduct[] inAppProducts = _controller.products.all
                .Select(product => product.ConvertToInAppProduct())
                .ToArray();
            _purchaseManager.OnInitialized?.Invoke(inAppProducts);
            
            _state = PurchaseServiceState.Available;
            Log.Info($"구매서비스 초기화 완료됨.");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Log.Warning($"구매서비스 초기화 실패 ({error})");
            _state = PurchaseServiceState.InitializeFailed;
            _purchaseManager.OnInitializeFailed?.Invoke();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // TODO: 국제화
            string reason = failureReason switch
            {
                PurchaseFailureReason.Unknown => "원인불명",
                PurchaseFailureReason.DuplicateTransaction => "트랜잭션 중복",
                PurchaseFailureReason.PaymentDeclined => "지불 거부",
                PurchaseFailureReason.ProductUnavailable => "상품을 사용할 수 없음",
                PurchaseFailureReason.PurchasingUnavailable => "구매가 불가능함",
                PurchaseFailureReason.SignatureInvalid => "잘못된 시그니쳐",
                PurchaseFailureReason.UserCancelled => "사용자 취소",
                PurchaseFailureReason.ExistingPurchasePending => "이미 구매되어 확인중입니다",
                _ => ""
            };
            string productId = product.definition.id;
            Log.Warning($"구매 실패 (productId:{productId}, reason:{reason})");
            _purchaseManager.OnPurchaseFailed?.Invoke(productId, reason);
        }
        
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            ValidatePurchaseAsync(purchaseEvent.purchasedProduct);
            return PurchaseProcessingResult.Pending;
        }

        private async void ValidatePurchaseAsync(Product purchasedProduct)
        {
            var result = await _receiptValidator.ValidateAsync(purchasedProduct.receipt);
            switch (result.Type)
            {
                case ValidatePurchaseResultType.Success:
                    foreach (var receipt in result.ValidateReceipts)
                    {
                        OnValidatePurchaseSuccess(receipt.productID);
                    }
                    break;
                
                case ValidatePurchaseResultType.SuccessTest:
                    OnValidatePurchaseSuccess(purchasedProduct.definition.id);
                    break;
                
                case ValidatePurchaseResultType.Failed:
                default:
                    OnValidatePurchaseFailed(purchasedProduct);
                    break;
            }
        }

        private void OnValidatePurchaseSuccess(string productId)
        {
            Log.Info($"영수증이 검증되어 구매완료 처리합니다 ({productId})");
            Product product = _controller.products.WithID(productId);
            _controller.ConfirmPendingPurchase(product);
            _purchaseManager.OnValidatePurchaseSuccess?.Invoke(product.ConvertToInAppProduct());
            _purchaseManager.OnPurchaseComplete?.Invoke(productId);
        }

        private void OnValidatePurchaseFailed(Product purchasedProduct)
        {
            string productId = purchasedProduct.definition.id;
            _purchaseManager.OnValidatePurchaseFailed?.Invoke(productId);
            Log.Warning($"영수증 검증에 실패했습니다 ({purchasedProduct.receipt})");
            
        }

        public InAppProduct GetProduct(string productId)
        {
            if (_state != PurchaseServiceState.Available) return null;
            Product product = _controller.products.WithID(productId);
            if (product != null) return product.ConvertToInAppProduct();
            return null;
        }
        
        public InAppProduct[] GetProducts()
        {
            if (_state != PurchaseServiceState.Available) return null;
            var result = _controller.products.all
                .Select(product => product.ConvertToInAppProduct())
                .ToArray();
            return result;
        }
        
    }
}