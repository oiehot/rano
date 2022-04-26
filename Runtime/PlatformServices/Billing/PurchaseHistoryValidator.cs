using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Rano.SaveSystem;

namespace Rano.PlatformServices.Billing
{
    public class PurchaseHistoryValidator : MonoBehaviour
    {
        private PurchaseManager _purchaseManager;
        private PurchaseHistory _purchaseHistory;
        private IReceiptValidator _receiptValidator;
        
        private void Awake()
        {
            _purchaseManager = this.GetRequiredComponent<PurchaseManager>();
            _purchaseHistory = this.GetRequiredComponent<PurchaseHistory>();
            #if (!UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX))
                _receiptValidator = this.GetRequiredComponent<LocalReceiptValidator>();
            #else
                _receiptValidator = this.GetRequiredComponent<TestReceiptValidator>();
            #endif
        }

        [ContextMenu(nameof(Validate))]
        public void Validate()
        {
            ValidateAsync().ConfigureAwait(false);
        }
        
        public async Task ValidateAsync()
        {
            if (_purchaseManager.IsAvailable == false)
            {
                Log.Warning($"영수증 검증을 할 수 있는 상태가 아닙니다 (state: {_purchaseManager.State})");
                return;
            }
                
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
        }
    }
}