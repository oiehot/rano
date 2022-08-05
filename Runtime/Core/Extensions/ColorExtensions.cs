using UnityEngine;

namespace Rano
{
    public static class ColorExtensions
    {
        public static Color WithAlpha(this Color color, float alpha)
        {
            return color * new Color(1f, 1f, 1f, alpha);
        }
    }
}