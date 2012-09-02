using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Southpaw.Runtime.Clientside;

namespace SampleApplication_ClientSide
{
    public class BlogViewModelBase : ViewModel
    {
        public int Id
        {
            [InlineCode("{this}.get('id')")]
            get { return default(int); }
            [InlineCode("{this}.set({{'id': {value}}})")]
            set { }
        }

        public string Name
        {
            [InlineCode("{this}.get('name')")]
            get { return default(string); }
            [InlineCode("{this}.set({{'name': {value}}})")]
            set { }
        }

        public List<SampleApplication_ClientSide.PostViewModel> Posts
        {
            [InlineCode("{this}.get('posts')")]
            get { return default(List<SampleApplication_ClientSide.PostViewModel>); }
            [InlineCode("{this}.set({{'posts': {value}}})")]
            set { }
        }

        public SampleApplication_ClientSide.UserViewModel Owner
        {
            [InlineCode("{this}.get('owner')")]
            get { return default(SampleApplication_ClientSide.UserViewModel); }
            [InlineCode("{this}.set({{'owner': {value}}})")]
            set { }
        }

        public bool SetFromJSON(JsDictionary<string, object> json, ViewSetOptions options)
        {
            if (json == null)
                return true;
            if (json.ContainsKey("posts"))
            {
                List<SampleApplication_ClientSide.PostViewModel> l = new List<SampleApplication_ClientSide.PostViewModel>();
                if (this.Posts != null)
                    l = this.Posts;

                foreach(JsDictionary<string, object> itemJson in (List<JsDictionary<string, object>>)json["posts"])
                {
                    SampleApplication_ClientSide.PostViewModel x = new SampleApplication_ClientSide.PostViewModel();
                    if (!x.SetFromJSON(itemJson, options))
                        return false;
                    l.Add(x);
                }
                json["Posts"] = l;
            }
            if (json.ContainsKey("owner"))
            {
                if (this.Owner != null)
                {
                    if (this.Owner.SetFromJSON((JsDictionary<string, object>)json["owner"], options))
                        json.Remove("owner");
                    else
                        return false;
                }
                else
                {
                    SampleApplication_ClientSide.UserViewModel x = new SampleApplication_ClientSide.UserViewModel();
                    if (!x.SetFromJSON((JsDictionary<string, object>)json["owner"], options))
                        return false;
                    json["owner"] = x;
                }
            }
            return base.Set(json, options);
        }
        public void SetIdFromString(string value)
        {
            SetPropertyFromString("id", value, typeof(int), false);
        }

        public void SetNameFromString(string value)
        {
            SetPropertyFromString("name", value, typeof(string), false);
        }

    }
}

namespace SampleApplication_ClientSide
{
    public class UserViewModelBase : ViewModel
    {
        public int Id
        {
            [InlineCode("{this}.get('id')")]
            get { return default(int); }
            [InlineCode("{this}.set({{'id': {value}}})")]
            set { }
        }

        public string Email
        {
            [InlineCode("{this}.get('email')")]
            get { return default(string); }
            [InlineCode("{this}.set({{'email': {value}}})")]
            set { }
        }

        public string FirstName
        {
            [InlineCode("{this}.get('firstName')")]
            get { return default(string); }
            [InlineCode("{this}.set({{'firstName': {value}}})")]
            set { }
        }

        public string LastName
        {
            [InlineCode("{this}.get('lastName')")]
            get { return default(string); }
            [InlineCode("{this}.set({{'lastName': {value}}})")]
            set { }
        }

        public bool SetFromJSON(JsDictionary<string, object> json, ViewSetOptions options)
        {
            if (json == null)
                return true;
            return base.Set(json, options);
        }
        public void SetIdFromString(string value)
        {
            SetPropertyFromString("id", value, typeof(int), false);
        }

        public void SetEmailFromString(string value)
        {
            SetPropertyFromString("email", value, typeof(string), false);
        }

        public void SetFirstNameFromString(string value)
        {
            SetPropertyFromString("firstName", value, typeof(string), false);
        }

        public void SetLastNameFromString(string value)
        {
            SetPropertyFromString("lastName", value, typeof(string), false);
        }

    }
}

namespace SampleApplication_ClientSide
{
    public class PostViewModelBase : ViewModel
    {
        public int Id
        {
            [InlineCode("{this}.get('id')")]
            get { return default(int); }
            [InlineCode("{this}.set({{'id': {value}}})")]
            set { }
        }

        public SampleApplication_ClientSide.UserViewModel Author
        {
            [InlineCode("{this}.get('author')")]
            get { return default(SampleApplication_ClientSide.UserViewModel); }
            [InlineCode("{this}.set({{'author': {value}}})")]
            set { }
        }

        public string Title
        {
            [InlineCode("{this}.get('title')")]
            get { return default(string); }
            [InlineCode("{this}.set({{'title': {value}}})")]
            set { }
        }

        public string Content
        {
            [InlineCode("{this}.get('content')")]
            get { return default(string); }
            [InlineCode("{this}.set({{'content': {value}}})")]
            set { }
        }

        public SampleApplication_ClientSide.BlogViewModel Blog
        {
            [InlineCode("{this}.get('blog')")]
            get { return default(SampleApplication_ClientSide.BlogViewModel); }
            [InlineCode("{this}.set({{'blog': {value}}})")]
            set { }
        }

        public DateTime PostedAt
        {
            [InlineCode("{this}.get('postedAt')")]
            get { return default(DateTime); }
            [InlineCode("{this}.set({{'postedAt': {value}}})")]
            set { }
        }

        public bool SetFromJSON(JsDictionary<string, object> json, ViewSetOptions options)
        {
            if (json == null)
                return true;
            if (json.ContainsKey("author"))
            {
                if (this.Author != null)
                {
                    if (this.Author.SetFromJSON((JsDictionary<string, object>)json["author"], options))
                        json.Remove("author");
                    else
                        return false;
                }
                else
                {
                    SampleApplication_ClientSide.UserViewModel x = new SampleApplication_ClientSide.UserViewModel();
                    if (!x.SetFromJSON((JsDictionary<string, object>)json["author"], options))
                        return false;
                    json["author"] = x;
                }
            }
            if (json.ContainsKey("blog"))
            {
                if (this.Blog != null)
                {
                    if (this.Blog.SetFromJSON((JsDictionary<string, object>)json["blog"], options))
                        json.Remove("blog");
                    else
                        return false;
                }
                else
                {
                    SampleApplication_ClientSide.BlogViewModel x = new SampleApplication_ClientSide.BlogViewModel();
                    if (!x.SetFromJSON((JsDictionary<string, object>)json["blog"], options))
                        return false;
                    json["blog"] = x;
                }
            }
            return base.Set(json, options);
        }
        public void SetIdFromString(string value)
        {
            SetPropertyFromString("id", value, typeof(int), false);
        }

        public void SetTitleFromString(string value)
        {
            SetPropertyFromString("title", value, typeof(string), false);
        }

        public void SetContentFromString(string value)
        {
            SetPropertyFromString("content", value, typeof(string), false);
        }

        public void SetPostedAtFromString(string value)
        {
            SetPropertyFromString("postedAt", value, typeof(DateTime), false);
        }

    }
}

