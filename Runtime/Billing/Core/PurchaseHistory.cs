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
    
    /// <summary>
    /// 구매 영수증을 보관한다.
    /// </summary>
    public sealed class PurchaseHistory : MonoBehaviour, IPurchaseHistory, ISaveLoadable
    {
        private IPurchaseManager _purchaseManager;
        private Dictionary<string, InAppProduct> _products = new();

        public event Action OnInitialized;
        public event Action OnUpdated;
        
        public Dictionary<string, InAppProduct> Products => _products;

        private void Awake()
        {
            _purchaseManager = this.GetRequiredComponent<IPurchaseManager>();
        }
        
        private void OnEnable()
        {
            _purchaseManager.OnInitialized += HandleInitialized;
            _purchaseManager.OnUpdateSuccess += HandleUpdated;
            _purchaseManager.OnValidatePurchaseSuccess += HandleValidatePurchaseSuccess;
            _purchaseManager.OnValidatePurchaseFailed += HandleValidatePurchaseFailed;
        }

        private void OnDisable()
        {
            _purchaseManager.OnInitialized -= HandleInitialized;
            _purchaseManager.OnUpdateSuccess -= HandleUpdated;
            _purchaseManager.OnValidatePurchaseSuccess -= HandleValidatePurchaseSuccess;
            _purchaseManager.OnValidatePurchaseFailed -= HandleValidatePurchaseFailed;
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
                Debug.Assert(newProduct.hasReceipt);
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
                    // 이미 있으면 업데이트 하지 않는다.
                    // 예를 들어 PurchaseHistory가 로컬 세이브데이터에 의해서 복원된 경우에 해당한다.
                    //
                    // GPGS나 GameCenter 서비스를 이용하는 환경에서는
                    // 구매 히스토리 데이터를 로컬이 아닌 클라우드에서 전달받은 값으로 설정해야 한다. (product)
                    Log.Info($"PurchaseHistory 상품정보 업데이트 생략됨 ({product.id}, reason:이미 있음)");
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
            switch (product.type)
            {
                case EInAppProductType.NonConsumable:
                    return product.validated;
                
                case EInAppProductType.Consumable:
                    Log.Warning($"구매여부를 알아낼 수 없습니다 (소비형 상품, {product.id})");
                    break;
                
                case EInAppProductType.Subscription:
                    Log.Warning($"구매여부를 알아낼 수 없습니다 (구독형 상품, {product.id})");
                    break;
                
                default:
                    Log.Error($"구매여부를 알아낼 수 없습니다 (알 수 없는 상품 타입, {product.type})");
                    break;
            }
            
            return false;
        }

        public bool IsPurchased(string productId)
        {
            if (_products.TryGetValue(productId, out InAppProduct product))
            {
                return IsPurchased(product);
            }
            else
            {
                Log.Warning($"구매여부 확인 실패 ({productId}, 등재되지 않음)");
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