#nullable enable

using System;
using UnityEngine;

namespace Rano.Billing
{
    public enum EInAppProductType
    {
        Consumable,
        NonConsumable,
        Subscription
    }

    [CreateAssetMenu(fileName = "InAppProduct", menuName = "Rano/Billing/InAppProduct")]
    public class InAppProductSO : ScriptableObject
    {
        #nullable disable
        [Header("Ids")]
        [SerializeField] private string _id;
        [SerializeField] private string _iosId;
        [SerializeField] private string _androidId;
        
        [Header("Product")]
        [SerializeField] private string _title;
        [SerializeField] private string _description;
        [SerializeField] private EInAppProductType _type;
        #nullable enable

        public string ID => _id;
        public string IosID => _iosId;
        public string AndroidID => _androidId;
        public EInAppProductType Type => _type;
        public string Title => _title;
        public string Description => _description;

        void OnValidate()
        {
            Validate();
        }
        
        public bool Validate()
        {
            int warningCount = 0;
            
            if (String.IsNullOrEmpty(_id))
            {
                Log.Warning("ID가 비어 있음");
                ++warningCount;
            }

            #if UNITY_ANDROID
            if (String.IsNullOrEmpty(_androidId))
            {
                Log.Warning("Android ID가 비어 있음");
                ++warningCount;
            }
            #endif
            
            #if UNITY_IOS
            if (String.IsNullOrEmpty(_iosId))
            {
                Log.Warning("iOS ID가 비어 있음");
                ++warningCount;
            }
            #endif

            if (warningCount > 0) return false;
            return true;
        }
    }
}