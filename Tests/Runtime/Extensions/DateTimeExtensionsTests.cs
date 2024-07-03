using Rano;
using System;
using NUnit.Framework;

namespace Rano.Tests.Extensions
{
    public class DateTimeExtensionsTest
    {
        [Test]
        public void TimeSpan_ToHumanReadbleString_Test()
        {
            TimeSpan timeSpan;

            timeSpan = new TimeSpan(hours: 0, minutes: 0, seconds: 0);
            Assert.AreEqual(
                timeSpan.ToHumanReadableString(),
                ""
            );

            timeSpan = new TimeSpan(hours: 23, minutes: 59, seconds: 59);
            Assert.AreEqual(
                timeSpan.ToHumanReadableString(),
                "23 hours 59 minutes 59 seconds"
            );

            timeSpan = new TimeSpan(hours: 1, minutes: 1, seconds: 1);
            Assert.AreEqual(
                timeSpan.ToHumanReadableString(),
                "1 hour 1 minute 1 second"
            );


            timeSpan = new TimeSpan(hours: 24, minutes: 0, seconds: 0);
            Assert.AreEqual(
                timeSpan.ToHumanReadableString(),
                "1 day"
            );

            timeSpan = new TimeSpan(hours: 48, minutes: 0, seconds: 0);
            Assert.AreEqual(
                timeSpan.ToHumanReadableString(),
                "2 days"
            );
        }

        [Test]
        public void TimeSpan_ToReadbleString_Test()
        {
            TimeSpan timeSpan;

            timeSpan = new TimeSpan(hours: 0, minutes: 0, seconds: 0);
            Assert.AreEqual(
                timeSpan.ToReadableString(),
                "00:00:00"
            );

            timeSpan = new TimeSpan(hours: 23, minutes: 59, seconds: 59);
            Assert.AreEqual(
                timeSpan.ToReadableString(),
                "23:59:59"
            );

            timeSpan = new TimeSpan(hours: 24, minutes: 00, seconds: 00);
            Assert.AreEqual(
                timeSpan.ToReadableString(),
                "00:00:00"
            );
        }
    }
}