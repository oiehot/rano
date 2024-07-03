using System.Threading;
using NUnit.Framework;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.TestTools;

namespace Rano.Tests.Async
{
    public partial class JobSystemTests
    {
        struct SimpleJob : IJob
        {
            public void Execute()
            {
                // Pass
            }
        }

        [Test]
        public void SimpleJobTest()
        {
            SimpleJob job;
            JobHandle handle = job.Schedule();
            handle.Complete();
        }
    }
}