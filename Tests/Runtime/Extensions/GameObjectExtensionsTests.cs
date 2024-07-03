using Rano;
using System;
using UnityEngine;
using NUnit.Framework;

namespace Rano.Tests.Extensions
{
    public class GameObjectExtensionsTests
    {
        private string _gameObjectName;

        [SetUp]
        public void Setup()
        {
            _gameObjectName = "__TestGameObject__";
        }

        [Test]
        public void GetPath_Test()
        {
            GameObject go;
            go = new GameObject(_gameObjectName);
            Assert.AreEqual(go.GetPath(), $"/{_gameObjectName}");
            UnityEngine.Object.Destroy(go);
        }

        [Test]
        public void GetRequiredComponent_Test()
        {
            GameObject go;
            go = new GameObject(_gameObjectName);

            Rigidbody a = go.AddComponent<Rigidbody>();
            Rigidbody b = go.GetRequiredComponent<Rigidbody>();
            Assert.AreEqual(a, b);
            try
            {
                BoxCollider c = go.GetRequiredComponent<BoxCollider>();
            }
            catch (MissingComponentException)
            {
                // Pass
                Assert.IsTrue(true);
            }
            UnityEngine.Object.Destroy(go);
        }

        [Test]
        public void GetOrAddComponent_Test()
        {
            GameObject go;
            go = new GameObject(_gameObjectName);
            Rigidbody a = go.GetOrAddComponent<Rigidbody>();
            Rigidbody b = go.GetOrAddComponent<Rigidbody>();
            Assert.AreEqual(a, b);
        }

        [Test]
        public void HasComponent_Test()
        {
            GameObject go;
            go = new GameObject(_gameObjectName);
            go.AddComponent<Rigidbody>();
            Assert.IsTrue(go.HasComponent<Transform>());
            Assert.IsTrue(go.HasComponent<Rigidbody>());
            Assert.IsFalse(go.HasComponent<BoxCollider>());
            UnityEngine.Object.Destroy(go);
        }
    }
}