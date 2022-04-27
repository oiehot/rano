#if false
using UnityEngine;
using UnityEngine.Events;

namespace Rano.PlatformServices.Billing
{
    [CreateAssetMenu(menuName = "Rano/Events/PurchaseManager/ValidatePurchase Success Channel")]
    public class ValidatePurchaseSuccessChannelSO : DescriptionBaseSO
    {
        public UnityAction<InAppProduct> OnEventRaised;

        public void RaiseEvent(InAppProduct product)
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke(product);
        }
    }
}
#endif