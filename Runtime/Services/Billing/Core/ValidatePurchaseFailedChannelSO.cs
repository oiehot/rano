#if false
using UnityEngine;
using UnityEngine.Events;

namespace Rano.Services.Billing
{
    [CreateAssetMenu(menuName = "Rano/Events/PurchaseManager/ValidatePurchase Failed Channel")]
    public class ValidatePurchaseFailedChannelSO : DescriptionBaseSO
    {
        public UnityAction<string> OnEventRaised;

        public void RaiseEvent(string productId)
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke(productId);
        }
    }
}
#endif