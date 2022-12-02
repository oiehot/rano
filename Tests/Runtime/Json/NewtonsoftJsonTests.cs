using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; // JArray, JObject
using NUnit.Framework;

namespace Rano.Tests.Json
{
    public class NewtonsoftJsonTests
    {
        [System.Serializable]
        public class Author
        {
            public string name;
            public string email;
            public string twitter;

            public Author(string name, string email, string twitter)
            {
                this.name = name;
                this.email = email;
                this.twitter = twitter;
            }
        }

        [System.Serializable]
        public class Article
        {
            public int id;
            public string title;
            public Author author;

            public Article(int id, string title, Author author)
            {
                this.id = id;
                this.title = title;
                this.author = author;
            }
        }
        private string _jsonArticles;
        private string _jsonArticle;
        
        [SetUp]
        public void Setup()
        {
            _jsonArticles = @"[
              {
                'id': 100,
                'title': 'Hello World',
                'author': {
                  'name': 'Taewoo Lee',
                  'email': 'oiehot@gmail.com',
                  'twitter': '@oiehot'
                }
              }
            ]";

            _jsonArticle = @"{
              'id': 100,
              'title': 'Hello World',
              'author': {
                'name': 'Taewoo Lee',
                'email': 'oiehot@gmail.com',
                'twitter': '@oiehot'
              }
            }";
        }

        [Test]
        public void JToken_Cast_Test()
        {
            dynamic article = JObject.Parse(_jsonArticle);
            JToken nameToken = article.author.name;

            Assert.IsNotNull(nameToken);
            Assert.AreEqual(nameToken.ToString(), "Taewoo Lee");
        }

        [Test]
        public void JObject_Add_Test()
        {
            JObject jObject = new JObject();
            
            Assert.IsNotNull(jObject);
            
            jObject.Add("int_key", 100);
            jObject.Add("float_key", 3.141592f);
            jObject.Add("string_key", "foo");
            jObject.Add("boolean_key", true);

            Assert.AreEqual((int)jObject["int_key"], 100);
            Assert.AreEqual((float)jObject["float_key"], 3.141592f);
            Assert.IsNotNull(jObject["string_key"]);
            Assert.AreEqual(jObject["string_key"].ToString(), "foo");
            Assert.AreEqual((bool)jObject["boolean_key"], true);
        }

        [Test]
        public void JObject_ValueQuery_Test()
        {
            JObject jObject = new JObject();
            Assert.IsNotNull(jObject);
            
            jObject.Add("int_key", 100);
            jObject.Add("float_key", 3.141592f);
            jObject.Add("string_key", "foo");
            jObject.Add("boolean_key", true);

            Assert.AreEqual(jObject.Value<int>("int_key"), 100);
            Assert.AreEqual(jObject.Value<float>("float_key"), 3.141592f);
            Assert.AreEqual(jObject.Value<string>("string_key"), "foo");
            Assert.AreEqual(jObject.Value<bool>("boolean_key"), true);
        }

        [Test]
        public void JObject_Remove_Test()
        {
            const string key = "foo";
            const string value = "bar";
            
            JObject jObject = new JObject();
            Assert.IsNotNull(jObject);
            
            jObject.Add(key, value);
            jObject.Remove(key);
            
            Assert.IsFalse(jObject.ContainsKey(key));
        }

        [Test]
        public void JObject_RemoveAll_Test()
        {
            JObject jObject = new JObject();
            Assert.IsNotNull(jObject);
            
            jObject.Add("foo", "0");
            jObject.Add("bar", "1");
            jObject.Add("baz", "2");
            jObject.RemoveAll();
        }

        [Test]
        public void JObject_ContainsKey_Test()
        {
            JObject jObject = new JObject();
            Assert.IsNotNull(jObject);
            
            jObject.Add("foo", "bar");
            Assert.IsTrue(jObject.ContainsKey("foo"));
            Assert.IsFalse(jObject.ContainsKey("baz"));
        }

        [Test]
        public void JObject_Parse_Test()
        {
            dynamic article = JObject.Parse(_jsonArticle);
            Assert.IsNotNull(article);

            Assert.IsNotNull(article.ToString());

            Assert.AreEqual((int)article.id, 100);
            Assert.AreEqual((int)article["id"], 100);

            Assert.AreEqual(article["title"].ToString(), "Hello World");
            Assert.AreEqual(article.title.ToString(), "Hello World");

            Assert.AreEqual(article["author"]["name"].ToString(), "Taewoo Lee");
            Assert.AreEqual(article.author.name.ToString(), "Taewoo Lee");

            Assert.AreEqual(article["author"]["email"].ToString(), "oiehot@gmail.com");
            Assert.AreEqual(article.author.email.ToString(), "oiehot@gmail.com");

            Assert.AreEqual(article["author"]["twitter"].ToString(), "@oiehot");
            Assert.AreEqual(article.author.twitter.ToString(), "@oiehot");
        }

        [Test]
        public void JObject_FromObject_Test()
        {
            Author author = new Author("Taewoo Lee", "oiehot@gmail.com", "@oiehot");
            Article article = new Article(100, "Foo Bar", author);
            
            JObject jObject = JObject.FromObject(article);
            Assert.IsNotNull(jObject);
            
            Assert.AreEqual((int)jObject["id"], 100);
            Assert.IsNotNull(jObject["title"]);
            Assert.AreEqual(jObject["title"].ToString(), "Foo Bar");
            Assert.IsNotNull(jObject["author"]);
            Assert.IsNotNull(jObject["author"]["name"]);
            Assert.IsNotNull(jObject["author"]["email"]);
            Assert.IsNotNull(jObject["author"]["twitter"]);
            Assert.AreEqual(jObject["author"]["name"].ToString(), "Taewoo Lee");
            Assert.AreEqual(jObject["author"]["email"].ToString(), "oiehot@gmail.com");
            Assert.AreEqual(jObject["author"]["twitter"].ToString(), "@oiehot");
        }

        [Test]
        public void JArray_Parse_Test()
        {
            dynamic articles = JArray.Parse(_jsonArticles);
            dynamic article = articles[0];

            Assert.AreEqual((int)article.id, 100);
            Assert.AreEqual(article.id.ToString(), "100");
            Assert.AreEqual(article.title.ToString(), "Hello World");
            JToken nameToken = article.author.name;
            Assert.AreEqual(nameToken.ToString(), "Taewoo Lee");
            Assert.AreEqual(article.author.email.ToString(), "oiehot@gmail.com");
            Assert.AreEqual(article.author.twitter.ToString(), "@oiehot");
        }

        [Test]
        public void JArray_Add_Test()
        {
            JArray a = new JArray();
            a.Add(1);
            a.Add(3.141592f);
            a.Add("foo");
            a.Add(true);
            Assert.AreEqual((int)a[0], 1);
            Assert.AreEqual((float)a[1], 3.141592f);
            Assert.AreEqual(a[2].ToString(), "foo");
            Assert.AreEqual((bool)a[3], true);
        }

        [Test]
        public void JArray_Foreach_Test()
        {
            JArray a = new JArray();
            a.Add(1);
            a.Add(3.141592f);
            a.Add("foo");
            a.Add(true);
            foreach (JToken t in a)
            {
                Assert.IsNotNull(t);
            }
        }

        [Test]
        public void JArray_Remove_Test()
        {
            JArray jArray = new JArray();
            Assert.IsNotNull(jArray);
            jArray.Add("foo");
            jArray.Add("bar");
            jArray.Add("baz");
            jArray.Remove(jArray[0]);
            Assert.AreEqual(jArray.Count, 2);
        }

        [Test]
        public void JArray_RemoveAll_Test()
        {
            JArray jArray = new JArray();
            Assert.IsNotNull(jArray);
            jArray.Add("foo");
            jArray.Add("bar");
            jArray.Add("baz");
            jArray.RemoveAll();
            Assert.AreEqual(jArray.Count, 0);
        }


        [Test]
        public void JsonConvert_DeserializeObject_ListT_Test()
        {
            List<Article> articles = JsonConvert.DeserializeObject<List<Article>>(_jsonArticles);
            Assert.IsNotNull(articles);
            
            Article article = articles![0];
            
            Assert.AreEqual(article.id, 100);
            Assert.AreEqual(article.title, "Hello World");
            Assert.AreEqual(article.author.name, "Taewoo Lee");
            Assert.AreEqual(article.author.email, "oiehot@gmail.com");
            Assert.AreEqual(article.author.twitter, "@oiehot");
        }

        [Test]
        public void JsonConvert_DeserializeObject_T_Test()
        {
            Article article = JsonConvert.DeserializeObject<Article>(_jsonArticle);
            Assert.IsNotNull(article);
            Assert.AreEqual(article!.id, 100);
            Assert.AreEqual(article.title, "Hello World");
            Assert.AreEqual(article.author.name, "Taewoo Lee");
            Assert.AreEqual(article.author.email, "oiehot@gmail.com");
            Assert.AreEqual(article.author.twitter, "@oiehot");
        }

        [Test]
        public void JsonConvert_SerializeObject_Test()
        {
            Author author = new Author("Taewoo Lee", "oiehot@gmail.com", "@oiehot");
            Article article = new Article(200, "Foo Bar", author);
            string result = JsonConvert.SerializeObject(article);
            Debug.Log(result);
            Assert.IsNotNull(result);
        }
    }
}