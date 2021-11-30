// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; // JArray, JObject
using NUnit.Framework;
using Rano.File;

namespace Rano.Tests.File
{
    public class LocalSaveTests
    {
        #region CLASSES

        [JsonObject(MemberSerialization.OptIn)]
        public class Author
        {
            [JsonProperty] public string name;
            [JsonProperty] public string email;
            [JsonProperty] public string twitter;
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class Article
        {
            [JsonProperty] public int id;
            [JsonProperty] public string title;
            [JsonProperty] public Author author;
        }

        #endregion

        [Test]
        public void Byte_WriteRead_Test()
        {
            byte[] bytes = { 0x00, 0x01, 0xff };
            string filePath = "Byte_WriteRead_Test.txt";
            LocalSave.Write(filePath, bytes);

            byte[] result = LocalSave.Read(filePath);
            Assert.AreEqual(result[0], 0x00);
            Assert.AreEqual(result[1], 0x01);
            Assert.AreEqual(result[2], 0xff);
        }

        [Test]
        public void Json_SaveLoad_Test()
        {
            string filePath = "Json_SaveLoad_Test.json";

            Author author = new Author();
            author.name = "Taewoo Lee";
            author.email = "oiehot@gmail.com";
            author.twitter = "@oiehot";

            Article article = new Article();
            article.id = 100;
            article.title = "Hello World";
            article.author = author;

            LocalSave.SaveToJson(filePath, article);

            string loadedString = LocalSave.GetStringFromJson(filePath);
            Assert.IsNotNull(loadedString);

            Article loadedArticle = LocalSave.GetObjectFromJson<Article>(filePath);
            Assert.AreEqual(loadedArticle.id, 100);
            Assert.AreEqual(loadedArticle.author.name, "Taewoo Lee");

            dynamic loadedDynamic = LocalSave.GetDynamicFromJson(filePath);
            Assert.AreEqual((int)loadedDynamic.id, 100);
            Assert.AreEqual(loadedDynamic.author.name.ToString(), "Taewoo Lee");
        }
    }
}