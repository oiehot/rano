#nullable enable

using System.Collections.Generic;
using UnityEngine;

namespace Rano
{
    /// <summary>
    /// 코루틴의 성능을 높이도록 자주 사용하는 Wait* 를 캐싱해두는 클래스.
    /// </summary>
    public static class CoroutineYieldCache
    {
        /// <summary>
        /// WaitForSeconds의 최적 캐싱 수, 이 숫자를 넘어가는 WaitForSeconds 캐싱이 만들어지면 경고한다.
        /// </summary>
        private const int MAX_CACHE_COUNT = 100;

        private static WaitForEndOfFrame _waitForEndOfFrame;
        private static WaitForFixedUpdate _waitForFixedUpdate;
        private static Dictionary<float, WaitForSeconds> _waitForSecondsCaches;

        public static WaitForEndOfFrame WaitForEndOfFrame => _waitForEndOfFrame;
        public static WaitForFixedUpdate WaitForFixedUpdate => _waitForFixedUpdate;

        static CoroutineYieldCache()
        {
            _waitForEndOfFrame = new WaitForEndOfFrame();
            _waitForFixedUpdate = new WaitForFixedUpdate();
            _waitForSecondsCaches = new Dictionary<float, WaitForSeconds>(new Rano.Math.FloatComparer());
        }

        /// <summary>
        /// 캐싱된 WaitForSeconds가 있으면 돌려주고 없으면 만들어 돌려준다.
        /// </summary>
        public static WaitForSeconds? WaitForSeconds(float seconds)
        {
            Debug.Assert(seconds >= 0f, $"음수 값으로 WaitForSeconds 객체를 얻고 있습니다: {seconds}");
            if (seconds <= 0.0f) return null;

            WaitForSeconds wfs;
            if (_waitForSecondsCaches.TryGetValue(seconds, out wfs) == false)
            {
                // 사전에 객체를 추가한다.
                _waitForSecondsCaches.Add(seconds, wfs = new WaitForSeconds(seconds));

                // 캐싱 수가 지나치게 많아지면 경고한다.
                Debug.Assert(_waitForSecondsCaches.Count < MAX_CACHE_COUNT,
                    $"WaitForSeconds의 캐싱 수가 너무 많습니다: {_waitForSecondsCaches.Count}");
            }
            return wfs;
        }
    }
}
