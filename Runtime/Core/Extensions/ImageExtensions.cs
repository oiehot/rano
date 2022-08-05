using UnityEngine;
using UnityEngine.UI;

namespace Rano
{
    public static class ImageExtensions
    {
        public static void SetAlpha(this Image image, float alpha)
        {
            var c = image.color;
            c.a = alpha;
            image.color = c;
        }

        public static float GetAlpha(this Image image)
        {
            return image.color.a;
        }
    }
}