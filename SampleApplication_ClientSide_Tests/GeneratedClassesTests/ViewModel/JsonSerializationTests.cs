using System;
using System.Collections.Generic;
using System.Text;
using System.Testing;
using System.Serialization;
using SampleApplication_ClientSide;

namespace SampleApplication_ClientSide_Tests.GeneratedClassesTests.ViewModel
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
            var json =
                (JsDictionary<string, object>)
                Json.Parse(
                    @" { ""title"": ""Test Title"", ""content"": ""Test Text"", ""author"": { ""email"": ""test@test.com"", ""id"": 2 } } ");
            var post = new PostViewModel();
            post.SetFromJSON(json, null);
            Assert.AreEqual("Test Title", post.Title);
            Assert.AreEqual("Test Text", post.Content);
            Assert.AreEqual(2, post.Author.Id);
            Assert.AreEqual("test@test.com", post.Author.Email);
        }

        [Test]
        public void DeserializeFromJson_WithNestedObject_WithExistingNestedObjectReference_ShouldPreserveObjectIdentity()
        {
            var json =
                (JsDictionary<string, object>)
                Json.Parse(
                    @" { ""title"": ""Test Title"", ""content"": ""Test Text"", ""author"": { ""email"": ""test@test.com"", ""id"": 2 } } ");
            var post = new PostViewModel();
            var originalAuthor = new UserViewModel {Id = 3, Email = "test2@test.com"};
            var anotherAuthor = new UserViewModel {Id = 2, Email = "test@test.com"};
            post.Author = originalAuthor;
            post.SetFromJSON(json, null);
            Assert.AreEqual("Test Title", post.Title);
            Assert.AreEqual("Test Text", post.Content);
            Assert.AreEqual(2, post.Author.Id);
            Assert.AreEqual("test@test.com", post.Author.Email);
            Assert.IsTrue(post.Author == originalAuthor,
                          "identity equality check - ensure any existing nested object's identity is preserved by FromJson, otherwise existing event bindings to be lost.");
            Assert.IsTrue(post.Author != anotherAuthor,
                          "identity equality check - verify that 2 instances which are known to have different identities are marked as not equal");
        }

        [Test]
        public void DeserializeFromJson_WithNestedList_ShouldWork()
        {
            var json =
                (JsDictionary<string, object>)
                Json.Parse(
                    @" { ""name"": ""Blog Name"", ""owner"": { ""email"": ""test@test.com"", ""id"": 2 }, ""posts"": [{ ""id"": 1, ""title"": ""test 1""}, {""id"": 2, ""title"": ""test 2"" }] } ");
            var blog = new BlogViewModel();
            blog.SetFromJSON(json, null);
            Assert.AreEqual("Blog Name", blog.Name);
            Assert.AreEqual(2, blog.Owner.Id);
            Assert.AreEqual("test@test.com", blog.Owner.Email);
            Assert.AreEqual(2, blog.Posts.Count);
            Assert.AreEqual(1, blog.Posts[0].Id);
            Assert.AreEqual("test 1", blog.Posts[0].Title);
            Assert.AreEqual(2, blog.Posts[1].Id);
            Assert.AreEqual("test 2", blog.Posts[1].Title);
        }

        [Test]
        public void DeserializeFromJson_WithNestedList_WithExistingListContent_ShouldReplaceListContents()
        {
            var json =
                (JsDictionary<string, object>)
                Json.Parse(
                    @" { ""name"": ""Blog Name"", ""owner"": { ""email"": ""test@test.com"", ""id"": 2 }, ""posts"": [{ ""id"": 1, ""title"": ""test 1""}, {""id"": 2, ""title"": ""test 2"" }] } ");
            var blog = new BlogViewModel();
            var originalPostsList = new List<PostViewModel>();
            originalPostsList.Add(new PostViewModel {Id = 5, Title = "test 5"});
            blog.Posts = originalPostsList;
            blog.SetFromJSON(json, null);
            Assert.AreEqual("Blog Name", blog.Name);
            Assert.AreEqual(2, blog.Owner.Id);
            Assert.AreEqual("test@test.com", blog.Owner.Email);
            Assert.AreEqual(2, blog.Posts.Count);
            Assert.AreEqual(1, blog.Posts[0].Id);
            Assert.AreEqual("test 1", blog.Posts[0].Title);
            Assert.AreEqual(2, blog.Posts[1].Id);
            Assert.AreEqual("test 2", blog.Posts[1].Title);
        }

        [Test]
        public void DeserializeFromJson_WithNestedList_WithExistingListContent_ShouldPreserveListIdentity()
        {
            var json =
                (JsDictionary<string, object>)
                Json.Parse(
                    @" { ""name"": ""Blog Name"", ""owner"": { ""email"": ""test@test.com"", ""id"": 2 }, ""posts"": [{ ""id"": 1, ""title"": ""test 1""}, {""id"": 2, ""title"": ""test 2"" }] } ");
            var blog = new BlogViewModel();
            var originalPostsList = new List<PostViewModel>();
            originalPostsList.Add(new PostViewModel {Id = 5, Title = "test 5"});
            blog.Posts = originalPostsList;
            var anotherPostsList = new List<PostViewModel>();
            blog.SetFromJSON(json, null);
            Assert.IsTrue(blog.Posts == originalPostsList,
                          "identity equality check - ensure any existing nested list's identity is preserved by FromJson, otherwise existing event bindings to be lost.");
            Assert.IsTrue(blog.Posts != anotherPostsList,
                          "identity equality check - verify that 2 instances which are known to have different identities are marked as not equal");
        }
    }
}
