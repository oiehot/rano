// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using NUnit.Framework;

namespace Rano.Tests.Extensions
{
    public class NumberExtensionsTests
    {
        [Test]
        public void Int_ToCommaStringTest()
        {
            Assert.AreEqual(((int)1).ToCommaString(), "1");
            Assert.AreEqual(((int)12).ToCommaString(), "12");
            Assert.AreEqual(((int)123).ToCommaString(), "123");
            Assert.AreEqual(((int)1234).ToCommaString(), "1,234");
            Assert.AreEqual(((int)12345).ToCommaString(), "12,345");
            Assert.AreEqual(((int)123456).ToCommaString(), "123,456");
            Assert.AreEqual(((int)1234567).ToCommaString(), "1,234,567");
            Assert.AreEqual(((int)12345678).ToCommaString(), "12,345,678");
            Assert.AreEqual(((int)123456789).ToCommaString(), "123,456,789");
            Assert.AreEqual(((int)1234567890).ToCommaString(), "1,234,567,890");
        }

        [Test]
        public void Int_ToUnitStringTest()
        {
            Assert.AreEqual(((int)1).ToUnitString(), "1");
            Assert.AreEqual(((int)12).ToUnitString(), "12");
            Assert.AreEqual(((int)123).ToUnitString(), "123");
            Assert.AreEqual(((int)1200).ToUnitString(), "1.2K");
            Assert.AreEqual(((int)12300).ToUnitString(), "12.3K");
            Assert.AreEqual(((int)123400).ToUnitString(), "123.4K");
            Assert.AreEqual(((int)1230000).ToUnitString(), "1.23M");
            Assert.AreEqual(((int)12340000).ToUnitString(), "12.34M");
            Assert.AreEqual(((int)123450000).ToUnitString(), "123.45M");
            Assert.AreEqual(((int)1234000000).ToUnitString(), "1.234B");
        }

        [Test]
        public void Long_ToUnitStringTest()
        {
            Assert.AreEqual(((long)1).ToUnitString(), "1");
            Assert.AreEqual(((long)12).ToUnitString(), "12");
            Assert.AreEqual(((long)123).ToUnitString(), "123");
            Assert.AreEqual(((long)1200).ToUnitString(), "1.2K");
            Assert.AreEqual(((long)12300).ToUnitString(), "12.3K");
            Assert.AreEqual(((long)123400).ToUnitString(), "123.4K");
            Assert.AreEqual(((long)1230000).ToUnitString(), "1.23M");
            Assert.AreEqual(((long)12340000).ToUnitString(), "12.34M");
            Assert.AreEqual(((long)123450000).ToUnitString(), "123.45M");
            Assert.AreEqual(((long)1234000000).ToUnitString(), "1.234B");
            Assert.AreEqual(((long)12345000000).ToUnitString(), "12.345B");
            Assert.AreEqual(((long)123456000000).ToUnitString(), "123.456B");
            Assert.AreEqual(((long)1234000000000).ToUnitString(), "1.234T");
            Assert.AreEqual(((long)12345000000000).ToUnitString(), "12.345T");
            Assert.AreEqual(((long)123456000000000).ToUnitString(), "123.456T");
            // TODO: 1234.567T => 1,234.567 T
            Assert.AreEqual(((long)1234567000000000).ToUnitString(), "1234.567T");
        }
    }
}