using System;
using UnityEngine;

namespace Rano.PlatformServices.Billing
{
    [CreateAssetMenu(fileName = "BillingServicesSettings", menuName = "Rano/Settings/BillingServices Settings")]
    public class BillingServicesSettingsSO : ScriptableObject
    {
        [Header("Billing Services")]
        
        /// <summary>
        /// Enabling this stores the non-consumable purchases on device locally 
        /// </summary>
        [SerializeField] private bool _maintainPurchaseHistory = false;

        /// <summary>
        /// Automatically close the transaction once the purchase is done.
        /// Disabling this requires you to manually call FinishTransactions
        /// </summary>
        [SerializeField] private bool _autoFinishTransactions = true;

        /// <summary>
        /// Enabling this allows the plugin to do an internal verification once
        /// after the purchase for additional security.
        /// Recommended to set it to true.
        /// </summary>
        [SerializeField] private bool _verifyTransactionReceipts = true;
        
        /// <summary>
        /// Public key needs to be set in the Essential Kit Settings and can be obtained
        /// from Monetisation Setup of Monetise section.
        /// Public key is used internally by the plugin to validate a purchased receipt on Android
        /// </summary>
        [SerializeField] private string _androidPublicKey;
        
        public bool MaintainPurchaseHistory => _maintainPurchaseHistory;
        public bool AutoFinishTransactions => _autoFinishTransactions;
        public bool VerifyTransactionReceipts => _verifyTransactionReceipts;
        public string AndroidPublicKey => _androidPublicKey;
    }
}