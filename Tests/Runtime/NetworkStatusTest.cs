namespace Rano.RuntimeTests
{
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
     
    public class NetworkStatusTest
    {
        [Test]
        public async void GetInternetAvailableAsync_Test()
        {
            bool internetAvailable = await Rano.Network.NetworkStatus.GetInternetAvailableAsync();
            Assert.AreEqual(internetAvailable, true);
        }
    }
}