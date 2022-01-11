// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

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
