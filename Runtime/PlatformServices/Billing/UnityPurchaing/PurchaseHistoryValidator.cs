using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Rano.PlatformServices.Billing
{
    public class PurchaseHistoryValidator : MonoBehaviour, IPurchaseHistoryValidator 
    {
        private IPurchaseHistory _purchaseHistory;
        private IReceiptValidator _receiptValidator;
        
        public Action OnValidated { get; set; }
        
        private void Awake()
        {
            _purchaseHistory = this.GetRequiredComponent<IPurchaseHistory>();
            #if (!UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX))
                _receiptValidator = this.GetRequiredComponent<LocalReceiptValidator>();
            #else
                _receiptValidator = this.GetRequiredComponent<TestReceiptValidator>();
            #endif
        }

        private void OnEnable()
        {
            _purchaseHistory.OnInitialized += HandleHistoryInitialized;
            _purchaseHistory.OnUpdated += HandleHistoryUpdated;
        }

        private void OnDisable()
        {
            _purchaseHistory.OnInitialized -= HandleHistoryInitialized;
            _purchaseHistory.OnUpdated -= HandleHistoryUpdated;
        }
        
        private void HandleHistoryInitialized()
        {
            Log.Info("PurchaseHistory가 초기화되었으나 따로 영수증을 검증하진 않습니다.");
        }

        private void HandleHistoryUpdated()
        {
            Log.Info("PurchaseHistory가 업데이트 되었으므로(초기화 아님) 영수증을 검증합니다.");
            ValidateAsync().ConfigureAwait(false);
        }
        
        public async Task ValidateAsync()
        {
            var products = _purchaseHistory.Products;
            Log.Info($"PurchaseHistory에 저장된 상품들의 영수증을 검증합니다... ({products.Count})");
            
            foreach (var kv in products)
            {
                InAppProduct product = kv.Value;
                if (product.hasReceipt == false)
                {
                    Log.Info($"영수증이 없으므로 검증상태를 False로 설정 ({product.id})");
                    _purchaseHistory.SetValidate(product.id, false);
                    continue;
                };
                
                var result = await _receiptValidator.ValidateAsync(product.receipt);
                switch (result.Type)
                {
                    case ValidatePurchaseResultType.Success:
                        foreach (var receipt in result.ValidateReceipts)
                        {
                            Log.Info($"영수증에서 검증됨 ({receipt.productID})");
                            _purchaseHistory.SetValidate(receipt.productID, true);
                        }
                        break;
                    
                    case ValidatePurchaseResultType.SuccessTest:
                        Log.Info($"테스트영수증 검증됨 ({product.id})");
                        _purchaseHistory.SetValidate(product.id, true);
                        break;
                    
                    case ValidatePurchaseResultType.Failed:
                        Log.Info($"영수증 검증에 실패하여 검증상태를 False로 설정 ({product.id})");
                        _purchaseHistory.SetValidate(product.id, false);
                        break;
                }
            }
            OnValidated?.Invoke();
        }
    }
}