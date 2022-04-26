using UnityEngine.Purchasing.Security;

namespace Rano.PlatformServices.Billing
{
    public enum ValidatePurchaseResultType
    {
        Failed,
        Success,
        SuccessTest
    }

    public struct ValidatePurchaseResult
    {
        private ValidatePurchaseResultType _type;
        private IPurchaseReceipt[] _validateReceipts;

        public ValidatePurchaseResultType Type => _type;
        public IPurchaseReceipt[] ValidateReceipts => _validateReceipts;

        public ValidatePurchaseResult(ValidatePurchaseResultType type, IPurchaseReceipt[] validateReceipts)
        {
            _type = type;
            _validateReceipts = validateReceipts;
        }
    }
}