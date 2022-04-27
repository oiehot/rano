using System.Threading.Tasks;
using UnityEngine;

namespace Rano.PlatformServices.Billing
{
    public sealed class TestReceiptValidator : MonoBehaviour, IReceiptValidator
    {   
        public async Task<ValidatePurchaseResult> ValidateAsync(string rawReceipt)
        {
            await Task.Yield();
            return new ValidatePurchaseResult(ValidatePurchaseResultType.SuccessTest, null);
        }
    }
}