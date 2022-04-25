using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using System.Linq;
using Rano.SaveSystem;

namespace Rano.PlatformServices.Billing
{
    public sealed class PurchaseManager : MonoSingleton<PurchaseManager>, ISaveLoadable
    {
        #region Private Fields

        private UnityPurchaseService _purchaseService;
        private Dictionary<string, InAppProduct> _latestProducts = new();
        private DateTime _lastUpdateDateTime;
        
        [SerializeField] private InAppProductSO[] _rawProducts;
        [SerializeField] private bool _autoUpdate = true;
        [SerializeField] private float _autoUpdateInterval = 60f;
        
        #endregion

        #region Properties

        public PurchaseServiceState State => _purchaseService.State;

        public InAppProductSO[] RawProducts
        {
            get => _rawProducts;
            set => _rawProducts = value;
        }

        public Dictionary<string, InAppProduct> LatestProducts => _latestProducts;

        #endregion

        #region Event Properties

        public Action<string> OnPurchaseComplete { get; set; }
        public Action<string, string> OnPurchaseFailed { get; set; }
        public Action OnRestoreAllPurchasesComplete { get; set; }
        public Action OnRestoreAllPurchasesFailed { get; set; }
        public Action<string> OnProductRefunded { get; set; }
        public Action<string> OnProductRemoved { get; set; }

        #endregion

        #region Unity Events

        protected override void Awake()
        {
            base.Awake();

#if (UNITY_EDITOR)
            _purchaseService = new UnityPurchaseService(this);
#elif (!UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX))
            // 리플렉션을 사용해서 AppleTangle 등의 타입을 얻고
            // Data 프로퍼티로 값을 얻어내 UnityPurchaseService 생성자에 전달한다.
            // AppleTangle등의 클래스는 게임 네임스페이스에서 자동생성되기 때문에
            // 추가적인 연결없이 접근하기 위해서 이 방식을 사용했다.
            byte[] GetTangleData(Type type)
            {
                MethodInfo methodInfo;
                methodInfo = type.GetMethod("Data", BindingFlags.Public | BindingFlags.Static);
                object value = methodInfo.Invoke(null, null);
                return (byte[])value;
            }

            Type GetType(string name)
            {
                Type t = AppDomain.CurrentDomain.GetAssemblies()
                    // .Where(a => a.FullName == "MyFramework")
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.Name == name);
                return t;
            }

            byte[] appleTangleData = GetTangleData(GetType("AppleTangle"));
            byte[] appleStoreKitTestTangleData = GetTangleData(GetType("AppleStoreKitTestTangle"));
            byte[] googlePlayTangleData = GetTangleData(GetType("GooglePlayTangle"));
            _purchaseService = new UnityPurchaseService(
                this,
                appleTangleData,
                appleStoreKitTestTangleData,
                googlePlayTangleData
            );
#endif
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(nameof(UpdateCoroutine));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StopCoroutine(nameof(UpdateCoroutine));
            // TODO: Save
        }

        #endregion
        
        #region Coroutine Methods

        private IEnumerator UpdateCoroutine()
        {
            while (true)
            {
                yield return YieldCache.WaitForSeconds(_autoUpdateInterval);
                if (_autoUpdate) UpdateStatus();
            }
        }

        #endregion

        #region Action Methods

        public void Initialize()
        {
            Log.Info("구매서비스 초기화 시작.");
            _purchaseService.Initialize();
        }
        
        public void UpdateStatus()
        {
            Log.Info("구매서비스 상태 업데이트 요청.");
            _purchaseService.UpdateStatus();
        }

        public void Purchase(string productId)
        {
            Log.Info($"구매요청 ({productId})");
            _purchaseService.Purchase(productId);
        }

        public void RestoreAllPurchases()
        {
            Log.Info($"구매복구 요청");
            _purchaseService.RestoreAllPurchases();
        }
        
        public bool IsPurchased(string productId)
        {
            if (LatestProducts.TryGetValue(productId, out var product))
            {
                if (product.purchaseState == PurchaseState.Purchased) return true;
            }
            return false;
        }
        
        public async void SetProductsAsync(Dictionary<string, InAppProduct> products)
        {
            // 상품들의 영수증들을 검증하고 구매상태를 갱신한다.
            await ValidatePurchaseStatesAsync(products);
            
            // 전,후를 비교하여 환불 혹은 제거 이벤트를 콜백한다.
            CheckProductsDifference(_latestProducts, products);
            
            // 캐시 업데이트. 
            _latestProducts = products;

            _lastUpdateDateTime = DateTime.Now;
        }

        public void SetProduct(InAppProduct product)
        {
            _latestProducts[product.id] = product;
            _lastUpdateDateTime = DateTime.Now;
        }
        
