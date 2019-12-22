// TODO: 풀고 작동되게 작업해야함
#if false
namespace Rano.RuntimeTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    
    [Serializable]
    public struct LocalFileTestData
    {
        public string name;
        public int score;
    }
    
    public class FileTests
    {
        [Test]
        public void A_LocalFileSave()
        {
            
            Rano.File.LocalSave.Save("test.txt", System.Text.Encoding.UTF8.GetBytes("Hello World!"));
        }
        
        [Test]
        public void B_LocalFileLoad()
        {
            byte[] bytes = Rano.File.LocalSave.Load("test.txt");
            Assert.AreEqual(System.Text.Encoding.UTF8.GetString(bytes), "Hello World!");
        }
        
        [Test]
        public void C_LocalJsonSave()
        {
            var data = new LocalFileTestData();
            data.name = "Taewoo Lee";
            data.score = 999;
            Rano.File.LocalSave.SaveToJson("test.json", data);
        }
        
        [Test]
        public void D_LocalJsonLoad()
        {
            var data = Rano.File.LocalSave.Load<LocalFileTestData>("test.json");
            Assert.AreEqual(data.name, "Taewoo Lee");
            Assert.AreEqual(data.score, 999);
        }
    }
}
#endif