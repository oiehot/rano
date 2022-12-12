#nullable enable

using UnityEngine;

namespace Rano.Device
{
    public static class ScreenDevice
    {
        public enum EScreenDeviceType
        {
            Unknown = 0,
            Iphone8,
            IphoneX
        }
        
        public static EScreenDeviceType CurrentDeviceType
        {
            get
            {
                if (IsIphone8AspectRatio) return EScreenDeviceType.Iphone8;
                if (IsIphoneXAspectRatio) return EScreenDeviceType.IphoneX;
                return EScreenDeviceType.Unknown;
            }
        }
        
        public const float ASPECT_RATIO_16_9 = 16.0f / 9.0f;
        public const float ASPECT_RATIO_4_3 = 4.0f / 3.0f;
        
        public const int IPHONE8_WIDTH_PX = 750;
        public const int IPHONE8_HEIGHT_PX = 1334;
        public const float IPHONE8_ASPECT_RATIO = IPHONE8_WIDTH_PX / (float)IPHONE8_HEIGHT_PX;

        public const int IPHONEX_WIDTH_PX = 1125;
        public const int IPHONEX_HEIGHT_PX = 2436;
        public const float IPHONEX_ASPECT_RATIO = IPHONEX_WIDTH_PX / (float)IPHONEX_HEIGHT_PX;
        
        // public const int IPHONE14_PRO_WIDTH_PX = 1179;
        // public const int IPHONE14_PRO_HEIGHT_PX = 2556;
        // public const float IPHONE14_PRO_ASPECT_RATIO = IPHONE14_PRO_WIDTH_PX / (float)IPHONE14_PRO_HEIGHT_PX;
        
        public static int CurrentWidthPixel => Screen.width;
        public static int CurrentHeightPixel => Screen.height;
        public static float CurrentAspectRatio => CurrentWidthPixel / (float)CurrentHeightPixel;
        public static bool IsPortraitOrientation
        {
            get
            {
                ScreenOrientation orientation = Screen.orientation;
                if (orientation is ScreenOrientation.Portrait or ScreenOrientation.PortraitUpsideDown) return true;
                return false;
            }
        }

        public static bool IsIphone8AspectRatio => Mathf.Approximately(CurrentAspectRatio, IPHONE8_ASPECT_RATIO);
        public static bool IsIphoneXAspectRatio => Mathf.Approximately(CurrentAspectRatio, IPHONEX_ASPECT_RATIO);        
        // public static bool IsIphone14ProAspectRatio => Mathf.Approximately(CurrentAspectRatio, IPHONE14_PRO_ASPECT_RATIO); 
        
        public static void LogStatus()
        {
            // Log.Info($"IPHONE8_ASPECT_RATIO: {IPHONE8_ASPECT_RATIO}");
            // Log.Info($"IPHONEX_ASPECT_RATIO: {IPHONEX_ASPECT_RATIO}");
            // Log.Info($"IPHONE14_PRO_ASPECT_RATIO: {IPHONE14_PRO_ASPECT_RATIO}");
            
            Log.Info($"CurrentWidthPixel: {CurrentWidthPixel}");
            Log.Info($"CurrentHeightPixel: {CurrentHeightPixel}");
            Log.Info($"CurrentAspectRatio: {CurrentAspectRatio}");
            Log.Info($"CurrentDeviceType: {CurrentDeviceType}");

            Log.Info($"IsPortraitOrientation: {IsPortraitOrientation}");
            Log.Info($"IsIphone8AspectRatio: {IsIphone8AspectRatio}");
            Log.Info($"IsIphoneXAspectRatio: {IsIphoneXAspectRatio}");
            // Log.Info($"IsIphone14ProAspectRatio: {IsIphone14ProAspectRatio}");
        }
    }
}