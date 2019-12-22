namespace Rano.RuntimeTests
{
    using System;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using LitJson;
    
    public class Person
    {
        public string name;
        public int age;
        public DateTime birthday;
    }
    
    public class LitJsonTest
    {
        [Test]
        // ! Object > String
        public void ObjectToJsonTest()
        {
            Person p = new Person();
            p.name = "Taewoo Lee";
            p.age = 37;
            p.birthday = new DateTime(1983, 10, 27);
            
            string json_p = JsonMapper.ToJson(p);
            
            Assert.AreEqual(json_p, "{\"name\":\"Taewoo Lee\",\"age\":37,\"birthday\":\"10/27/1983 00:00:00\"}");
        }
        
        [Test]
        // ! Json > Object
        public void JsonToObjectTest()
        {
            string json_p = "{\"name\":\"Taewoo Lee\",\"age\":37,\"birthday\":\"10/27/1983 00:00:00\"}";
            
            Person p = JsonMapper.ToObject<Person>(json_p);
            
            Assert.AreEqual(p.name, "Taewoo Lee");
            Assert.AreEqual(p.age, 37);
            Assert.AreEqual(p.birthday, new DateTime(1983, 10, 27));
        }
        
        [Test]
        // ! Json > Non-generaic
        public void JsonToGenericTest()
        {
            string text = @"
            {
                ""album"" : {
                    ""name""   : ""The Dark Side of the Moon"",
                    ""artist"" : ""Pink Floyd"",
                    ""year""   : 1973,
                    ""tracks"" : [
                        ""Speak To Me"",
                        ""Breathe"",
                        ""On The Run""
                    ]
                }
            }
            ";
            
            JsonData data = JsonMapper.ToObject(text);
            
            int albumYear = (int)data["album"]["year"];
            string albumName = (string)data["album"]["name"];
            string albumSecondTrack = (string)data["album"]["tracks"][1];
            
            Assert.AreEqual(albumName, "The Dark Side of the Moon");
            Assert.AreEqual(albumYear, 1973);
            Assert.AreEqual(albumSecondTrack, "Breathe");
        }
    }
}