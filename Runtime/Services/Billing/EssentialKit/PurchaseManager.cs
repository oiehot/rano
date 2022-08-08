#if USE_ESSENTIAL_KIT_BILLING_SERVICES

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;

namespace Rano.Services.Billing.EssentialKit
{
    
    public enum PurchaseManagerState
    {
        NotUpdated,
        Updating,
        Updated
    }
    
    public sealed class PurchaseManager : ManagerComponent
    {
        /// <summary>
        /// 구매상품목록을 주기적으로 업데이트하는지 여부.
        /// </summary>
        [SerializeField] private bool _autoUpdate = true;

        /// <summary>
        /// 구매상품목록 업데이트의 주기 (초)
        /// </summary>
        [SerializeField] private float _autoUpdateInterval = 60f;
        
        private DateTime _lastUpdatedDateTime;
        private PurchaseManagerState _state = PurchaseManagerState.NotUpdated;
        public PurchaseManagerState State => _state;

        public bool IsUpdating => (_state == PurchaseManagerState.Updating);
        public bool IsUpdated => (_state == PurchaseManagerState.Updated);
        
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
        private Dictionary<string, InAppProduct> _products = new Dictionary<string, InAppProduct>();
        
        public Dictionary<string, InAppProduct> Products => _products;
        
        public Action<string> OnPurchaseComplete { get; set; }
        public Action<string, string> OnPurchaseFailed { get; set; }
        public Action<string> OnRestorePurchase { get; set; }
        public Action<int> OnRestoreAllPurchasesComplete { get; set; }
        
        /// <summary>
        /// 구매한 상품이었는데 사라진 아이템을 발견했을 때 호출된다.
        /// </summary>
        public Action<string> OnPurchaseRefunded { get; set; }
        
        /// <summary>
        /// 구매하지 않은 상품이었는데 사라진 아이템을 발견했을 때 호출된다.
        /// </summary>
        public Action<string> OnProductRemoved { get; set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            BillingServices.OnInitializeStoreComplete += HandleUpdateStatusComplete;
            BillingServices.OnTransactionStateChange += HandleTransactionStateChange;
            BillingServices.OnRestorePurchasesComplete += HandleRestoreAllPurchasesComplete;
            StartCoroutine(nameof(UpdateCoroutine));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            BillingServices.OnInitializeStoreComplete -= HandleUpdateStatusComplete;
            BillingServices.OnTransactionStateChange -= HandleTransactionStateChange;
            BillingServices.OnRestorePurchasesComplete -= HandleRestoreAllPurchasesComplete;
            StopCoroutine(nameof(UpdateCoroutine));
        }

        private IEnumerator UpdateCoroutine()
        {
            while (true)
            {
                yield return YieldCache.WaitForSeconds(_autoUpdateInterval);
                if (_autoUpdate) UpdateStatus();
            }
        }

        /// <summary>
        /// 결제서비스 서버로부터 상품정보들을 로컬로 캐싱받는다. 
        /// </summary>
        public void UpdateStatus()
        {
            _state = PurchaseManagerState.Updating;
            
            var beforeProducts = _products;
            _products.Clear();
            
            if (IsFeatureAvailable)
            {
                try
                {
                    Log.Info("결제서비스 상품 업데이트 요청됨");
                    BillingServices.InitializeStore(); // 비동기임
                }
                catch
                {
                    Log.Warning($"결제서비스 상품 업데이트에 실패했습니다.");
                    _state = PurchaseManagerState.NotUpdated;
                    return;
                }
            }
            else
            {
                Log.Warning("결제서비스를 사용할 수 없어 상품들을 업데이트 할 수 없습니다.");
                _state = PurchaseManagerState.NotUpdated;
                return;
            }
            CheckProductsDifference(beforeProducts, _products);
        }

