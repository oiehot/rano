// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using NUnit.Framework;
using UnityEngine;

namespace Rano.Tests.Math
{
    public class ComparerTests
    {
        [Test]
        public void FloatComparerTest()
        {
            FloatComparer comparer = new FloatComparer();

            var values = new (float a, float b)[]
            {
                (0f, 0f),
                (0.1f, 0.1f),
                (0.25f, 0.25f),
                (0.5f, 0.5f),
                (0.75f, 0.75f),
                (1f, 1f),

                // Bankers Rounding
                (0.124f, 0.12f),
                (0.125f, 0.12f),
                (0.126f, 0.13f)
            };

            foreach (var value in values)
            {
                Assert.AreEqual(comparer.Equals(value.a, value.b), true,
                    message: $"{value.a}  {value.b} 가 동일하지 않음");

                Assert.AreEqual(comparer.GetHashCode(value.a), comparer.GetHashCode(value.b),
                    message: $"{value.a} 와 {value.b} 의 Hash코드가 같지 않음.");
            }
        }
    }
}