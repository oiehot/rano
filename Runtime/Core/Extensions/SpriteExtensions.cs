using System;
using UnityEngine;

namespace Rano
{
    public static class SpriteExtensions
    {
        /// <summary>
        /// 0.0~1.0 단위의 Custom Pivot 위치를 리턴한다.
        /// SpriteEditor에서 입력한 값이다.
        /// </summary>
        public static Vector2 GetCustomPivot(this Sprite sprite)
        {
            return new Vector2(
                sprite.pivot.x / sprite.rect.width,
                sprite.pivot.y / sprite.rect.height);
        }

        /// <summary>
        /// 월드스페이스 상의 CenterPivot 을 리턴한다.
        /// </summary>
        public static Vector3 GetPivotWorldSpace(this Sprite sprite)
        {
            var spriteWidth = sprite.bounds.size.x;
            var spriteHeight = sprite.bounds.size.y;
            var x = sprite.bounds.min.x;
            var y = sprite.bounds.min.y;
            Vector2 customPivot = sprite.GetCustomPivot();
            Vector3 pivotWorldSpace = new Vector3(
                x + (spriteWidth * customPivot.x),
                y + (spriteHeight * (customPivot.y)),
                sprite.bounds.center.z);
            return pivotWorldSpace;
        }
    }
}