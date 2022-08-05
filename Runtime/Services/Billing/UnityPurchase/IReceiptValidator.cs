using System.Threading.Tasks;

namespace Rano.Services.Billing.UnityPurchase
{
    public interface IReceiptValidator
    {
        Task<ValidatePurchaseResult> ValidateAsync(string rawReceipt);
    }
}