        public async Task ValidatePurchaseStatesAsync(Dictionary<string, InAppProduct> products)
        {
            foreach (var kv in products)
            {
                string productId = kv.Key;
                InAppProduct product = kv.Value;

                if ( !(product.enabled && product.availableToPurchase) )
                {
                    throw new NotImplementedException();
                }
                
                switch (product.type)
                {
                    case InAppProductType.Consumable:
                        product.purchaseState = PurchaseState.Unknown;
                        break;
                    
                    case InAppProductType.NonConsumable:
                        if (product.hasReceipt &&
                            _purchaseService.LocalValidateReceipt(product.receipt))
                        {
                            product.purchaseState = PurchaseState.Purchased;
                        }
                        else
                        {
                            product.purchaseState = PurchaseState.NotPurchased;
                        }
                        break;

                    case InAppProductType.Subscription:
                        throw new NotImplementedException();
                        break;
                    
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public void CheckProductsDifference(Dictionary<string, InAppProduct> before, Dictionary<string, InAppProduct> after)
        {
            // 이전에는 구매했었으나 지금은 구매되어있지 않은 상품을 발견하면
            // OnPurchaseRemoved 콜백을 호출한다.
            foreach (var kv in after)
            {
                string productId = kv.Key;
                InAppProduct afterProduct = kv.Value;
                if (before.TryGetValue(productId, out var beforeProduct))
                {
                    if (beforeProduct.IsPurchased && !afterProduct.IsPurchased)
                    {
                        UnityEngine.Debug.Assert(beforeProduct.hasReceipt && !afterProduct.hasReceipt);
                        Log.Info($"구매하신 상품이 환불처리된것을 확인했습니다 ({productId})");
                        OnProductRefunded?.Invoke(productId);
                    }
                }
            }

            // 이전에는 존재했었고 구입했었으나, 현재는 사라진 혹은 구매되지 않은 상품을 발견하면
            // OnPurchaseRemoved 콜백을 호출한다.
            foreach (var kv in before)
            {
                string productId = kv.Key;
                InAppProduct beforeProduct = kv.Value;
                bool afterContains = after.ContainsKey(productId);
                
                // 이전에 구매했었으나, 지금은 사라진 아이템일 때 => 상품 제거됨
                if (beforeProduct.IsPurchased && !afterContains)
                {
                    Log.Info($"구매하신 상품은 더이상 이용할 수 없는 상품입니다 ({productId})");
                    OnProductRefunded?.Invoke(productId);
                }
                
                // 이전에 구매하지 않았으나, 지금은 사라진 아이템일 때 => 상품 제거됨
                if (!beforeProduct.IsPurchased && !afterContains)
                {
                    Log.Info($"상품을 더이상 이용할 수 없습니다 ({productId})");
                    OnProductRemoved?.Invoke(productId);
                }
            }
        }

        #endregion

        #region ISaveLoadable Methods

        public void ClearState()
        {
            _latestProducts.Clear();
            _lastUpdateDateTime = DateTime.MinValue;
        }

        public void DefaultState()
        {
            _latestProducts.Clear();
            _lastUpdateDateTime = DateTime.Now;
        }

        public object CaptureState()
        {
            var state = new PurchaseManagerSaveData();
            state.products = _latestProducts; 
            state.lastUpdateDateTime = _lastUpdateDateTime;
            return state;
        }
        
        public void ValidateState(object state)
        {
            // pass
        }
        public void RestoreState(object state)
        {
            var data = (PurchaseManagerSaveData)state;
            _latestProducts = data.products;
            _lastUpdateDateTime = data.lastUpdateDateTime;
        }
        
        #endregion

        #region Log Methods

        public void LogStatus()
        {
            Log.Info("PurchaseManager Status:");
            Log.Info($"  state: {State}");
            Log.Info($"  autoUpdate: {_autoUpdate}");
            Log.Info($"  autoUpdateInterval: {_autoUpdateInterval}");
            Log.Info($"  rawProducts: {_rawProducts.Length}");
            Log.Info($"  latestProducts: {_latestProducts.Count}");
            Log.Info($"  lastUpdatedDateTime: {_lastUpdateDateTime}");
        }

        public void LogProduct(string productId)
        {
            InAppProduct product;
            if (LatestProducts.TryGetValue(productId, out product))
            {
                product.LogStatus();
            }
            else
            {
                Log.Warning($"목록에 없는 상품의 정보를 얻을 수는 없습니다 ({productId})");
            }
        }

        public void LogProductList()
        {
            Log.Info("Products:");
            foreach (var kv in Instance.LatestProducts)
            {
                var productId = kv.Key;
                var product = kv.Value;
                Log.Info($"* {productId} ({product.purchaseState})");
            }
        }

        #endregion

    }
}