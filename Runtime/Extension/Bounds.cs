using UnityEngine;

namespace Rano
{
    public static class BoundsExtensions
    {
        public static Vector3 GetRandomPoint(this Bounds bounds)
        {
            return bounds.center + new Vector3(
                (Random.value - 0.5f) * bounds.size.x,
                (Random.value - 0.5f) * bounds.size.y,
                (Random.value - 0.5f) * bounds.size.z
            );
        }
    }
}