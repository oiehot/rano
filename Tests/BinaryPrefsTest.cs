using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    [Serializable]
    public struct TestData
    {
        public string name;
        public int score;
    }
    
    public class BinaryPrefsTest
    {
        [Test]
        public void A_SetTest()
        {
            var data = new TestData();
            data.name = "Taewoo Lee";
            data.score = 999;
            
            Rano.Data.BinaryPrefs<TestData>.Set("TestKey", data);
            // TODO: PlayerPrefs.Set("TestKey", Converter<TestData>.ToBase64() );
        }
        
        [Test]
        public void B_GetTest()
        {
            var data = Rano.Data.BinaryPrefs<TestData>.Get("TestKey");
            // TODO: data = Converter<TestData>.FromBase64( PlayerPrefs.Get("TestKey") );
            Assert.AreEqual("Taewoo Lee", data.name);
            Assert.AreEqual(999, data.score);
        }
        
        [Test]
        public void C_DeleteTest()
        {
            PlayerPrefs.DeleteKey("TestKey");
            Assert.IsFalse(PlayerPrefs.HasKey("TestKey"));
        }
    }
}