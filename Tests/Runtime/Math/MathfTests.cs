using NUnit.Framework;
using UnityEngine;

namespace Rano.Tests.Math
{
    public class MathfTest
    {
        [Test]
        public void ApproximatelyTest()
        {
            Assert.IsTrue(Mathf.Approximately(0.0001f, 0.0001f));
            Assert.IsTrue(Mathf.Approximately(0.001f, 0.001f));
            Assert.IsTrue(Mathf.Approximately(0.01f, 0.01f));
            Assert.IsTrue(Mathf.Approximately(0.1f, 0.1f));
            Assert.IsTrue(Mathf.Approximately(0.0f, 0.0f));
            Assert.IsTrue(Mathf.Approximately(0.5f, 0.5f));
            Assert.IsTrue(Mathf.Approximately(1.0f, 1.0f));
            Assert.IsFalse(Mathf.Approximately(0.0f, 0.0001f));
        }
        
        [Test]
        public void LerfTest()
        {
            Assert.IsTrue(Mathf.Approximately(
                Mathf.Lerp(0f, 100f, 0.5f),
                50.0f
            ));
            
        }
     
        [Test]
        public void InverseLerfTest()
        {
            Assert.IsTrue(Mathf.Approximately(
                Mathf.InverseLerp(0f, 100f, 50f),
                0.5f
            ));
            Assert.IsTrue(Mathf.Approximately(
                Mathf.InverseLerp(0f, 100f, -10f),
                0f
            ));
            Assert.IsTrue(Mathf.Approximately(
                Mathf.InverseLerp(0f, 100f, 1000f),
                1f
            ));
        }
    }
}