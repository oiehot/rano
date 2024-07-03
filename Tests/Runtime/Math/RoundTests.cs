using System;
using NUnit.Framework;

namespace Rano.Tests.Math
{
    public class FloatTests
    {
        [Test]
        public void CeilingTests()
        {
            Assert.AreEqual(System.Math.Ceiling(0.9f), 1);
            Assert.AreEqual(System.Math.Ceiling(1.4f), 2);
            Assert.AreEqual(System.Math.Ceiling(1.5f), 2);
            Assert.AreEqual(System.Math.Ceiling(1.6f), 2);
        }

        [Test]
        public void TruncateTests()
        {
            Assert.AreEqual(System.Math.Truncate(1.4f), 1);
            Assert.AreEqual(System.Math.Truncate(1.5f), 1);
            Assert.AreEqual(System.Math.Truncate(1.6f), 1);
        }

        [Test]
        public void RoundTests()
        {
            Assert.AreEqual(System.Math.Round(0.124f, 2), 0.12);
            Assert.AreEqual(System.Math.Round(0.126f, 2), 0.13);

            Assert.AreEqual(System.Math.Round(0.125f, 2), 0.12d); // Default: ToEven (Banker's Rounding)
            Assert.AreEqual(System.Math.Round(0.135f, 2), 0.14d);

            Assert.AreEqual(System.Math.Round(0.125f, 2, System.MidpointRounding.ToEven), 0.12d); // ToEven: 가장 가까운 짝수로
            Assert.AreEqual(System.Math.Round(0.135f, 2, System.MidpointRounding.ToEven), 0.14d);

            Assert.AreEqual(System.Math.Round(0.125f, 2, System.MidpointRounding.AwayFromZero), 0.13d);
            Assert.AreEqual(System.Math.Round(0.135f, 2, System.MidpointRounding.AwayFromZero), 0.14d);
        }
    }
}