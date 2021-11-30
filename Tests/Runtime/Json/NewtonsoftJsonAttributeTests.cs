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

namespace Rano.Tests.Json
{
    public class NewtonsoftJsonAttributeTests
    {
        #region CLASSES

        public enum Gender
        {
            Male,
            Female
        }

        public enum Country
        {
            SouthKorea,
            NorthKorea,
            USA
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class Address
        {
            [JsonProperty] public Country country;
            public string line1;
            public string line2;
            public int zipCode;
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class Author
        {
            public string name;
            public string email;
            public string twitter;
            public Gender gender;
            public DateTime birthday;
            [JsonProperty] public Address address;
            [JsonIgnore] public int ignoreField;

            public int _publicKey;
            public int _propertyKey { get; private set; }

            [JsonProperty("private_key", Order = -1)] private int _privateKey;
            private int _subPrivateKey;

            public int GetPrivateKey() { return _privateKey; }
            public void SetPrivateKey(int key) { _privateKey = key; }
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class Article
        {
            public int id;
            public string title;
            [JsonProperty] public Author author;
        }

        #endregion

        #region FIELDS

        // Pass

        #endregion

        [SetUp]
        public void Setup()
        {
            // Pass
        }

        [Test]
        public void JsonConvert_SerializeObject_WithAttribute_Test()
        {
            Address address = new Address();
            address.country = Country.SouthKorea;
            address.line1 = "108-2012";
            address.line2 = "Yangji-ro";
            address.zipCode = 123456;

            Author author = new Author();
            author.name = "Taewoo Lee";
            author.gender = Gender.Male;
            author.email = "oiehot@gmail.com";
            author.twitter = "@oiehot";
            author.birthday = new DateTime(1983, 10, 27);
            author.address = address;
            author.SetPrivateKey(1234);

            Article article = new Article();
            article.id = 100;
            article.title = "Hello World";
            article.author = author;

            string result = JsonConvert.SerializeObject(article);

            Article deArticle = JsonConvert.DeserializeObject<Article>(result);
            Assert.AreEqual(deArticle.author.address.country, Country.SouthKorea);
            Assert.AreEqual(deArticle.author.GetPrivateKey(), 1234);
        }
    }
}