        /// <summary>
        /// 전/후 제품목록을 비교하여 환불되었거나 항목이 사라진 아이템을 체크하여 적절한 이벤트를 발생시킨다.
        /// </summary>
        /// <param name="before">업데이트 이전 상품목록</param>
        /// <param name="after">업데이트 이후 상품목록</param>
        private void CheckProductsDifference(Dictionary<string, InAppProduct> before, Dictionary<string, InAppProduct> after)
        {
            // 이전에는 구매했었으나 지금은 구매되어있지 않은 상품을 발견하면
            // OnPurchaseRemoved 콜백을 호출한다.
            foreach (var kv in after)
            {
                string productId = kv.Key;
                InAppProduct product = kv.Value;
                InAppProduct beforeProduct;
                if (before.TryGetValue(productId, out beforeProduct))
                {
                    if (beforeProduct.IsPurchased && !product.IsPurchased)
                    {
                        Log.Info($"구매하신 상품이 환불되었습니다 ({productId})");
                        OnPurchaseRefunded?.Invoke(productId);
                    }
                }
            }

            // 이전에는 존재했었고 구입했었으나, 현재는 사라진 혹은 구매되지 않은 상품을 발견하면
            // OnPurchaseRemoved 콜백을 호출한다.
            foreach (var kv in before)
            {
                string productId = kv.Key;
                InAppProduct beforeProduct = kv.Value;
                bool afterContains = after.ContainsKey(productId);
                if (afterContains == false)
                {
                    if (beforeProduct.IsPurchased)
                    {
                        // 이전에 구매했었으나, 지금은 사라진 아이템일 때 => 상품 제거됨
                        Log.Info($"구매하신 상품을 더이상 사용할 수 없습니다 ({productId})");
                        OnProductRemoved?.Invoke(productId);
                    }
                    else
                    {
                        // 이전에 구매하지 않았으나, 지금은 사라진 아이템일 때 => 상품 제거됨
                        Log.Info($"상품을 더이상 사용할 수 없습니다 ({productId})");
                        OnProductRemoved?.Invoke(productId);
                    }
                }
            }
        }
        
        private void HandleUpdateStatusComplete(BillingServicesInitializeStoreResult result, Error error)
        {
            if (error == null)
            {
                IBillingProduct[] products = result.Products;
                Log.Info($"구매상품목록 업데이트 성공 ({products.Length})");
                
                // 넘겨받은 IBillingProduct를 내부적으로 저장한다.
                for (int i = 0; i < products.Length; i++)
                {
                    IBillingProduct product = products[i];
                    var purchased = BillingServices.IsProductPurchased(product);
                    
                    InAppProduct iapProduct = new InAppProduct(
                        id: product.Id,
                        platformId: product.PlatformId,
                        localizedTitle: product.LocalizedTitle,
                        localizedDescription: product.LocalizedDescription,
                        localizedPrice: product.LocalizedPrice,
                        price: product.Price,
                        priceCurrencyCode: product.PriceCurrencyCode,
                        purchased: purchased,
                        tag: product.Tag
                    );
                    
                    _products.Add(iapProduct.Id, iapProduct);
                }
            }
            else
            {
                Log.Warning($"구매상품목록 업데이트 실패 ({error})");
                _state = PurchaseManagerState.NotUpdated;
                return;
            }

            string[] invalidIds = result.InvalidProductIds;
            if (invalidIds.Length > 0)
            {
                Log.Warning($"구매상품목록 업데이트중 잘못된 상품들을 발견하였습니다. ({invalidIds.Length})");
                for (int i = 0; i < invalidIds.Length; i++)
                {
                    Log.Warning($"[{i}] {invalidIds[i]}");
                }
            }
            
            _state = PurchaseManagerState.Updated;
            _lastUpdatedDateTime = DateTime.Now;
        }

#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
        public void VerificationTransactions()
        {
            Log.Info($"BillingServices.GetTransactions 시작");
            // Fetch all pending transactions
            var transactions = BillingServices.GetTransactions();
            
            Log.Info($"=> transactions (count:{transactions.Length})");
            
            // Process each transaction
            foreach (var transaction in transactions)
            {
                string productId = transaction.Payment.ProductId;
                
                // TODO: Verify the receipt with your server - This is an async call usually
                // TODO: Set the ReceiptVerificationState to Success or Failed
                transaction.ReceiptVerificationState = BillingReceiptVerificationState.Success;
                
                Log.Info($"테스트로 영수증 검증 완료시킴 {productId}");
            }

            // TODO: Call Finish Transactions to clear the pending transaction queue
            Log.Info($"BillingServices.FinishTransactions 시작");
            BillingServices.FinishTransactions(transactions);
        }
#endif

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
                                // 상품이 구매되었으므로 구입여부 플래그를 켠다.
                                // (스토어로 부터 최신 정보를 가져오지 않더라도 최신 정보를 활용할 수 있도록)
                                _products[productId].SetPurchaseFlag(true);
                                OnPurchaseComplete?.Invoke(transaction.Payment.ProductId);
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
                        break;

                    case BillingTransactionState.Failed:
                        Log.Info($"상품 구매실패 ({transaction.Error.Description})");
                        OnPurchaseFailed?.Invoke(transaction.Payment.ProductId, transaction.Error.Description);
                        break;
                    
