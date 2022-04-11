using System;
using UnityEngine;

namespace Rano.PlatformServices.Admob
{
    public enum AdVendor
    {
        None,
        Admob
    }
    public enum AdType
    {
        None,
        RewardedAd
    }
    
    [CreateAssetMenu(fileName = "Ad", menuName = "Rano/Platform Services/Ad/Ad")]
    public class AdSO : ScriptableObject
    {
        public AdVendor vendor;
        public AdType type;
        public string adName;
        public string iosUnitId;
        public string androidUnitId;
    }
}