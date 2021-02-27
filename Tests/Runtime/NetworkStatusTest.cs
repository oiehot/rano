using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Rano.RuntimeTests
{    
    public class NetworkStatusTest
    {
        [Test]
        public async void GetInternetAvailableAsync_Test()
        {
            bool internetAvailable = await Rano.NetworkStatus.GetInternetAvailableAsync();
            Assert.AreEqual(internetAvailable, true);
        }
    }
}