using System;
using UnityEngine;

namespace Rano.Billing
{
    [CreateAssetMenu(fileName = "BillingServicesSettings", menuName = "Rano/Settings/BillingServices Settings")]
    public class BillingServicesSettingsSO : ScriptableObject
    {
        [Header("BillingService Settings")]
        
        [SerializeField] private string _androidPublicKey;
        public string AndroidPublicKey => _androidPublicKey;
        
        // [Header("UnityPurchasing Settings")]
        
#if USE_ESSENTIAL_KIT_BILLING_SERVICES
        [Header("EssentialKit BillingServices Settings")]
        [SerializeField] private bool _maintainPurchaseHistory = false;
        [SerializeField] private bool _autoFinishTransactions = true;
        [SerializeField] private bool _verifyTransactionReceipts = true;
        public bool MaintainPurchaseHistory => _maintainPurchaseHistory;
        public bool AutoFinishTransactions => _autoFinishTransactions;
        public bool VerifyTransactionReceipts => _verifyTransactionReceipts;
#endif
    }
}