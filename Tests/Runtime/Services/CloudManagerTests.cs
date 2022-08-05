using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Rano.Services.Cloud;

namespace Rano.Tests.PlatformServices
{
    public sealed class CloudManagerTests
    {
        private CloudManager _cloudManager;
        private Dictionary<string, string> _keys;
        private const string testString = "\tHello World! \r\n";

        public CloudManagerTests()
        {
            _keys = new Dictionary<string, string>();
            _keys["bool"] = "__bool";
            _keys["byteArray"] = "__byteArray";
            _keys["double"] = "__double";
            _keys["float"] = "__float";
            _keys["int"] = "__int";
            _keys["long"] = "__long";
            _keys["string"] = "__string";
            
            GameObject cloudManagerGO = new GameObject("CloudManager");
            _cloudManager = cloudManagerGO.AddComponent<CloudManager>();
        }

        ~CloudManagerTests()
        {
            foreach (string key in _keys.Keys)
            {
                _cloudManager.RemoveKey(key);
            }
        }

        [Test]
        public void SetBoolTest()
        {
            _cloudManager.SetBool(_keys["bool"], true);
            Assert.AreEqual(_cloudManager.GetBool(_keys["bool"]), true);
        }

        [Test]
        public void SetByteArrayTest()
        {
            byte[] testByte = { 0x00, 0x01, 0x10, 0x20, 0xfe, 0xff, 0x0d, 0x0a };
            _cloudManager.SetByteArray(_keys["byteArray"], testByte);
            Assert.AreEqual(_cloudManager.GetByteArray(_keys["byteArray"]), testByte);
        }

        [Test]
        public void SetDoubleTest()
        {
            _cloudManager.SetDouble(_keys["double"], (double)3.141592);
            Assert.AreEqual(_cloudManager.GetDouble(_keys["double"]), (double)3.141592);
        }

        [Test]
        public void SetFloatTest()
        {
            _cloudManager.SetFloat(_keys["float"], 3.14f);
            Assert.AreEqual(_cloudManager.GetFloat(_keys["float"]), 3, 14f);
        }

        [Test]
        public void SetIntTest()
        {
            _cloudManager.SetInt(_keys["int"], (int)1024);
            Assert.AreEqual(_cloudManager.GetInt(_keys["int"]), (int)1024);
        }

        [Test]
        public void SetLongTest()
        {
            _cloudManager.SetLong(_keys["long"], (long)10241024);
            Assert.AreEqual(_cloudManager.GetLong(_keys["long"]), (long)10241024);
        }

        [Test]
        public void SetStringTest()
        {
            _cloudManager.SetString(_keys["string"], testString);
            Assert.AreEqual(_cloudManager.GetString(_keys["string"]), testString);
        }

        [Test]
        public void RemoveKeyTest()
        {
            string tempKey = "__temporaryKey";
            _cloudManager.SetString(tempKey, testString);
            Assert.AreEqual(_cloudManager.GetString(tempKey), testString);
            _cloudManager.RemoveKey(tempKey);
            Assert.AreEqual(_cloudManager.GetString(tempKey), null);
        }
    }
}
