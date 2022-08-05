using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Rano.Tests.Core
{
    public class YieldCacheTests
    {
        [UnityTest]
        public IEnumerator WaitForEndOfFrame_CacheTest()
        {
            Assert.IsNotNull(CoroutineYieldCache.WaitForEndOfFrame);
            yield return CoroutineYieldCache.WaitForEndOfFrame;
        }

        [UnityTest]
        public IEnumerator WaitForFixedUpdate_CacheTest()
        {
            Assert.IsNotNull(CoroutineYieldCache.WaitForFixedUpdate);
            yield return CoroutineYieldCache.WaitForFixedUpdate;
        }

        [UnityTest]
        public IEnumerator WaitForSeconds_CacheTest()
        {
            Assert.IsNull(
                CoroutineYieldCache.WaitForSeconds(0f),
                message: "WaitForSeconds(0)는 null을 리턴해야만 합니다."
            );

            Assert.AreEqual(
                CoroutineYieldCache.WaitForSeconds(0.25f),
                CoroutineYieldCache.WaitForSeconds(0.25f),
                message: $"두 개의 WaitForSeconds(0.25f)가 다릅니다. 캐싱되지 않고 있습니다."
            );

            Assert.AreEqual(
                CoroutineYieldCache.WaitForSeconds(0.12f),
                CoroutineYieldCache.WaitForSeconds(0.123f),
                message: $"두 개의 WaitForSeconds 0.12f, 0.123f 는 같아야 하는데 다릅니다. 캐싱되지 않고 있습니다."
            );

            Assert.AreEqual(
                CoroutineYieldCache.WaitForSeconds(0.13f),
                CoroutineYieldCache.WaitForSeconds(0.126f),
                message: $"두 개의 WaitForSeconds 0.13f, 0.126f 는 같아야 하는데 다릅니다. 캐싱되지 않고 있습니다."
            );

            yield return CoroutineYieldCache.WaitForSeconds(0f);
            yield return CoroutineYieldCache.WaitForSeconds(1f);
            yield return CoroutineYieldCache.WaitForSeconds(0.25f);
        }
    }
}