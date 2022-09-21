using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Rano.Billing.UnityPurchase
{
    public sealed class UnityPurchaseManager : ManagerComponent, IPurchaseManager, IStoreListener
    {
        private enum EState
        {
            None,
            Initializing,
            Initialized,
        }
        
        private EState _state = EState.None;
        private IStoreController _controller;
        private IExtensionProvider _extensions;
        private IReceiptValidator _receiptValidator;
        [SerializeField] private InAppProductSO[] _rawProducts;
     
        public bool IsInitialized => _state >= EState.Initialized;
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
            
            if (_state == EState.Initializing)
            {
                Log.Warning($"이미 초기화가 진행중입니다");
                return;
            }
            if (_state == EState.Initialized)
            {
                Log.Warning($"이미 초기화가 되었습니다");
                return;
            }
            if (_rawProducts == null || _rawProducts.Length <= 0)
            {
                Log.Warning("원시상품들이 없으면 초기화할 수 없습니다.");
                return;
            }

            // 초기화 중 설정
            _state = EState.Initializing;
            
            // ConfigurationBuilder 인스턴스 생성
            ConfigurationBuilder builder;
            try
            {
                builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            }
            catch (Exception e)
            {
                Log.Warning("UnityEngine.Purchasing의 ConfigurationBuilder 인스턴스를 만드는 중 예외 발생");
                Log.Exception(e);
                return;
            }

            // 원시상품들의 정보를 초기화시에 사용한다.
            foreach (InAppProductSO rawProduct in _rawProducts)
            {
                UnityEngine.Debug.Assert(rawProduct != null);
                Log.Info($"원시상품 추가 ({rawProduct.Id})");
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
                    Log.Warning($"원시상품 추가 실패 ({rawProduct.Id}");
                    Log.Exception(e);
                    _state = EState.None;
                    this.onInitializeFailed?.Invoke();
                    return;
                }
            }

            // UnityPurchasing 초기화
            try
            {
                UnityPurchasing.Initialize(this, builder);
            }
            catch (Exception e)
            {
                Log.Warning($"UnityPurchasing 초기화 실패");
                Log.Exception(e);
                _state = EState.None;
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
            if (_state != EState.Initialized)
            {
                Log.Warning($"초기화가 되지 않아 구매할 수 없습니다");
                return;
            }
            try
            {
                _controller.InitiatePurchase(productId);    
            }
            catch (Exception e)
            {
                Log.Warning($"구매 중 예외 발생 ({productId})");
                Log.Exception(e);
                return;
            }
        }

        /// <summary>
        /// 구매를 복구합니다 (iOS 전용)
        /// </summary>
        public void RestoreAllPurchases()
        {
            Log.Info($"구매복구 요청");
            if (_state != EState.Initialized)
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
                // 구매 복구되지 않더라도 절차가 성공하면 실행됨.
                Log.Info("구매복구 성공");
                this.onRestoreAllPurchasesComplete?.Invoke();
            }
            else
            {
                Log.Warning("구매복구 실패");
                this.onRestoreAllPurchasesFailed?.Invoke();
            }
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            UnityEngine.Debug.Assert(controller != null && extensions != null);
            
            _controller = controller;
            _extensions = extensions;

            // UnityPurchase의 Product를 Rano에서 사용하는 InAppProduct로 변환한다.
            InAppProduct[] inAppProducts = _controller.products.all
                .Select(product => product.ConvertToInAppProduct())
                .ToArray();
            
            Log.Info($"초기화 완료");
            _state = EState.Initialized;
            this.onInitialized?.Invoke(inAppProducts);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Log.Warning($"초기화 실패 ({error})");
            _state = EState.None;
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
            Log.Todo("국제화 필요한지 여부 확인하고 문자열 국제화 시킬 것");
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
        
        /// <summary>
        /// </summary>
        /// <remarks>IStoreListener 인터페이스의 메소드 구현</remarks>
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
            if (IsInitialized == false) return null;
            Product product = _controller.products.WithID(productId);
            if (product != null) return product.ConvertToInAppProduct();
            return null;
        }
        
        public InAppProduct[] GetProducts()
        {
            if (IsInitialized == false) return null;
            var result = _controller.products.all
                .Select(product => product.ConvertToInAppProduct())
                .ToArray();
            return result;
        }
        
        [ContextMenu("Log Status")]
        public void LogStatus()
        {
            Log.Info($"{nameof(UnityPurchaseManager)} Status:");
            Log.Info($"  State: {_state}");
            Log.Info($"  IsInitialized: {IsInitialized}");
            Log.Info($"  rawProducts: {_rawProducts.Length}");
            if (IsInitialized)
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
            if (IsInitialized == false)
            {
                Log.Warning($"초기화되지 않아 상품을 출력할 수 없습니다 ({productId})");
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
            if (IsInitialized == false)
            {
                Log.Warning($"초기화되지 않아 상품들을 출력할 수 없습니다");
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