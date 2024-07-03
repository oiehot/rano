using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Rano.Tests.Async
{
    public class AsyncTests
    {
        async Task AsyncMethod()
        {
            await Task.Run(() =>
            {
                // Pass
            });
        }

        [Test]
        public async void AsyncAwaitTest()
        {
            await AsyncMethod();
        }

        [Test]
        public void AsyncGetResultTest()
        {
            Task<bool> task = Task.Run(() =>
            {
                return true;
            });

            while (!task.IsCompleted)
            {
                // Pass
            }

            Assert.IsTrue(task.Result);
        }
    }
}
