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
            json["Id"] = 1;
            json["Email"] = "test@test.com";
            json["FirstName"] = "John";
            json["LastName"] = "Doe";
            var user = new UserViewModel();
            user.SetFromJSON(json, null);
            Assert.AreEqual(user.Id, json["Id"]);
            Assert.AreEqual(user.Email, json["Email"]);
            Assert.AreEqual(user.FirstName, json["FirstName"]);
            Assert.AreEqual(user.LastName, json["LastName"]);
        }

        [Test]
        public void DeserializeFromJson_WithNestedObject_ShouldWork()
        {
            var json =
                (JsDictionary<string, object>)
                Json.Parse(
                    @" { ""Title"": ""Test Title"", ""Content"": ""Test Text"", ""Author"": { ""Email"": ""test@test.com"", ""Id"": 2 } } ");
            var post = new PostViewModel();
            post.SetFromJSON(json, null);
            Assert.AreEqual(post.Title, "Test Title");
            Assert.AreEqual(post.Content, "Test Text");
            Assert.AreEqual(post.Author.Id, 2);
            Assert.AreEqual(post.Author.Email, "test@test.com");
        }

        [Test]
        public void DeserializeFromJson_WithNestedObject_WithExistingNestedObjectReference_ShouldPreserveObjectIdentity()
        {
            var json =
                (JsDictionary<string, object>)
                Json.Parse(
                    @" { ""Title"": ""Test Title"", ""Content"": ""Test Text"", ""Author"": { ""Email"": ""test@test.com"", ""Id"": 2 } } ");
            var post = new PostViewModel();
            var originalAuthor = new UserViewModel {Id = 3, Email = "test2@test.com"};
            var anotherAuthor = new UserViewModel {Id = 2, Email = "test@test.com"};
            post.Author = originalAuthor;
            post.SetFromJSON(json, null);
            Assert.AreEqual(post.Title, "Test Title");
            Assert.AreEqual(post.Content, "Test Text");
            Assert.AreEqual(post.Author.Id, 2);
            Assert.AreEqual(post.Author.Email, "test@test.com");
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
                    @" { ""Name"": ""Blog Name"", ""Owner"": { ""Email"": ""test@test.com"", ""Id"": 2 }, ""Posts"": [{ ""Id"": 1, ""Title"": ""test 1""}, {""Id"": 2, ""Title"": ""test 2"" }] } ");
            var blog = new BlogViewModel();
            blog.SetFromJSON(json, null);
            Assert.AreEqual(blog.Name, "Blog Name");
            Assert.AreEqual(blog.Owner.Id, 2);
            Assert.AreEqual(blog.Owner.Email, "test@test.com");
            Assert.AreEqual(blog.Posts.Count, 2);
            Assert.AreEqual(blog.Posts[0].Id, 1);
            Assert.AreEqual(blog.Posts[0].Title, "test 1");
            Assert.AreEqual(blog.Posts[1].Id, 2);
            Assert.AreEqual(blog.Posts[1].Title, "test 2");
        }

        [Test]
        public void DeserializeFromJson_WithNestedList_WithExistingListContent_ShouldReplaceListContents()
        {
            var json =
                (JsDictionary<string, object>)
                Json.Parse(
                    @" { ""Name"": ""Blog Name"", ""Owner"": { ""Email"": ""test@test.com"", ""Id"": 2 }, ""Posts"": [{ ""Id"": 1, ""Title"": ""test 1""}, {""Id"": 2, ""Title"": ""test 2"" }] } ");
            var blog = new BlogViewModel();
            var originalPostsList = new List<PostViewModel>();
            originalPostsList.Add(new PostViewModel {Id = 5, Title = "test 5"});
            blog.Posts = originalPostsList;
            blog.SetFromJSON(json, null);
            Assert.AreEqual(blog.Name, "Blog Name");
            Assert.AreEqual(blog.Owner.Id, 2);
            Assert.AreEqual(blog.Owner.Email, "test@test.com");
            Assert.AreEqual(blog.Posts.Count, 2);
            Assert.AreEqual(blog.Posts[0].Id, 1);
            Assert.AreEqual(blog.Posts[0].Title, "test 1");
            Assert.AreEqual(blog.Posts[1].Id, 2);
            Assert.AreEqual(blog.Posts[1].Title, "test 2");
        }

        [Test]
        public void DeserializeFromJson_WithNestedList_WithExistingListContent_ShouldPreserveListIdentity()
        {
            var json =
                (JsDictionary<string, object>)
                Json.Parse(
                    @" { ""Name"": ""Blog Name"", ""Owner"": { ""Email"": ""test@test.com"", ""Id"": 2 }, ""Posts"": [{ ""Id"": 1, ""Title"": ""test 1""}, {""Id"": 2, ""Title"": ""test 2"" }] } ");
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
