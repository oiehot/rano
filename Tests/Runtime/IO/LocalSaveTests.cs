// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using NUnit.Framework;
using Rano.IO;

namespace Rano.Tests.IO
{
    public class LocalSaveTests
    {
        [Test]
        public void Byte_WriteRead_Test()
        {
            byte[] bytes = { 0x00, 0x01, 0xff };
            string filePath = "Byte_WriteRead_Test.txt";
            LocalFile.WriteBytes(filePath, bytes);

            byte[] result = LocalFile.ReadBytes(filePath);
            Assert.AreEqual(result[0], 0x00);
            Assert.AreEqual(result[1], 0x01);
            Assert.AreEqual(result[2], 0xff);
        }
    }
}