using UnityEditor;
using UnityEngine;

namespace Rano.PlatformServices.Billing
{
    public enum InAppProductType
    {
        Consumable,
        NonConsumable
    }
    
    [CreateAssetMenu(fileName = "InAppProduct", menuName = "Rano/Platform Services/Billing/In App Product")]
    public class InAppProductSO : ScriptableObject
    {
        [Header("Ids")]
        public string id;
        public string iosId;
        public string tvosId;
        public string androidId;
        
        [Header("Product")]
        public string title;
        public string description;
        public InAppProductType productType;
    }
}