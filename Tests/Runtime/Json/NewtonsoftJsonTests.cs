using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; // JArray, JObject
using NUnit.Framework;

namespace Rano.Tests.Json
{
    public class NewtonsoftJsonTests
    {
        #region CLASSES

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

        #endregion

        #region FIELDS

        private string _json_articles;
        private string _json_article;

        #endregion

        [SetUp]
        public void Setup()
        {
            _json_articles = @"[
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

            _json_article = @"{
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
            dynamic article = JObject.Parse(_json_article);
            JToken nameToken = article.author.name;

            Assert.IsNotNull(nameToken);
            Assert.AreEqual(nameToken.ToString(), "Taewoo Lee");
        }

        [Test]
        public void JObject_Add_Test()
        {
            JObject jobj = new JObject();
            jobj.Add("int_key", 100);
            jobj.Add("float_key", 3.141592f);
            jobj.Add("string_key", "foo");
            jobj.Add("boolean_key", true);

            Assert.IsNotNull(jobj.ToString());
            Assert.AreEqual((int)jobj["int_key"], 100);
            Assert.AreEqual((float)jobj["float_key"], 3.141592f);
            Assert.AreEqual(jobj["string_key"].ToString(), "foo");
            Assert.AreEqual((bool)jobj["boolean_key"], true);
        }

        [Test]
        public void JObject_ValueQuery_Test()
        {
            JObject jobj = new JObject();
            jobj.Add("int_key", 100);
            jobj.Add("float_key", 3.141592f);
            jobj.Add("string_key", "foo");
            jobj.Add("boolean_key", true);

            Assert.AreEqual(jobj.Value<int>("int_key"), 100);
            Assert.AreEqual(jobj.Value<int?>("int_key"), 100);
            Assert.AreEqual(jobj.Value<float>("float_key"), 3.141592f);
            Assert.AreEqual(jobj.Value<string>("string_key"), "foo");
            Assert.AreEqual(jobj.Value<bool>("boolean_key"), true);
        }

        [Test]
        public void JObject_Remove_Test()
        {
            const string KEY = "foo";
            const string VALUE = "bar";
            
            JObject jObject = new JObject();
            
            jObject.Add(KEY, VALUE);
            jObject.Remove(KEY);
            Assert.IsFalse(jObject.ContainsKey(KEY));
        }

        [Test]
        public void JObject_RemoveAll_Test()
        {
            JObject jobj = new JObject();
            jobj.Add("foo", "0");
            jobj.Add("bar", "1");
            jobj.Add("baz", "2");
            jobj.RemoveAll();
        }

        [Test]
        public void JObject_ContainsKey_Test()
        {
            JObject jobj = new JObject();
            jobj.Add("foo", "bar");
            Assert.IsTrue(jobj.ContainsKey("foo"));
            Assert.IsFalse(jobj.ContainsKey("baz"));
        }

        [Test]
        public void JObject_Parse_Test()
        {
            dynamic article = JObject.Parse(_json_article);

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
            JObject jobj = JObject.FromObject(article);

            Assert.IsNotNull(jobj.ToString());
            Assert.AreEqual((int)jobj["id"], 100);
            Assert.AreEqual(jobj["title"].ToString(), "Foo Bar");
            Assert.AreEqual(jobj["author"]["name"].ToString(), "Taewoo Lee");
            Assert.AreEqual(jobj["author"]["email"].ToString(), "oiehot@gmail.com");
            Assert.AreEqual(jobj["author"]["twitter"].ToString(), "@oiehot");
        }

        [Test]
        public void JArray_Parse_Test()
        {
            dynamic articles = JArray.Parse(_json_articles);
            dynamic article = articles[0];

            Assert.AreEqual((int)article.id, (int)100);
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
            JArray a = new JArray();
            a.Add("foo");
            a.Add("bar");
            a.Add("baz");
            a.Remove(a[0]);
            Assert.AreEqual(a.Count, 2);
        }

        [Test]
        public void JArray_RemoveAll_Test()
        {
            JArray a = new JArray();
            a.Add("foo");
            a.Add("bar");
            a.Add("baz");
            a.RemoveAll();
            Assert.AreEqual(a.Count, 0);
        }


        [Test]
        public void JsonConvert_DeserializeObject_ListT_Test()
        {
            List<Article> articles = JsonConvert.DeserializeObject<List<Article>>(_json_articles);
            Article article = articles[0];
            Assert.AreEqual(article.id, 100);
            Assert.AreEqual(article.title, "Hello World");
            Assert.AreEqual(article.author.name, "Taewoo Lee");
            Assert.AreEqual(article.author.email, "oiehot@gmail.com");
            Assert.AreEqual(article.author.twitter, "@oiehot");
        }

        [Test]
        public void JsonConvert_DeserializeObject_T_Test()
        {
            Article article = JsonConvert.DeserializeObject<Article>(_json_article);
            Assert.AreEqual(article.id, 100);
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