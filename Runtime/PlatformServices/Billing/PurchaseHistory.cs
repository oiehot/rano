using System;
using System.Collections.Generic;
using UnityEngine;
using Rano.SaveSystem;

namespace Rano.PlatformServices.Billing
{
    [Serializable]
    public sealed class PurchaseHistorySaveData
    {
        public Dictionary<string, InAppProduct> products;
    }
    
    public sealed class PurchaseHistory : MonoBehaviour, ISaveLoadable
    {
        private PurchaseManager _purchaseManager;
        private Dictionary<string, InAppProduct> _products = new();
        public Dictionary<string, InAppProduct> Products => _products;

        private void Awake()
        {
            _purchaseManager = this.GetRequiredComponent<PurchaseManager>();
        }
        private void OnEnable()
        {
            _purchaseManager.OnInitialized += HandleInitialized;
            _purchaseManager.OnValidatePurchaseSuccess += HandleValidatePurchaseSuccess;
            _purchaseManager.OnValidatePurchaseFailed += HandleValidatePurchaseFailed;
        }

        private void OnDisable()
        {
            _purchaseManager.OnInitialized -= HandleInitialized;
            _purchaseManager.OnValidatePurchaseSuccess -= HandleValidatePurchaseSuccess;
            _purchaseManager.OnValidatePurchaseFailed -= HandleValidatePurchaseFailed;
        }

        private void HandleInitialized(InAppProduct[] products)
        {
            Log.Info($"상품목록을 PurchaseHistory로 가져옵니다 (src:{products.Length}, current:{_products.Count})");
            foreach (var product in products)
            {
                AddProduct(product);
            }
        }

        private void HandleValidatePurchaseSuccess(InAppProduct newProduct)
        {
            string productId = newProduct.id;
            if (_products.TryGetValue(productId, out var savedProduct))
            {
                UnityEngine.Debug.Assert(newProduct.hasReceipt == true);
                Log.Info($"PurchaseHistory에 저장된 상품의 검증상태를 True로 업데이트합니다 ({productId})");
                savedProduct.receipt = newProduct.receipt;
                savedProduct.hasReceipt = newProduct.hasReceipt;
                savedProduct.validated = true;
                savedProduct.lastValidateDateTime = DateTime.Now;
                savedProduct.purchaseCount++;
            }
            else
            {
                Log.Warning($"상품이 없어서 검증상태를 업데이트할 수 없음 ({productId})");
            }
        }

        private void HandleValidatePurchaseFailed(string productId)
        {
            if (_products.TryGetValue(productId, out var product))
            {
                Log.Info($"PurchaseHistory에 저장된 상품의 검증상태를 False로 업데이트합니다 ({productId})");
                product.validated = false;
                product.lastValidateDateTime = DateTime.Now;
            }
            else
            {
                Log.Warning($"상품이 없어서 검증상태를 업데이트할 수 없음 ({productId})");
            }
        }

        private void AddProduct(InAppProduct product)
        {
            // 주의: 에디터에서는 이미 있으면 업데이트 하지 않는다. 로컬데이터가 우선시 되기 때문이다.
            #if (UNITY_EDITOR)
                if (_products.ContainsKey(product.id) == false)
                {
                    _products[product.id] = product;
                }
            #else
                _products[product.id] = product;
            #endif   
        }

        public void SetValidate(string productId, bool value)
        {
            if (_products.TryGetValue(productId, out var product))
            {
                Log.Info($"PurchaseHistory에 저장된 상품의 검증상태를 업데이트합니다 (productId:{productId}, validate:{value})");
                product.validated = value;
                product.lastValidateDateTime = DateTime.Now;
            }
            else
            {
                Log.Warning($"상품이 없어서 검증여부를 설정할 수 없음 ({productId})");
            }
        }

        public bool IsPurchased(string productId)
        {
            if (_products.TryGetValue(productId, out var product))
            {
                if (product.type == InAppProductType.Subscription)
                {
                    throw new Exception($"구독형 상품에 대한 구매여부를 알아낼 수 없습니다");
                }

                if (product.type == InAppProductType.NonConsumable && product.validated)
                {
                    return true;
                }
                
                return false;
            }
            else
            {
                Log.Warning($"상품이 없어서 구매여부를 얻을 수 없음 ({productId})");
                return false;
            }
        }

        [ContextMenu("Clear History")]
        public void ClearHistory()
        {
            Log.Info($"상품목록을 비웁니다.");
            _products.Clear();
        }

        [ContextMenu("Log Status")]
        public void LogStatus()
        {
            Log.Info($"Products in {nameof(PurchaseHistory)} ({_products.Count})");
            foreach (var kv in _products)
            {
                InAppProduct product = kv.Value;
                product.LogStatus();
            }
        }
        
        public void ClearState()
        {
            _products.Clear();
        }

        public void DefaultState()
        {
            _products.Clear();
        }

        public object CaptureState()
        {
            var state = new PurchaseHistorySaveData();
            state.products = _products;
            return state;
        }

        public void ValidateState(object state)
        {
            var saveData = (PurchaseHistorySaveData) state;
            if (saveData.products == null) throw new Exception("상품목록이 null일 수는 없습니다");
        }

        public void RestoreState(object state)
        {
            var saveData = (PurchaseHistorySaveData)state;
            _products = saveData.products;
        }
    }
}