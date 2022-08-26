using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Rano.Services.Billing
{
    public enum InAppProductType
    {
        Consumable,
        NonConsumable,
        Subscription
    }

    [CreateAssetMenu(fileName = "InAppProduct", menuName = "Rano/Services/Billing/InAppProduct")]
    public class InAppProductSO : ScriptableObject
    {
        [Header("Ids")]
        [SerializeField] [FormerlySerializedAs("id")] private string _id;
        [SerializeField] [FormerlySerializedAs("iosId")] private string _iosId;
        [SerializeField] [FormerlySerializedAs("androidId")] private string _androidId;
        
        [Header("Product")]
        [SerializeField] [FormerlySerializedAs("title")] private string _title;
        [SerializeField] [FormerlySerializedAs("description")] private string _description;
        [SerializeField] [FormerlySerializedAs("productType")] private InAppProductType type;

        public string Id => _id;
        public string IosId => _iosId;
        public string AndroidId => _androidId;
        public InAppProductType Type => type;
        public string Title => _title;
        public string Description => _description;
    }
}