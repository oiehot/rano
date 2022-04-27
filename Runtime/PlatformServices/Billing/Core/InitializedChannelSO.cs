#if false
using UnityEngine;
using UnityEngine.Events;

namespace Rano.PlatformServices.Billing
{
    [CreateAssetMenu(menuName = "Rano/Events/PurchaseManager/Initialized Channel")]
    public class InitializedChannelSO : DescriptionBaseSO
    {
        public UnityAction<InAppProduct[]> OnEventRaised;

        public void RaiseEvent(InAppProduct[] products )
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke(products);
        }
    }
}
#endif