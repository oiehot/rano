using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;

namespace Rano.PlatformServices.Billing
{
    public enum PurchaseManagerState
    {
        NotUpdated,
        Updating,
        Updated
    }

    public sealed class PurchaseManager : MonoSingleton<PurchaseManager>
    {

        #region Private Fields
        
        private PurchaseManagerState _state = PurchaseManagerState.NotUpdated;
        private Dictionary<string, InAppProduct> _products = new();

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
        
        public PurchaseManagerState State => _state;
        
        /// <summary>
        ///     PlatformId에 매칭되는 Product 객체를 담는 사전.
        ///     PlatformId가 아닌 Settings에 적혀진 ProductId를 키로 사용한다.
        /// </summary>
        public Dictionary<string, InAppProduct> Products => _products;
        public bool IsUpdating => _state == PurchaseManagerState.Updating;
        public bool IsUpdated => _state == PurchaseManagerState.Updated;

        public bool IsFeatureAvailable => false;

        #endregion

        #region Event Properties

        public Action<string> OnPurchaseComplete { get; set; }
        public Action<string, string> OnPurchaseFailed { get; set; }
        public Action<string> OnRestorePurchase { get; set; }
        public Action<int> OnRestoreAllPurchasesComplete { get; set; }

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

        #region EventHandler Methods

        private void HandleUpdateStatusComplete(BillingServicesInitializeStoreResult result, Error error)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     상품구매를 요청한뒤 결과가 리턴되었을 때 호출된다.
        /// </summary>
        private void HandleTransactionStateChange(BillingServicesTransactionStateChangeResult result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     구매복구 요청 후 결과가 리턴되었을 때 호출된다.
        /// </summary>
        private void HandleRestoreAllPurchasesComplete(BillingServicesRestorePurchasesResult result, Error error)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Action Methods

        /// <summary>
        ///     결제서비스 서버로부터 상품정보들을 로컬로 캐싱받는다.
        /// </summary>
        public void UpdateStatus()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     상품구매 요청.
        /// </summary>
        /// <param name="productId">상품 Id</param>
        /// <returns>구매요청 성공 여부</returns>
        public bool Purchase(string productId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     모든 구매항목의 복구를 요청한다.
        /// </summary>
        public bool RestoreAllPurchases()
        {
            throw new NotImplementedException();
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
            if (Products.TryGetValue(productId, out product))
                LogProduct(product);
            else
                Log.Warning($"목록에 없는 상품의 정보를 얻을 수는 없습니다 ({productId})");
        }

        public void LogProductList()
        {
            Log.Info("Products:");
            foreach (var kv in Instance.Products)
            {
                var product = kv.Value;
                var pstr = product.IsPurchased ? "purchased" : "not purchased";
                Log.Info($"* {kv.Key} ({pstr})");
            }
        }

        #endregion
    }
}