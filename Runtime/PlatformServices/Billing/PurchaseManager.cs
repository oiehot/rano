using System;
using System.Collections.Generic;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;

namespace Rano.PlatformServices.Billing
{
    public enum PurchaseFailedReason
    {
        Unknown,
        AlreadyPurchased
    }
    
    public enum PurchaseManagerState
    {
        NotInitialized,
        Initialized
    }
    
    public sealed class PurchaseManager : MonoSingleton<PurchaseManager>
    {
        private PurchaseManagerState _state = PurchaseManagerState.NotInitialized;
        public PurchaseManagerState State => _state;

        public bool IsInitialized => (_state == PurchaseManagerState.Initialized);
        
        public bool IsFeatureAvailable
        {
            get
            {
                try
                {
                    return BillingServices.IsAvailable();
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// PlatformId에 매칭되는 Product 객체를 담는 사전.
        /// PlatformId가 아닌 Settings에 적혀진 ProductId를 키로 사용한다.
        /// </summary>
        private Dictionary<string, IBillingProduct> _products = new Dictionary<string, IBillingProduct>();
        
        public Dictionary<string, IBillingProduct> Products => _products;

        public Action<string> OnPurchaseComplete { get; set; }
        public Action<string, string> OnPurchaseFailed { get; set; }
        public Action<string> OnRestorePurchase { get; set; }
        public Action<int> OnRestoreAllPurchasesComplete { get; set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            BillingServices.OnInitializeStoreComplete += HandleInitializeStoreComplete;
            BillingServices.OnTransactionStateChange += HandleTransactionStateChange;
            BillingServices.OnRestorePurchasesComplete += HandleRestoreAllPurchasesComplete;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            BillingServices.OnInitializeStoreComplete -= HandleInitializeStoreComplete;
            BillingServices.OnTransactionStateChange -= HandleTransactionStateChange;
            BillingServices.OnRestorePurchasesComplete -= HandleRestoreAllPurchasesComplete;
        }
        
        public void Initialize()
        {
            _products.Clear();
            if (IsFeatureAvailable)
            {
                try
                {
                    Log.Info("결제서비스 초기화 시작");
                    BillingServices.InitializeStore();
                }
                catch
                {
                    Log.Warning($"결제서비스 초기화에 실패했습니다.");
                    _state = PurchaseManagerState.NotInitialized;
                }
            }
            else
            {
                Log.Warning("결제서비스를 사용할 수 없어 초기화할 수 없습니다.");
                _state = PurchaseManagerState.NotInitialized;
            }
        }
        
        private void HandleInitializeStoreComplete(BillingServicesInitializeStoreResult result, Error error)
        {
            if (error == null)
            {
                IBillingProduct[] products = result.Products;
                Log.Info("결제서비스 초기화 완료.");
                Log.Info($"총 {products.Length} 개의 상품이 있습니다.");
                for (int i = 0; i < products.Length; i++)
                {
                    IBillingProduct product = products[i];
                    _products.Add(product.Id, product);
                }
            }
            else
            {
                Log.Warning($"결제서비스 초기화 실패 ({error})");
                _state = PurchaseManagerState.NotInitialized;
                return;
            }

            string[] invalidIds = result.InvalidProductIds;
            if (invalidIds.Length > 0)
            {
                Log.Info($"총 {invalidIds.Length} 개의 잘못된 상품이 있습니다:");
                for (int i = 0; i < invalidIds.Length; i++)
                {
                    Log.Info($"[{i}] {invalidIds[i]}");
                }
            }
            
            _state = PurchaseManagerState.Initialized;
        }

        /// <summary>
        /// 상품구매를 요청한뒤 결과가 리턴되었을 때 호출된다.
        /// </summary>
        private void HandleTransactionStateChange(BillingServicesTransactionStateChangeResult result)
        {
            IBillingTransaction[] transactions = result.Transactions;
            for (int i = 0; i < transactions.Length; i++)
            {
                IBillingTransaction transaction = transactions[i];
                string productId = transaction.Payment.ProductId;
                int quantity = transaction.Payment.Quantity;
                
                switch (transaction.TransactionState)
                {
                    case BillingTransactionState.Deferred:
                        Log.Info($"상품 구매지연됨 ({productId})");
                        break;
                    
                    case BillingTransactionState.Purchasing:
                        Log.Info($"상품 구매중 ({productId})");
                        break;
                    
                    case BillingTransactionState.Purchased:
                        Log.Info($"상품 구매됨 ({productId})");
                        switch (transaction.ReceiptVerificationState)
                        {
                            case BillingReceiptVerificationState.Success:
                                Log.Info($"상품구매영수증 검증이 성공함 ({productId})");
                                break;
                            case BillingReceiptVerificationState.NotDetermined:
                                Log.Info($"상품구매영수증 검증이 결정되지 않았음({productId})");
                                break;
                            case BillingReceiptVerificationState.Failed:
                                Log.Info($"상품구매영수증 검증이 되지 않았음 ({productId})");
                                break;
                            default:
                                Log.Info($"상품구매영수증 검증상태를 알 수 없음 ({productId}, {transaction.ReceiptVerificationState})");
                                break;
                        }
                        OnPurchaseComplete?.Invoke(transaction.Payment.ProductId);
                        break;

                    case BillingTransactionState.Failed:
                        Log.Info($"상품 구매실패 ({transaction.Error.Description})");
                        OnPurchaseFailed?.Invoke(transaction.Payment.ProductId, transaction.Error.Description);
                        break;
                    
                    case BillingTransactionState.Refunded:
                        Log.Info($"상품 환불됨 ({transaction.Payment.ProductId})");
                        break;
                    
                    case BillingTransactionState.Restored:
                        Log.Info($"상품 복구됨 ({transaction.Payment.ProductId})");
                        break;
                    default:
                        throw new Exception($"알수없는 상품구매 트랜잭션 상태 ({transaction.TransactionState})");
                }
            }
        }

        /// <summary>
        /// 구매복구 요청 후 결과가 리턴되었을 때 호출된다.
        /// </summary>
        /// <param name="result">결과</param>
        /// <param name="error">에러</param>
        private void HandleRestoreAllPurchasesComplete(BillingServicesRestorePurchasesResult result, Error error)
        {
            if (error == null)
            {
                IBillingTransaction[] transactions = result.Transactions;
                Log.Info($"구매복구 할 상품은 총 {transactions.Length} 개 입니다:");
                for (int i = 0; i < transactions.Length; i++)
                {
                    IBillingTransaction transaction = transactions[i];
                    Log.Info($"[{i}]: {transaction.Payment.ProductId}");
                    OnRestorePurchase?.Invoke(transaction.Payment.ProductId);
                }
                OnRestoreAllPurchasesComplete?.Invoke(transactions.Length);
            }
            else
            {
                Log.Info($"구매복구 요청이 실패했습니다. ({error.Description})");
            }
        }

        /// <summary>
        /// 상품구매 요청.
        /// </summary>
        /// <param name="productId">상품 Id</param>
        /// <returns>구매요청 성공 여부</returns>
        public bool Purchase(string productId)
        {
            if (!IsFeatureAvailable)
            {
                Log.Warning($"결제서비스를 사용할 수 없어 구매를 진행할 수 없습니다 ({productId})");
                return false;
            }
            
            if (_state != PurchaseManagerState.Initialized)
            {
                Log.Warning($"결제서비스가 초기화되어 있지 않아 구매를 진행할 수 없습니다 ({productId})");
                return false;
            }
                
            if (!BillingServices.CanMakePayments())
            {
                Log.Warning($"결제서비스에서 지불기능을 사용할 수 없어 구매할 수 없습니다 ({productId})");
                return false;
            }

            if (!_products.ContainsKey(productId))
            {
                Log.Warning($"상품을 구매가능목록에서 찾을 수 없어 구매할 수 없습니다 ({productId})");
                return false;
            }

            // TODO: Consumable, Non-Consumable 체크해야하는가?
            // InAppProductSO iapInfo;
            // if (iapInfo.productType == InAppProductType.NonConsumable &&
            // IBillingProduct product = BillingServices.GetProductWithId(productId);
            IBillingProduct product = _products[productId];
            if (BillingServices.IsProductPurchased(product))
            {
                Log.Warning($"이미 구입한 상품입니다 ({productId})");
                return false;
            }
            
            Log.Info($"상품구매 요청 ({productId})");
            return BillingServices.BuyProduct(product);
        }
        
        /// <summary>
        /// 모든 구매항목의 복구를 요청한다.
        /// </summary>
        public bool RestoreAllPurchases()
        {
            if (!IsFeatureAvailable)
            {
                Log.Warning($"결제서비스를 사용할 수 없어 구매복구를 할 수 없습니다");
                return false;
            }
            
            if (_state != PurchaseManagerState.Initialized)
            {
                Log.Warning($"결제서비스가 초기화되어 있지 않아 구매복구를 할 수 없습니다");
                return false;
            }

            Log.Info("구매복구 요청 시작");
            try
            {
                BillingServices.RestorePurchases();
            }
            catch
            {
                Log.Warning("구매복구에 실패했습니다");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 상품의 구매여부를 리턴한다.
        /// </summary>
        /// <param name="productId">상품Id</param>
        /// <returns></returns>
        public bool IsPurchased(string productId)
        {
            if (!IsFeatureAvailable)
            {
                Log.Warning($"결제서비스를 사용할 수 없어 구매여부를 확인할 수 없습니다 ({productId})");
                return false;
            }
            
            if (_state != PurchaseManagerState.Initialized)
            {
                Log.Warning($"결제서비스가 초기화되어 있지 않아 구매여부를 확인할 수 없습니다 ({productId})");
                return false;
            }
            
            if (_products.ContainsKey(productId) == false)
            {
                throw new Exception($"상품을 구매가능목록에서 찾을 수 없어 구매여부 확인을 할 수 없습니다 ({productId})");
            }
            IBillingProduct product = _products[productId];
            // IBillingProduct product = BillingServices.GetProductWithId(productId);
            return BillingServices.IsProductPurchased(product);
        }

        /// <summary>
        /// 스토어에 등재된 아이템인지 확인한다.
        /// </summary>
        public bool IsListingProduct(string productId)
        {
            if (!IsFeatureAvailable)
            {
                Log.Warning($"결제서비스를 사용할 수 없어 등재여부를 확인할 수 없습니다 ({productId})");
                return false;
            }
            
            if (_state != PurchaseManagerState.Initialized)
            {
                Log.Warning($"결제서비스가 초기화되어 있지 않아 등재여부를 확인할 수 없습니다 ({productId})");
                return false;
            }
            return _products.ContainsKey(productId);
        }

        public void LogStatus()
        {
            Log.Info("PurchaseManager Status:");
            Log.Info($"  FeatureAvailable: {IsFeatureAvailable}");
            Log.Info($"  State: {_state}");
        }
        
        private void LogProduct(IBillingProduct product)
        {
            Log.Info($"* {product.Id}");
            Log.Info($"  PlatformId: {product.PlatformId}");
            Log.Info($"  LocalizedTitle: {product.LocalizedTitle}");
            Log.Info($"  LocalizedDescription: {product.LocalizedDescription}");
            Log.Info($"  LocalizedPrice: {product.LocalizedPrice}");
            Log.Info($"  Price: {product.Price}");
            Log.Info($"  PriceCurrencyCode: {product.PriceCurrencyCode}");
        }
        
        public void LogProduct(string productId)
        {
            IBillingProduct product;
            if (_products.TryGetValue(productId, out product))
            {
                LogProduct(product);
            }
            else
            {
                Log.Warning($"목록에 없는 상품의 정보를 얻을 수는 없습니다 ({productId})");
            }
        }
        
        public void LogProductList()
        {
            Log.Info("Products:");
            foreach (var kv in PurchaseManager.Instance.Products)
            {
                Log.Info($"* {kv.Key}");
            }
        }
        
        public void LogPurchasedProductList()
        {
            Log.Info("Purchased Products:");
            foreach (var kv in PurchaseManager.Instance.Products)
            {
                var purchasedId = kv.Key;
                if (IsPurchased(purchasedId)) Log.Info($"* {purchasedId}");
            }
        }
    }
}