using UnityEngine;
using Rano;
using System;
using NUnit.Framework;

namespace Rano.Tests.DotNet
{
    public class DynamicLanguageRuntimeTests
    {
        public class Human
        {
            public string name;
            public int age;

            public Human(string name, int age)
            {
                this.name = name;
                this.age = age;
            }

            public string SetName(dynamic name)
            {
                this.name = (string)name;
                return "dynamic";
            }

            public string SetName(string name)
            {
                this.name = name;
                return "string";
            }
        }

        [Test]
        public void DynamicOverloading_Test()
        {
            Human h = new Human("Taewoo Lee", 38);
            
            Assert.AreEqual(h.SetName("Foo"), "string");
            Assert.AreEqual(h.SetName((object)"Bar"), "dynamic");
            Assert.AreEqual(h.SetName((dynamic)"Baz"), "string");
        }
    }
}