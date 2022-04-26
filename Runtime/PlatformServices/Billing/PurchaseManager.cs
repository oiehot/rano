using System;
using UnityEngine;

namespace Rano.PlatformServices.Billing
{
    public sealed class PurchaseManager : MonoSingleton<PurchaseManager>
    {
        private PurchaseHistory _purchaseHistory;
        private UnityPurchaseService _purchaseService;
        [SerializeField] private InAppProductSO[] _rawProducts;
        public InAppProductSO[] RawProducts
        {
            get => _rawProducts;
            set => _rawProducts = value;
        }
        
        public PurchaseServiceState State => _purchaseService.State;
        
        public Action<InAppProduct[]> OnInitialized { get; set; }
        public Action OnInitializeFailed { get; set; }
        public Action<string> OnPurchaseComplete { get; set; }
        public Action<string, string> OnPurchaseFailed { get; set; }
        public Action<InAppProduct> OnValidatePurchaseSuccess { get; set; }
        public Action<string> OnValidatePurchaseFailed { get; set; }
        public Action OnRestoreAllPurchasesComplete { get; set; }
        public Action OnRestoreAllPurchasesFailed { get; set; }

        public bool IsAvailable => _purchaseService.State == PurchaseServiceState.Available;

        protected override void Awake()
        {
            base.Awake();
            _purchaseHistory = this.GetRequiredComponent<PurchaseHistory>();
            _purchaseService = this.GetRequiredComponent<UnityPurchaseService>();            
        }
        
        public void Initialize()
        {
            Log.Info("구매서비스 초기화 시작.");
            _purchaseService.Initialize();
        }
        
        public void Purchase(string productId)
        {
            Log.Info($"구매요청 ({productId})");
            _purchaseService.Purchase(productId);
        }

        public void RestoreAllPurchases()
        {
            Log.Info($"구매복구 요청");
            _purchaseService.RestoreAllPurchases();
        }

        public InAppProduct GetProduct(string productId)
        {
            return _purchaseService.GetProduct(productId);
        }

        public InAppProduct[] GetProducts()
        {
            return _purchaseService.GetProducts();
        }
        
        /// <summary>
        /// 구매여부 반환
        /// </summary>
        /// <remarks>로컬 데이터로 부터 값을 얻는다. </remarks>
        public bool IsPurchased(string productId)
        {
            return _purchaseHistory.IsPurchased(productId);
        }
        
        [ContextMenu("Log Status")]
        public void LogStatus()
        {
            Log.Info("PurchaseManager Status:");
            Log.Info($"  State: {State}");
            Log.Info($"  IsAvailable: {IsAvailable}");
            Log.Info($"  rawProducts: {_rawProducts.Length}");
        }
        
        public void LogProduct(string productId)
        {
            if (!IsAvailable)
            {
                Log.Warning($"초기화되지 않았습니다 (state:{State})");
                return;
            }
            var product = GetProduct(productId);
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
            
            var products = GetProducts();
            if (products == null)
            {
                Log.Warning($"상품들을 찾을 수 없습니다.");
                return;
            }
            Log.Info("Products:");
            foreach (var product in products)
            {
                string purchasedStr;
                try
                {
                    bool purchased = IsPurchased(product.id);
                    purchasedStr = purchased ? "purchased" : "not purchased";
                }
                catch
                {
                    purchasedStr = "unknown (not found in history)";
                }
                Log.Info($"* {product.id} ({purchasedStr})");
            }
        }
    }
}