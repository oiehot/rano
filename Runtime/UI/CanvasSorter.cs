// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rano
{
    /// <summary>
    /// 여러 캔버스들 SortingOrder를 일괄적으로 바꾸는데 사용하는 컴포넌트.
    /// Admob 전면광고 출력시 캔버스 랜더링 순서문제를 해결할 때 사용하면 좋다.
    /// </summary>
    public class CanvasSorter : MonoBehaviour
    {
        [SerializeField] private List<Canvas> _canvases = new List<Canvas>();
        [SerializeField] private Dictionary<Canvas, int> _originalSortingOrders = new Dictionary<Canvas, int>();

        /// <summary>
        /// 정렬 대상 캔버스들.
        /// </summary>
        public List<Canvas> canvases { get { return _canvases; } }

        /// <summary>
        /// 캔버스들의 기본 SortingOrder 값들.
        /// </summary>
        public Dictionary<Canvas, int> originalSortingOrders { get { return _originalSortingOrders; } }

        void Awake()
        {
            // TODO: 씬 안의 모든 Canvas에 대해서 수행할것.
            // 캔버스들의 SortingOrder 들을 기억해둔다.
            foreach (Canvas canvas in _canvases)
            {
                _originalSortingOrders[canvas] = canvas.sortingOrder;
            }
        }

        /// <summary>
        /// 모든 캔버스들의 SortingOrder를 offset 만큼 이동시킨다.
        /// </summary>
        /// <param name="offset">이동시킬 값. 음수도 가능하다.</param>
        public void MoveSortingOrder(int offset)
        {
            // TODO: offset 기본값 설정할것.
            Log.Info($"모든 캔버스들의 SortingOrder를 {offset} 만큼 이동.");
            foreach (Canvas canvas in _canvases)
            {
                canvas.sortingOrder += offset;
            }
        }

        /// <summary>
        /// 등록된 모든 캔버스들의 SortingOrder를 초기값으로 리셋한다.
        /// </summary>
        public void ResetSortingOrder()
        {
            Log.Info("모든 캔버스들의 SortingOrder를 기본값으로 복구.");
            foreach (Canvas canvas in _canvases)
            {
                if (_originalSortingOrders.ContainsKey(canvas) == false) continue;
                canvas.sortingOrder = _originalSortingOrders[canvas];
            }
        }
    }
}