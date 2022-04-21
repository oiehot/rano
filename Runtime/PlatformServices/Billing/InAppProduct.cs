using System;
using VoxelBusters.EssentialKit;

namespace Rano.PlatformServices.Billing
{
    public class InAppProduct : IBillingProduct
#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
        ,ICloneable
#endif
    {
        private string _id;
        private string _platformId;
        private string _localizedTitle;
        private string _localizedDescription;
        private string _price;
        private string _localizedPrice;
        private string _priceCurrencyCode;
        private object _tag;
        public bool _purchased;

        public string Id => _id;
        public string PlatformId => _platformId;
        public string LocalizedTitle => _localizedTitle;
        public string LocalizedDescription => _localizedDescription;
        public string Price => _price;
        public string LocalizedPrice => _localizedPrice;
        public string PriceCurrencyCode => _priceCurrencyCode;
        public object Tag => _tag;

        public bool IsPurchased => _purchased;
        
        public InAppProduct(
            string id,
            string platformId,
            string localizedTitle,
            string localizedDescription,
            string localizedPrice,
            string price,            
            string priceCurrencyCode,
            bool purchased,
            object tag
            )
        {
            _id = id;
            _platformId = platformId;
            _localizedTitle = localizedTitle;
            _localizedDescription = localizedDescription;
            _price = price;
            _localizedPrice = localizedPrice;
            _priceCurrencyCode = priceCurrencyCode;
            _purchased = purchased;
            _tag = tag;
        }
        public void SetPurchaseFlag(bool value)
        {
            _purchased = value;
        }

        public override string ToString()
        {
            return $"{_id}";
        }
        
#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
        public object Clone()
        {
            return new InAppProduct(_id, _platformId, _localizedTitle,
                _localizedDescription, _localizedPrice, _price,
                _priceCurrencyCode, _purchased, _tag);
        }
#endif
    }
}