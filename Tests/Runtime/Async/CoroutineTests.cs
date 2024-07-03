using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Rano.Tests.Async
{
    public sealed class CoroutineTests
    {
        [UnityTest]
        public IEnumerator CourtineTest()
        {
            yield return null;
        }
    }
}
