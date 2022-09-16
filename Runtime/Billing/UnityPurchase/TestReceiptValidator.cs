using System.Threading.Tasks;
using UnityEngine;

namespace Rano.Billing.UnityPurchase
{
    public sealed class TestReceiptValidator : MonoBehaviour, IReceiptValidator
    {   
        public async Task<ValidatePurchaseResult> ValidateAsync(string rawReceipt)
        {
            await Task.Yield();
            return new ValidatePurchaseResult(EValidatePurchaseResultType.SuccessTest, null);
        }
    }
}