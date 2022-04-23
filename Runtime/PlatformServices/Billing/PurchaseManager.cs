using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Rano.PlatformServices.Billing
{
    public sealed class PurchaseManager : MonoSingleton<PurchaseManager>
    {
        #region Platform Dependency Private Fields

        private UnityPurchaseService _purchaseService;

        #endregion
        
        #region Private Fields
        
        [SerializeField] private InAppProductSO[] _rawProducts;
        private Dictionary<string, InAppProduct> _latestProducts = new();

        /// <summary>
        ///     구매상품목록을 주기적으로 업데이트하는지 여부.
        /// </summary>
        [SerializeField] private bool _autoUpdate = true;

        /// <summary>
        ///     구매상품목록 업데이트의 주기 (초)
        /// </summary>
        [SerializeField] private float _autoUpdateInterval = 60f;

        private DateTime _lastUpdatedDateTime;
        
        #endregion

        #region Properties

        public PurchaseServiceState State => _purchaseService.State;

        public InAppProductSO[] RawProducts
        {
            set => _rawProducts = value;
            get => _rawProducts;
        }
        
        public Dictionary<string, InAppProduct> LatestProducts => _latestProducts;

        #endregion

        #region Event Properties

        public Action<string> OnPurchaseComplete { get; set; }
        public Action<string, string> OnPurchaseFailed { get; set; }
        public Action<string> OnRestorePurchase { get; set; }
        public Action OnRestoreAllPurchasesComplete { get; set; }
        public Action OnRestoreAllPurchasesFailed { get; set; }

        /// <summary>
        ///     구매한 상품이었는데 사라진 아이템을 발견했을 때 호출된다.
        /// </summary>
        public Action<string> OnPurchaseRefunded { get; set; }

        /// <summary>
        ///     구매하지 않은 상품이었는데 사라진 아이템을 발견했을 때 호출된다.
        /// </summary>
        public Action<string> OnProductRemoved { get; set; }

        #endregion

        #region Unity Events

        protected override void Awake()
        {
            base.Awake();
            _purchaseService = new UnityPurchaseService(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            // BillingServices.OnInitializeStoreComplete += HandleUpdateStatusComplete;
            // BillingServices.OnTransactionStateChange += HandleTransactionStateChange;
            // BillingServices.OnRestorePurchasesComplete += HandleRestoreAllPurchasesComplete;
            StartCoroutine(nameof(UpdateCoroutine));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            // BillingServices.OnInitializeStoreComplete -= HandleUpdateStatusComplete;
            // BillingServices.OnTransactionStateChange -= HandleTransactionStateChange;
            // BillingServices.OnRestorePurchasesComplete -= HandleRestoreAllPurchasesComplete;
            StopCoroutine(nameof(UpdateCoroutine));
        }

        #endregion
        
        #region Coroutine Methods

        private IEnumerator UpdateCoroutine()
        {
            while (true)
            {
                yield return YieldCache.WaitForSeconds(_autoUpdateInterval);
                if (_autoUpdate) UpdateStatus();
            }
        }

        #endregion

        #region Action Methods

        public void Initialize()
        {
            Log.Info("구매서비스 초기화 시작.");
            _purchaseService.Initialize();
        }
        
        /// <summary>
        ///     결제서비스 서버로부터 상품정보들을 로컬로 캐싱받는다.
        /// </summary>
        public void UpdateStatus()
        {
            Log.Info("구매서비스 상태 업데이트 요청.");
            _purchaseService.UpdateStatus();
        }

        /// <summary>
        ///     상품구매 요청.
        /// </summary>
        /// <param name="productId">상품 Id</param>
        public void Purchase(string productId)
        {
            Log.Info($"구매요청 ({productId})");
            _purchaseService.Purchase(productId);
        }

        /// <summary>
        ///     모든 구매항목의 복구를 요청한다.
        /// </summary>
        public void RestoreAllPurchases()
        {
            Log.Info($"구매복구 요청");
            _purchaseService.RestoreAllPurchases();
        }

        /// <summary>
        ///     상품의 구매여부를 리턴한다.
        /// </summary>
        /// <remarks>서버가 아닌 로컬에 캐시된 정보를 리턴한다.</remarks>
        /// <param name="productId">상품Id</param>
        /// <returns>구매 여부</returns>
        public bool IsPurchased(string productId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Log Methods

        public void LogStatus()
        {
            throw new NotImplementedException();
        }

        public void LogProduct(string productId)
        {
            InAppProduct product;
            if (LatestProducts.TryGetValue(productId, out product))
                product.LogStatus();
            else
                Log.Warning($"목록에 없는 상품의 정보를 얻을 수는 없습니다 ({productId})");
        }

        public void LogProductList()
        {
            Log.Info("Products:");
            foreach (var kv in Instance.LatestProducts)
            {
                var product = kv.Value;
                string purchasedStr;
                try
                {
                    purchasedStr = product.IsPurchased ? "purchased" : "not purchased";
                }
                catch
                {
                    purchasedStr = "unknown";
                }
                Log.Info($"* {kv.Key} ({purchasedStr})");
            }
        }

        #endregion
    }
}