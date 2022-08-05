using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Rano
{
    public static class CameraExtensions
    {
        /// <summary>
        /// 디바이스 스크린의 Width값을 리턴한다. (Pixel)
        /// </summary>
        public static int GetScreenWidthPixel(this Camera camera)
        {
            return Screen.width;
        }
        
        /// <summary>
        /// 디바이스 스크린의 Height값을 리턴한다. (Pixel)
        /// </summary>
        public static int GetScreenHeightPixel(this Camera camera)
        {
            return Screen.height;
        }
        
        /// <summary>
        /// 디바이스 스크린의 비례(Aspect)값을 리턴한다.
        /// </summary>
        public static float GetScreenAspect(this Camera camera)
        {
            return (float)Screen.width / (float)Screen.height;
        }

        /// <summary>
        /// Height값으로 OrthographicSize 값을 얻는다. (월드유닛)
        /// </summary>
        public static float CalcOrthographicSizeByHeight(this Camera camera, float height)
        {
            return height / 2;
        }

        /// <summary>
        /// Width값으로 OrthographicSize 값을 얻는다. (월드유닛)
        /// </summary>
        public static float CalcOrthographicSizeByWidth(this Camera camera, float width)
        {
            return width / (2 * camera.aspect);
        }

        public static float CalcOrthographicSizeByBounds(this Camera camera, Bounds bounds)
        {
            var width = bounds.size.x;
            var height = bounds.size.y;
            
            // 폭을 기준으로
            var maxWidth = height * camera.aspect;
            if (width > maxWidth)
            {
                // Width 기준으로 Fit
                return width / (2 * camera.aspect);
            }
            else
            {
                // Height 기준으로 Fit
                return height / 2;
            }
        }

        /// <summary>
        /// 카메라 뷰의 Width 값을 리턴한다. (월드유닛)
        /// </summary>
        public static float GetViewWidth(this Camera camera)
        {
            return camera.orthographicSize * 2 * camera.aspect;
        }

        /// <summary>
        /// 카메라 뷰의 Height값을 리턴한다. (월드유닛)
        /// </summary>
        public static float GetViewHeight(this Camera camera)
        {
            return camera.orthographicSize * 2;
        }

        /// <summary>
        /// 카메라 뷰의 Width 값을 설정한다. (월드유닛)
        /// </summary>
        public static void SetViewWidth(this Camera camera, float width)
        {
            camera.orthographicSize = width / (2 * camera.aspect);
        }
        
        /// <summary>
        /// 카메라 뷰의 Height 값을 설정한다. (월드유닛)
        /// </summary>
        public static void SetViewHeight(this Camera camera, float height)
        {
            camera.orthographicSize = height / 2;
        }

        /// <summary>
        /// 카메라 뷰의 비례(Aspect) 값을 리턴한다.
        /// </summary>
        public static float GetViewAspect(this Camera camera)
        {
            return camera.aspect;
        }
    }
}