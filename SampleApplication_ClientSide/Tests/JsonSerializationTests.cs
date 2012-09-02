using System;
using System.Collections.Generic;
using System.Text;
using System.Testing;
using System.Serialization;
using SampleApplication_ClientSide;

namespace SampleApplication_ClientSide.Tests
{
    [TestFixture]
    public class JsonDeserializationTests
    {
        [Test]
        public void DeserializeFromJson_SimpleObject_ShouldWork()
        {
            var json = new JsDictionary<string, object>();
            json["id"] = 1;
            json["email"] = "test@test.com";
            json["firstName"] = "John";
            json["lastName"] = "Doe";
            var user = new UserViewModel();
            user.SetFromJSON(json, null);
            Assert.AreEqual(json["id"], user.Id);
            Assert.AreEqual(json["email"], user.Email);
            Assert.AreEqual(json["firstName"], user.FirstName);
            Assert.AreEqual(json["lastName"], user.LastName);
        }

        [Test]
        public void DeserializeFromJson_WithNestedObject_ShouldWork()
        {
            var json = (JsDictionary<string, object>)Json.Parse(@" { ""title"": ""Test Title"", ""content"": ""Test Text"", ""author"": { ""email"": ""test@test.com"", ""id"": 2 } } ");
            var post = new PostViewModel();
            post.SetFromJSON(json, null);
            Assert.AreEqual("Test Title", post.Title);
            Assert.AreEqual("Test Text", post.Content);
            Assert.AreEqual(2, post.Author.Id);
            Assert.AreEqual("test@test.com", post.Author.Email);
        }

        // test:
        // should preserve nested object identity when nested object already exists (to avoid losing bound events when json is set)
        // should work for lists
    }
}
