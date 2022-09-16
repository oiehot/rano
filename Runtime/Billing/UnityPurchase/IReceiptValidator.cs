using System.Threading.Tasks;

namespace Rano.Billing.UnityPurchase
{
    public interface IReceiptValidator
    {
        Task<ValidatePurchaseResult> ValidateAsync(string rawReceipt);
    }
}