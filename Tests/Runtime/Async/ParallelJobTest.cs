// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System.Collections;
using System.Threading;
using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.TestTools;

namespace Rano.Tests.Async
{
    public partial class JobSystemTests
    {
        struct SumJob : IJobParallelFor
        {
            public NativeArray<int> a;
            public NativeArray<int> b;
            public NativeArray<int> result;

            public void Execute(int index)
            {
                result[index] = a[index] + b[index];
            }
        }

        private void NativeArrayInit(NativeArray<int> a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = i;
            }
        }

        [UnityTest]
        public IEnumerator ParallelJobTest()
        {
            const int MAX = 10000;
            const Allocator ALLOCATOR = Allocator.Persistent;

            SumJob job;
            job.a = new NativeArray<int>(MAX, ALLOCATOR);
            job.b = new NativeArray<int>(MAX, ALLOCATOR);
            job.result = new NativeArray<int>(MAX, ALLOCATOR);

            NativeArrayInit(job.a);
            NativeArrayInit(job.b);

            JobHandle handle = job.Schedule(MAX, (int)(MAX / 100));
            while (!handle.IsCompleted)
            {
                yield return null;
            }
            handle.Complete();

            int total = 0;
            for (int i = 0; i < job.result.Length; i++)
            {
                total += job.result[i];
            }
            Assert.AreEqual(total, 99990000);

            job.a.Dispose();
            job.b.Dispose();
            job.result.Dispose();
        }
    }
}