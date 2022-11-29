#nullable enable

using System.Collections.Generic;
using UnityEngine;

namespace Rano.Ad
{
    /// <summary>
    /// 캔버스들의 SortingOrder를 일괄적으로 변경하는 컴포넌트.
    /// Admob 전면광고 출력시 캔버스 랜더링 순서문제를 해결하기 위해 만들었다.
    /// </summary>
    public class CanvasSorter : MonoBehaviour
    {
        private Dictionary<Canvas, int> _savedSortingOrders = new Dictionary<Canvas, int>();
        private IEnumerable<Canvas>? _canvases;

        void Start()
        {
            _canvases = GameObject.FindObjectsOfType<Canvas>(includeInactive: true);
        }

        /// <summary>
        /// 모든 캔버스들의 SortingOrder를 offset 만큼 이동시킨다.
        /// </summary>
        /// <param name="offset">이동시킬 값. 음수도 가능하다.</param>
        /// <remarks>Inspector에서 설정할 수 있는 sortingOrder 값의 범위 -32768 ~ 32767</remarks>
        public void MoveSortingOrder(int offset)
        {
            if (_canvases == null) return;
            // IEnumerable<Canvas> canvases = GetCanvases();
            
            Log.Info($"모든 캔버스들의 SortingOrder를 {offset} 만큼 이동");
            foreach (Canvas canvas in _canvases)
            {
                var beforeSortingOrder = canvas.sortingOrder;
                _savedSortingOrders[canvas] = beforeSortingOrder; // 옮기기 전 값을 기억한다.
                canvas.sortingOrder = beforeSortingOrder + offset;
            }
        }

        /// <summary>
        /// 수정된 캔버스 SortingOrder 값들을 복구한다.
        /// </summary>
        public void RestoreSortingOrder()
        {
            Log.Info("최근에 변경된 캔버스 SortingOrder 값들을 복구");
            foreach (KeyValuePair<Canvas, int> kv in _savedSortingOrders)
            {
                // Canvas 컴포넌트가 제거된 상태일 수도 있으므로 Null 체크한다.
                if (kv.Key != null)
                {
                    kv.Key.sortingOrder = kv.Value;    
                }
            }
            // 한 번 리셋하면 기억된 값들을 제거한다.
            _savedSortingOrders.Clear();
        }
    }
}