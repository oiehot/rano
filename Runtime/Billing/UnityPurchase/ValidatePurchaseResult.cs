using UnityEngine.Purchasing.Security;

namespace Rano.Billing.UnityPurchase
{
    public enum EValidatePurchaseResultType
    {
        Failed,
        Success,
        SuccessTest
    }

    public struct ValidatePurchaseResult
    {
        private EValidatePurchaseResultType _type;
        private IPurchaseReceipt[] _validateReceipts;

        public EValidatePurchaseResultType Type => _type;
        public IPurchaseReceipt[] ValidateReceipts => _validateReceipts;

        public ValidatePurchaseResult(EValidatePurchaseResultType type, IPurchaseReceipt[] validateReceipts)
        {
            _type = type;
            _validateReceipts = validateReceipts;
        }
    }
}