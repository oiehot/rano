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
            if (BillingServices.IsAvailable())
            {
                try
                {
                    BillingServices.InitializeStore();
                }
                catch
                {
                    Log.Warning($"IAP 스토어 초기화에 실패했습니다.");
                    _state = PurchaseManagerState.NotInitialized;
                    return;
                }

                _state = PurchaseManagerState.Initialized;
            }
            else
            {
                Log.Warning("IAP 결제서비스를 사용할 수 없습니다.");
                _state = PurchaseManagerState.NotInitialized;
            }
        }


        private void HandleInitializeStoreComplete(BillingServicesInitializeStoreResult result, Error error)
        {
            if (error == null)
            {
                IBillingProduct[] products = result.Products;
                Log.Info("인앱상품 결제서비스 초기화 완료.");
                Log.Info($"총 {products.Length} 개의 상품이 있습니다:");
                for (int i = 0; i < products.Length; i++)
                {
                    IBillingProduct product = products[i];
                    _products.Add(product.Id, product);
                    Log.Info($"[{i}] {product}");
                }
            }
            else
            {
                Log.Warning($"인앱상품 결제서비스 초기화 실패: {error}");
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
        }

        /// <summary>
        /// 상품구매를 요청한뒤 결과가 리턴되었을 때 호출된다.
        /// </summary>
        /// <param name="result">결과</param>
        private void HandleTransactionStateChange(BillingServicesTransactionStateChangeResult result)
        {
            IBillingTransaction[] transactions = result.Transactions;
            for (int i = 0; i < transactions.Length; i++)
            {
                IBillingTransaction transaction = transactions[i];
                switch (transaction.TransactionState)
                {
                    case BillingTransactionState.Purchased:
                        // TODO: 영수증 검증절차.
                        if (transaction.ReceiptVerificationState == BillingReceiptVerificationState.Success)
                        {
                            Log.Info($"상품구매 성공. ({transaction.Payment.ProductId})");
                            OnPurchaseComplete?.Invoke(transaction.Payment.ProductId);
                        }
                        else
                        {
                            Log.Warning($"상품구매 영수증 검증실패. ({transaction.Payment.ProductId})");
                        }
                        break;

                    case BillingTransactionState.Failed:
                        Log.Info($"상품구매 실패. ({transaction.Error.Description})");
                        OnPurchaseFailed?.Invoke(transaction.Payment.ProductId, transaction.Error.Description);
                        break;
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
            if (BillingServices.CanMakePayments() == false)
            {
                Log.Warning("결제서비스에서 지불기능을 사용할 수 없어 구매할 수 없습니다.");
                return false;
            }

            if (_products.ContainsKey(productId) == false)
            {
                Log.Warning($"구매에 실패했습니다. 상품 {productId}을 구매가능목록에서 찾을 수 없습니다.");
                return false;
            }

            IBillingProduct product = _products[productId];

            if (BillingServices.IsProductPurchased(product) == false)
            {
                Log.Info($"인앱상품 구매 요청: {product}");
                BillingServices.BuyProduct(product);
                return true;
            }
            else
            {
                Log.Warning($"이미 구매한 상품이어서 재구매할 수 없습니다: {product}");
                return false;
            }
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        /// <summary>
        /// 구매목록을 삭제한다. 클라우드 혹은 시뮬레이터에 남아있는 구매목록이 삭제되는 것은 아니다.
        /// </summary>
        public void ClearPurchaseHistory()
        {
            Log.Info("구매목록 삭제.");
            BillingServices.ClearPurchaseHistory();
        }
#endif

        /// <summary>
        /// 모든 구매항목의 복구를 요청한다.
        /// </summary>
        public void RestoreAllPurchases()
        {
            Log.Info("모든 구매항목의 복구를 요청.");
            BillingServices.RestorePurchases();
        }

        /// <summary>
        /// 특정 항목의 복구를 요청.
        /// </summary>
        public void RestorePurchase(string productId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 상품의 구매가능 여부를 리턴한다.
        /// </summary>
        /// <param name="productId">상품Id</param>
        /// <returns></returns>
        public bool IsPurchased(string productId)
        {
            if (_products.ContainsKey(productId) == false)
            {
                throw new Exception($"구매여부 확인에 실패했습니다. 상품 {productId}을 구매가능목록에서 찾을 수 없습니다.");
            }

            IBillingProduct product = _products[productId];
            return BillingServices.IsProductPurchased(product);
        }

        public bool IsBuyable(string productId)
        {
            return _products.ContainsKey(productId);
        }
    }
}