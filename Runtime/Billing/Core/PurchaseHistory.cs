using System;
using System.Collections.Generic;
using UnityEngine;
using Rano.SaveSystem;

namespace Rano.Billing
{
    [Serializable]
    public sealed class PurchaseHistorySaveData
    {
        public Dictionary<string, InAppProduct> products;
    }
    
    public sealed class PurchaseHistory : MonoBehaviour, IPurchaseHistory, ISaveLoadable
    {
        private IPurchaseManager _purchaseManager;
        private Dictionary<string, InAppProduct> _products = new();
        
        public Action OnInitialized { get; set; }
        public Action OnUpdated { get; set; }
        
        public Dictionary<string, InAppProduct> Products => _products;

        private void Awake()
        {
            _purchaseManager = this.GetRequiredComponent<IPurchaseManager>();
        }
        
        private void OnEnable()
        {
            _purchaseManager.onInitialized += HandleInitialized;
            _purchaseManager.onUpdateSuccess += HandleUpdated;
            _purchaseManager.onValidatePurchaseSuccess += HandleValidatePurchaseSuccess;
            _purchaseManager.onValidatePurchaseFailed += HandleValidatePurchaseFailed;
        }

        private void OnDisable()
        {
            _purchaseManager.onInitialized -= HandleInitialized;
            _purchaseManager.onUpdateSuccess -= HandleUpdated;
            _purchaseManager.onValidatePurchaseSuccess -= HandleValidatePurchaseSuccess;
            _purchaseManager.onValidatePurchaseFailed -= HandleValidatePurchaseFailed;
        }

        private void HandleInitialized(InAppProduct[] products)
        {
            Log.Info($"PurchaseManager가 초기화되어 상품목록을 PurchaseHistory로 가져옵니다 (src:{products.Length}, current:{_products.Count})");
            foreach (var product in products)
            {
                UpdateProduct(product);
            }
            OnInitialized?.Invoke();
        }

        private void HandleUpdated(InAppProduct[] products)
        {
            Log.Info($"PurchaseManager가 업데이트되어 상품목록을 PurchaseHistory로 가져옵니다 (src:{products.Length}, current:{_products.Count})");
            foreach (var product in products)
            {
                UpdateProduct(product);
            }
            OnUpdated?.Invoke();
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
                savedProduct.lastValidateDateTime = DateTime.UtcNow;
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
                product.lastValidateDateTime = DateTime.UtcNow;
            }
            else
            {
                Log.Warning($"상품이 없어서 검증상태를 업데이트할 수 없음 ({productId})");
            }
        }

        private void UpdateProduct(InAppProduct product)
        {
            #if UNITY_EDITOR
                if (_products.ContainsKey(product.id) == false)
                {
                    Log.Info($"PurchaseHistory 상품정보 업데이트됨 ({product.id}, *신규)");
                    _products[product.id] = product;
                }
                else
                {
                    // 주의: 에디터에서는 이미 있으면 업데이트 하지 않는다. 로컬데이터가 우선시 되기 때문이다.
                    Log.Info($"PurchaseHistory 상품정보 업데이트 생략됨 ({product.id}, reason:로컬데이터 우선)");
                }
            #else
                Log.Info($"PurchaseHistory 상품정보 업데이트됨 ({product.id})");
                _products[product.id] = product;
            #endif   
        }

        public void SetValidate(string productId, bool value)
        {
            if (_products.TryGetValue(productId, out var product))
            {
                // Log.Info($"PurchaseHistory에 저장된 상품의 검증상태를 업데이트합니다 (productId:{productId}, validate:{value})");
                product.validated = value;
                product.lastValidateDateTime = DateTime.UtcNow;
            }
            else
            {
                throw new NotFoundProductException(productId);
            }
        }

        public bool IsPurchased(InAppProduct product)
        {
            if (product.type == InAppProductType.NonConsumable && product.validated)
            {
                return true;
            }
            if (product.type == InAppProductType.Subscription)
            {
                throw new NotImplementedException($"구독형 상품에 대한 구매여부를 알아낼 수 없습니다");
            }
            return false;
        }

        public bool IsPurchased(string productId)
        {
            if (_products.TryGetValue(productId, out var product))
            {
                return IsPurchased(product);
            }
            else
            {
                throw new NotFoundProductException(productId);
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
            Log.Info($"{nameof(PurchaseHistory)} Status:");
            Log.Info($"  products:");
            foreach (var kv in _products)
            {
                string productId = kv.Key;
                InAppProduct product = kv.Value;
                int receiptLen = String.IsNullOrEmpty(product.receipt) ? 0 : product.receipt.Length; 
                Log.Info($"    - {productId} (type:{product.type}, hasReceipt:{product.hasReceipt}, receiptLen:{receiptLen}, validated:{product.validated}, purchased:{IsPurchased(product.id)}, purchaseCount:{product.purchaseCount})");
            }
        }

        public void LogProduct(string productId)
        {
            if (_products.TryGetValue(productId, out var product))
            {
                product.LogStatus();
            }
            else
            {
                Log.Warning($"상품을 찾을 수 없습니다 ({productId})");
            }
        }

        public void LogProducts()
        {
            Log.Info($"{nameof(PurchaseHistory)} Products: ({_products.Count})");
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