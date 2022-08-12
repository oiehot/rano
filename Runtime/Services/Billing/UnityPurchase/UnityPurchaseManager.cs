using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Rano.Services.Billing.UnityPurchase
{
    public sealed class UnityPurchaseManager : ManagerComponent, IPurchaseManager, IStoreListener
    {
        private EPurchaseServiceState _state = EPurchaseServiceState.NotInitialized;
        private IStoreController _controller;
        private IExtensionProvider _extensions;
        private IReceiptValidator _receiptValidator;
        [SerializeField] private InAppProductSO[] _rawProducts;
     
        public EPurchaseServiceState State => _state;
        public bool IsAvailable => _state == EPurchaseServiceState.Available;        
        public InAppProductSO[] RawProducts
        {
            get => _rawProducts;
            set => _rawProducts = value;
        }
        
        public Action<InAppProduct[]> onInitialized { get; set; }
        public Action onInitializeFailed { get; set; }
        public Action<InAppProduct[]> onUpdateSuccess { get; set; }
        public Action onUpdateFailed { get; set; }
        public Action<string> onPurchaseComplete { get; set; }
        public Action<string, string> onPurchaseFailed { get; set; }
        public Action<InAppProduct> onValidatePurchaseSuccess { get; set; }
        public Action<string> onValidatePurchaseFailed { get; set; }
        public Action onRestoreAllPurchasesComplete { get; set; }
        public Action onRestoreAllPurchasesFailed { get; set; }
        
        protected override void Awake()
        {
            base.Awake();
            #if (!UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX))
                _receiptValidator = this.GetRequiredComponent<LocalReceiptValidator>();
            #else
                _receiptValidator = this.GetRequiredComponent<TestReceiptValidator>();
            #endif
        }
        
        public void Initialize()
        {
            Log.Info("구매서비스 초기화 시작.");
            
            if (_rawProducts == null || _rawProducts.Length <= 0)
            {
                Log.Warning("원시상품들이 없으면 초기화할 수 없습니다.");
                return;
            }

            if (_state != EPurchaseServiceState.NotInitialized)
            {
                Log.Warning($"이미 초기화가 되었습니다. (state:{_state})");
                return;
            }
            
            _state = EPurchaseServiceState.Initializing;
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            
            // 원시상품들의 정보를 초기화시에 사용한다.
            foreach (InAppProductSO rawProduct in _rawProducts)
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
                        UnityEngine.Debug.LogWarning(e);
                    #endif
                    _state = EPurchaseServiceState.InitializeFailed;
                    this.onInitializeFailed?.Invoke();
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
                    UnityEngine.Debug.LogWarning(e);
                #endif
                _state = EPurchaseServiceState.InitializeFailed;
                this.onInitializeFailed?.Invoke();
            }
        }
        
        public void UpdateProducts()
        {
            Log.Warning("이 메소드는 스토어에서 실시간으로 현재 상품정보들을 가져오는 것이지만");
            Log.Warning("초기화시 다운로드 받았던 정보에서 가져오는것을 확인했습니다.");
            Log.Warning("그러므로 이 메소드를 사용해선 안됩니다.");
            Log.Warning("상품정보를 업데이트하기 위해서는 앱을 재시작하셔야 합니다..");
            Log.Warning("업데이트 없이 메소드를 종료합니다.");
            #if false
            Product[] products = _controller.products.all;
            var additionalProducts = new HashSet<ProductDefinition>();
            foreach (Product product in products)
            {
                additionalProducts.Add(new ProductDefinition(product.definition.id, product.definition.type));
            }
            Log.Info($"상품정보 업데이트를 요청합니다,");
            _controller.FetchAdditionalProducts(additionalProducts, OnUpdateSuccess, OnUpdateFailed);
            #endif
        }

        public void Purchase(string productId)
        {
            Log.Info($"구매요청 ({productId})");
            if (_state != EPurchaseServiceState.Available)
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
            Log.Info($"구매복구 요청");
            if (_state != EPurchaseServiceState.Available)
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
                this.onRestoreAllPurchasesComplete?.Invoke();
            }
            else
            {
                Log.Warning("구매복구에 실패했습니다.");
                this.onRestoreAllPurchasesFailed?.Invoke();
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
            this.onInitialized?.Invoke(inAppProducts);
            _state = EPurchaseServiceState.Available;
            Log.Info($"초기화 완료됨.");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Log.Warning($"초기화 실패 ({error})");
            _state = EPurchaseServiceState.InitializeFailed;
            this.onInitializeFailed?.Invoke();
        }
        
        #if false
        public void OnUpdateSuccess()
        {
            Log.Info("상품정보 업데이트 성공.");
            InAppProduct[] inAppProducts = _controller.products.all
                .Select(product => product.ConvertToInAppProduct())
                .ToArray();
            this.onUpdateSuccess?.Invoke(inAppProducts);
        }

        public void OnUpdateFailed(InitializationFailureReason reason)
        {
            Log.Warning($"상품정보 업데이트 실패 ({reason})");
            this.onUpdateFailed?.Invoke();
        }
        #endif

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
            this.onPurchaseFailed?.Invoke(productId, reason);
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
                case EValidatePurchaseResultType.Success:
                    foreach (var receipt in result.ValidateReceipts)
                    {
                        OnValidatePurchaseSuccess(receipt.productID);
                    }
                    break;
                
                case EValidatePurchaseResultType.SuccessTest:
                    OnValidatePurchaseSuccess(purchasedProduct.definition.id);
                    break;
                
                case EValidatePurchaseResultType.Failed:
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
            this.onValidatePurchaseSuccess?.Invoke(product.ConvertToInAppProduct());
            this.onPurchaseComplete?.Invoke(productId);
        }

        private void OnValidatePurchaseFailed(Product purchasedProduct)
        {
            string productId = purchasedProduct.definition.id;
            this.onValidatePurchaseFailed?.Invoke(productId);
            Log.Warning($"영수증 검증에 실패했습니다 ({purchasedProduct.receipt})");   
        }
        
        public InAppProduct GetProduct(string productId)
        {
            if (_state != EPurchaseServiceState.Available) return null;
            Product product = _controller.products.WithID(productId);
            if (product != null) return product.ConvertToInAppProduct();
            return null;
        }
        
        public InAppProduct[] GetProducts()
        {
            if (_state != EPurchaseServiceState.Available) return null;
            var result = _controller.products.all
                .Select(product => product.ConvertToInAppProduct())
                .ToArray();
            return result;
        }
        
        [ContextMenu("Log Status")]
        public void LogStatus()
        {
            Log.Info($"{nameof(UnityPurchaseManager)} Status:");
            Log.Info($"  State: {State}");
            Log.Info($"  IsAvailable: {IsAvailable}");
            Log.Info($"  rawProducts: {_rawProducts.Length}");
            if (IsAvailable)
            {
                Log.Info($"  products:");
                Product[] products = _controller.products.all;
                foreach (Product product in products)
                {
                    int receiptLen = String.IsNullOrEmpty(product.receipt) ? 0 : product.receipt.Length;
                    Log.Info($"    - {product.definition.id} (type:{product.definition.type}, hasReceipt:{product.hasReceipt}, receiptLen:{receiptLen})");
                }
            }
        }

        public void LogProduct(string productId)
        {
            if (!IsAvailable)
            {
                Log.Warning($"초기화되지 않았습니다 (state:{State})");
                return;
            }
            
            Product product = _controller.products.WithID(productId);
            if (product == null)
            {
                Log.Warning($"상품을 찾을 수 없습니다 ({productId})");
                return;
            }
            product.LogStatus();
        }
        
        [ContextMenu("Log Products")]
        public void LogProducts()
        {
            if (!IsAvailable)
            {
                Log.Warning($"초기화되지 않았습니다 (state:{State})");
                return;
            }

            var products = _controller.products.all;
            if (products == null)
            {
                Log.Warning($"상품들을 찾을 수 없습니다.");
                return;
            }
            Log.Info($"{nameof(UnityPurchaseManager)} Products:");
            foreach (Product product in products)
            {
                product.LogStatus();
            }
        }

    }
}