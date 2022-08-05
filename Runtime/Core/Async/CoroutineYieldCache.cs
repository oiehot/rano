using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rano
{
    /// <summary>
    /// 코루틴의 성능을 높이도록 자주 사용하는 Wait* 를 캐싱해두는 클래스.
    /// </summary>
    public static class CoroutineYieldCache
    {
        public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
        public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();


        /// <summary>
        /// float을 Key로 하는 WaitForSeconds 캐시용 사전.
        /// </summary>
        private static readonly Dictionary<float, WaitForSeconds> waitForSecondsCaches =
            new Dictionary<float, WaitForSeconds>(new Rano.Math.FloatComparer());

        /// <summary>
        /// WaitForSeconds의 최적 캐싱 수, 이 숫자를 넘어가는 WaitForSeconds 캐싱이 만들어지면 경고한다.
        /// </summary>
        private static readonly int bestWaitForSecondsCount = 100;

        /// <summary>
        /// 캐싱된 WaitForSeconds가 있으면 돌려주고 없으면 만들어 돌려준다.
        /// </summary>
        public static WaitForSeconds WaitForSeconds(float seconds)
        {
            UnityEngine.Debug.Assert(seconds >= 0f, $"음수 값으로 WaitForSeconds 객체를 얻고 있습니다: {seconds}");
            if (seconds == 0f) return null;

            WaitForSeconds wfs;
            if (waitForSecondsCaches.TryGetValue(seconds, out wfs) == false)
            {
                // 사전에 객체를 추가한다.
                waitForSecondsCaches.Add(seconds, wfs = new WaitForSeconds(seconds));

                // 캐싱 수가 지나치게 많아지면 경고한다.
                UnityEngine.Debug.Assert(waitForSecondsCaches.Count < bestWaitForSecondsCount,
                    $"WaitForSeconds의 캐싱 수가 너무 많습니다: {waitForSecondsCaches.Count}");
            }
            return wfs;
        }
    }
}
