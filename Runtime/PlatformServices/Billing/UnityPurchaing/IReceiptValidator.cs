using System.Threading.Tasks;

namespace Rano.PlatformServices.Billing
{
    public interface IReceiptValidator
    {
        Task<ValidatePurchaseResult> ValidateAsync(string rawReceipt);
    }
}