                    case BillingTransactionState.Refunded:
                        // TODO: iOS에서만 작동되는것으로 보인다. Android에서는 작동안됨.
                        // TODO: 여기서 OnPurchaseRemove를 호출하지 않는다.
                        // TODO: 상품목록 업데이트시 차이에서 제외되는 것을 확인했을 때, 호출할 예정.
                        Log.Info($"상품 환불됨 ({transaction.Payment.ProductId})");
                        break;
                    
                    case BillingTransactionState.Restored:
                        Log.Info($"상품 복구됨 ({transaction.Payment.ProductId})");
                        break;
                    
                    default:
                        throw new Exception($"알수없는 상품구매 트랜잭션 상태 ({transaction.TransactionState})");
                }

                // TMP: Clear
                BillingServices.FinishTransactions(transactions);
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
            
            if (_state != PurchaseManagerState.Updated)
            {
                Log.Warning($"구매상품목록이 업데이트 되어있지 않아 구매를 진행할 수 없습니다 ({productId})");
                return false;
            }
                
            if (!BillingServices.CanMakePayments())
            {
                Log.Warning($"결제서비스에서 지불기능을 사용할 수 없어 구매할 수 없습니다 ({productId})");
                return false;
            }

            if (!_products.ContainsKey(productId))
            {
                Log.Warning($"구매상품목록에서 해당상품을 찾을 수 없어 구매할 수 없습니다 ({productId})");
                return false;
            }
;
            var product = _products[productId];
            if (_products[productId].IsPurchased)
            {
                Log.Warning($"이미 구입한 상품입니다 ({productId})");
                return false;
            }
            
            Log.Info($"상품구매 요청 ({productId})");
            return BillingServices.BuyProduct(product);
        }
        
#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
        public bool RefundTest(string productId)
        {
            if (_state != PurchaseManagerState.Updated)
            {
                Log.Warning($"구매상품목록이 업데이트 되어있지 않아 환불을 진행할 수 없습니다 ({productId})");
                return false;
            }
            if (!_products.ContainsKey(productId))
            {
                Log.Warning($"구매상품목록에서 해당상품을 찾을 수 없어 구매할 수 없습니다 ({productId})");
                return false;
            }
            var product = _products[productId];
            if (product.IsPurchased == false)
            {
                Log.Warning($"구매하지 않은 상품을 환불할 수는 없습니다 ({productId})");
                return false;
            }
            Log.Info($"상품을 환불합니다 ({productId})");
            var beforeProducts = _products.ToDictionary(
                kv => kv.Key,
                kv => (InAppProduct)kv.Value.Clone()
            );
            product.SetPurchaseFlag(false);
            CheckProductsDifference(beforeProducts, _products);
            return true;
        }
#endif
        
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
            
            if (_state != PurchaseManagerState.Updated)
            {
                Log.Warning($"구매상품목록이 업데이트 되어있지 않아 구매복구를 할 수 없습니다");
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
        /// <remarks>서버가 아닌 로컬에 캐시된 정보를 리턴한다.</remarks>
        /// <param name="productId">상품Id</param>
        /// <returns>구매 여부</returns>
        public bool IsPurchased(string productId)
        {
            if (_state != PurchaseManagerState.Updated)
            {
                Log.Warning($"구매상품목록이 업데이트 되어있지 않아 구매여부 확인을 할 수 없습니다");
                return false;
            }
            if (_products.ContainsKey(productId) == false)
            {
                throw new Exception($"상품을 구매가능목록에서 찾을 수 없어 구매여부 확인을 할 수 없습니다 ({productId})");
            }
            return _products[productId].IsPurchased;
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
            
            if (_state != PurchaseManagerState.Updated)
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
            Log.Info($"  CachedProductCount: {_products.Count}");
            Log.Info($"  LastUpdatedDateTime: {_lastUpdatedDateTime}");
        }
        
        private void LogProduct(InAppProduct product)
        {
            Log.Info($"- {product.Id}");
            Log.Info($"  PlatformId: {product.PlatformId}");
            Log.Info($"  LocalizedTitle: {product.LocalizedTitle}");
            Log.Info($"  LocalizedDescription: {product.LocalizedDescription}");
            Log.Info($"  LocalizedPrice: {product.LocalizedPrice}");
            Log.Info($"  Price: {product.Price}");
            Log.Info($"  PriceCurrencyCode: {product.PriceCurrencyCode}");
            Log.Info($"  Purchased: {product.IsPurchased}");
        }
        
        public void LogProduct(string productId)
        {
            InAppProduct product;
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
            Log.Info("Products (in local):");
            foreach (var kv in PurchaseManager.Instance.Products)
            {
                var product = kv.Value;
                string pstr = product.IsPurchased ? "purchased" : "not purchased";
                Log.Info($"* {kv.Key} ({pstr})");
            }
        }
    }
}

#endif