using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using System.Reflection;
using System.Linq;

namespace Rano.Billing.UnityPurchase
{
    public sealed class LocalReceiptValidator : MonoBehaviour, IReceiptValidator
    {
        private CrossPlatformValidator _crossPlatformValidator;
        private byte[] _appleTangleData;
        private byte[] _appleStoreKitTestTangleData;
        private byte[] _googlePlayTangleData;

        private byte[] GetTangleData(Type type)
        {
            MethodInfo methodInfo;
            methodInfo = type.GetMethod("Data", BindingFlags.Public | BindingFlags.Static);
            object value = methodInfo.Invoke(null, null);
            return (byte[])value;
        }

        private Type GetType(string name)
        {
            Type t = AppDomain.CurrentDomain.GetAssemblies()
                // .Where(a => a.FullName == "MyFramework")
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name == name);
            return t;
        }
        
        private void Awake()
        {
            #if (!UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX))
                // 리플렉션을 사용해서 AppleTangle 등의 타입을 얻고
                // Data 프로퍼티로 값을 얻어내 UnityPurchaseService 생성자에 전달한다.
                // AppleTangle등의 클래스는 게임 네임스페이스에서 자동생성되기 때문에
                // 추가적인 연결없이 접근하기 위해서 이 방식을 사용했다.
                _appleTangleData = GetTangleData(GetType("AppleTangle"));;
                _appleStoreKitTestTangleData = GetTangleData(GetType("AppleStoreKitTestTangle"));;
                _googlePlayTangleData = GetTangleData(GetType("GooglePlayTangle"));;
                    
                #if !DEBUG_STOREKIT_TEST
                    _crossPlatformValidator = new CrossPlatformValidator(
                        _googlePlayTangleData,
                        _appleTangleData,
                        Application.identifier);
                #else
                    // When Apple Xcode12 StoreKit Test
                    _crossPlatformValidator = new CrossPlatformValidator(
                        _googlePlayTangleData,
                        _appleStoreKitTestTangleData,
                        Application.identifier);
                #endif
            #endif
        }

        public async Task<ValidatePurchaseResult> ValidateAsync(string rawReceipt)
        {
            await Task.Yield();
            
            IPurchaseReceipt[] validateReceipts;
            try
            {
                validateReceipts = _crossPlatformValidator.Validate(rawReceipt);
            }
            catch (IAPSecurityException)
            {
                return new ValidatePurchaseResult(EValidatePurchaseResultType.Failed, null);
            }
            return new ValidatePurchaseResult(EValidatePurchaseResultType.Success, validateReceipts);
        }
    }
}