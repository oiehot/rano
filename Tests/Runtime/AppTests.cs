using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TestTools;  
using Rano.App;

namespace Rano.Tests
{   
    public class AppTests
    {
        [Test]
        public void VersionStruct_Test()
        {
            SVersion v = new SVersion("1.2.3");
            Assert.AreEqual(v.major, 1);
            Assert.AreEqual(v.minor, 2);
            Assert.AreEqual(v.build, 3);
            Assert.AreEqual(v.buildVersionCode, 12003);
            Assert.AreEqual(v.ToString(), "1.2.3");            
            Assert.AreEqual(v.fullVersion, "1.2.3 (12003)");
            Assert.AreEqual(v.GetHashCode(), 12003);

            Assert.IsTrue(v == new SVersion("1.2.3"));
            Assert.IsTrue(v != new SVersion("1.2.4"));

            Assert.IsTrue(v >= new SVersion("1.2.3"));
            Assert.IsTrue(v >= new SVersion("1.2.0"));
            Assert.IsTrue(v > new SVersion("1.2.0"));

            Assert.IsTrue(v <= new SVersion("1.2.3"));
            Assert.IsTrue(v <= new SVersion("1.2.5"));
            Assert.IsTrue(v < new SVersion("1.2.5"));

            SVersion v2 = new SVersion(1,2,3);
            Assert.AreEqual(v2.ToString(), "1.2.3");
        }
    }
}