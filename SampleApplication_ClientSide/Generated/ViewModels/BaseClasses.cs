using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Southpaw.Runtime.Clientside;

namespace SampleApplication_ClientSide
{
    public class BlogViewModelBase : ViewModel<System.Int32>
    {
        public string Name
        {
            [InlineCode("{this}.get('Name')")]
            get { return default(string); }
            [InlineCode("{this}.set({{'Name': {value}}})")]
            set { }
        }

        public IList<SampleApplication_ClientSide.PostViewModel> Posts
        {
            [InlineCode("{this}.get('Posts')")]
            get { return default(List<SampleApplication_ClientSide.PostViewModel>); }
            [InlineCode("{this}.set({{'Posts': {value}}})")]
            set { }
        }

        public SampleApplication_ClientSide.UserViewModel Owner
        {
            [InlineCode("{this}.get('Owner')")]
            get { return default(SampleApplication_ClientSide.UserViewModel); }
            [InlineCode("{this}.set({{'Owner': {value}}})")]
            set { }
        }

        public override bool SetFromJSON(JsDictionary<string, object> json, ViewSetOptions options)
        {
            if (json == null)
                return true;
            if (json.ContainsKey("Posts"))
            {
                IList<SampleApplication_ClientSide.PostViewModel> l = new List<SampleApplication_ClientSide.PostViewModel>();
                if (this.Posts != null)
                    l = this.Posts;

                l.Clear();
                var jsonList = (List<JsDictionary<string, object>>)json["Posts"];
                if (jsonList != null)
                {
                    foreach(JsDictionary<string, object> itemJson in jsonList)
                    {
                        SampleApplication_ClientSide.PostViewModel x = new SampleApplication_ClientSide.PostViewModel();
                        if (!x.SetFromJSON(itemJson, options))
                            return false;
                        l.Add(x);
                    }
                }
                json["Posts"] = l;
            }
            if (json.ContainsKey("Owner"))
            {
                if (this.Owner != null)
                {
                    if (this.Owner.SetFromJSON((JsDictionary<string, object>)json["Owner"], options))
                        json.Remove("Owner");
                    else
                        return false;
                }
                else
                {
                    SampleApplication_ClientSide.UserViewModel x = new SampleApplication_ClientSide.UserViewModel();
                    if (!x.SetFromJSON((JsDictionary<string, object>)json["Owner"], options))
                        return false;
                    json["Owner"] = x;
                }
            }
            return base.Set(json, options);
        }
        public void SetIdFromString(string value)
        {
            SetPropertyFromString("Id", value, typeof(int), false);
        }

        public void SetNameFromString(string value)
        {
            SetPropertyFromString("Name", value, typeof(string), false);
        }

        public override bool Validate(JsDictionary<string, object> attributes)
        {
            this.Errors.Clear();
            string res = null;
            res = new Southpaw.Runtime.Clientside.Validation.Type.IntValidator().Validate(attributes["Id"], new Southpaw.Runtime.Clientside.Validation.Type.TypeValidatorOptions { Property = "Id" });
            if (res != null) this.Errors.AddError("Id", res);
            return !this.Errors.IsError;
        }
    }
}

namespace SampleApplication_ClientSide
{
    public class UserViewModelBase : ViewModel<System.Int32>
    {
        public string Email
        {
            [InlineCode("{this}.get('Email')")]
            get { return default(string); }
            [InlineCode("{this}.set({{'Email': {value}}})")]
            set { }
        }

        public string FirstName
        {
            [InlineCode("{this}.get('FirstName')")]
            get { return default(string); }
            [InlineCode("{this}.set({{'FirstName': {value}}})")]
            set { }
        }

        public string LastName
        {
            [InlineCode("{this}.get('LastName')")]
            get { return default(string); }
            [InlineCode("{this}.set({{'LastName': {value}}})")]
            set { }
        }

        public override bool SetFromJSON(JsDictionary<string, object> json, ViewSetOptions options)
        {
            if (json == null)
                return true;
            return base.Set(json, options);
        }
        public void SetIdFromString(string value)
        {
            SetPropertyFromString("Id", value, typeof(int), false);
        }

        public void SetEmailFromString(string value)
        {
            SetPropertyFromString("Email", value, typeof(string), false);
        }

        public void SetFirstNameFromString(string value)
        {
            SetPropertyFromString("FirstName", value, typeof(string), false);
        }

        public void SetLastNameFromString(string value)
        {
            SetPropertyFromString("LastName", value, typeof(string), false);
        }

        public override bool Validate(JsDictionary<string, object> attributes)
        {
            this.Errors.Clear();
            string res = null;
            res = new Southpaw.Runtime.Clientside.Validation.Type.IntValidator().Validate(attributes["Id"], new Southpaw.Runtime.Clientside.Validation.Type.TypeValidatorOptions { Property = "Id" });
            if (res != null) this.Errors.AddError("Id", res);
            return !this.Errors.IsError;
        }
    }
}

namespace SampleApplication_ClientSide
{
    public class AdminUserViewModelBase : SampleApplication_ClientSide.UserViewModel
    {
        public int Permission
        {
            [InlineCode("{this}.get('Permission')")]
            get { return default(int); }
            [InlineCode("{this}.set({{'Permission': {value}}})")]
            set { }
        }

        public override bool SetFromJSON(JsDictionary<string, object> json, ViewSetOptions options)
        {
            if (json == null)
                return true;
            return base.SetFromJSON(json, options);
        }
        public void SetPermissionFromString(string value)
        {
            SetPropertyFromString("Permission", value, typeof(int), false);
        }

        public override bool Validate(JsDictionary<string, object> attributes)
        {
            this.Errors.Clear();
            string res = null;
            res = new Southpaw.Runtime.Clientside.Validation.Type.IntValidator().Validate(attributes["Permission"], new Southpaw.Runtime.Clientside.Validation.Type.TypeValidatorOptions { Property = "Permission" });
            if (res != null) this.Errors.AddError("Permission", res);
            return !this.Errors.IsError;
        }
    }
}

namespace SampleApplication_ClientSide
{
    public class PostViewModelBase : ViewModel<System.Int32>
    {
        public SampleApplication_ClientSide.UserViewModel Author
        {
            [InlineCode("{this}.get('Author')")]
            get { return default(SampleApplication_ClientSide.UserViewModel); }
            [InlineCode("{this}.set({{'Author': {value}}})")]
            set { }
        }

        public string Title
        {
            [InlineCode("{this}.get('Title')")]
            get { return default(string); }
            [InlineCode("{this}.set({{'Title': {value}}})")]
            set { }
        }

        public string Content
        {
            [InlineCode("{this}.get('Content')")]
            get { return default(string); }
            [InlineCode("{this}.set({{'Content': {value}}})")]
            set { }
        }

        public SampleApplication_ClientSide.BlogViewModel Blog
        {
            [InlineCode("{this}.get('Blog')")]
            get { return default(SampleApplication_ClientSide.BlogViewModel); }
            [InlineCode("{this}.set({{'Blog': {value}}})")]
            set { }
        }

        public DateTime PostedAt
        {
            [InlineCode("{this}.get('PostedAt')")]
            get { return default(DateTime); }
            [InlineCode("{this}.set({{'PostedAt': {value}}})")]
            set { }
        }

        public override bool SetFromJSON(JsDictionary<string, object> json, ViewSetOptions options)
        {
            if (json == null)
                return true;
            if (json.ContainsKey("Author"))
            {
                if (this.Author != null)
                {
                    if (this.Author.SetFromJSON((JsDictionary<string, object>)json["Author"], options))
                        json.Remove("Author");
                    else
                        return false;
                }
                else
                {
                    SampleApplication_ClientSide.UserViewModel x = new SampleApplication_ClientSide.UserViewModel();
                    if (!x.SetFromJSON((JsDictionary<string, object>)json["Author"], options))
                        return false;
                    json["Author"] = x;
                }
            }
            if (json.ContainsKey("Blog"))
            {
                if (this.Blog != null)
                {
                    if (this.Blog.SetFromJSON((JsDictionary<string, object>)json["Blog"], options))
                        json.Remove("Blog");
                    else
                        return false;
                }
                else
                {
                    SampleApplication_ClientSide.BlogViewModel x = new SampleApplication_ClientSide.BlogViewModel();
                    if (!x.SetFromJSON((JsDictionary<string, object>)json["Blog"], options))
                        return false;
                    json["Blog"] = x;
                }
            }
            return base.Set(json, options);
        }
        public void SetIdFromString(string value)
        {
            SetPropertyFromString("Id", value, typeof(int), false);
        }

        public void SetTitleFromString(string value)
        {
            SetPropertyFromString("Title", value, typeof(string), false);
        }

        public void SetContentFromString(string value)
        {
            SetPropertyFromString("Content", value, typeof(string), false);
        }

        public void SetPostedAtFromString(string value)
        {
            SetPropertyFromString("PostedAt", value, typeof(DateTime), false);
        }

        public override bool Validate(JsDictionary<string, object> attributes)
        {
            this.Errors.Clear();
            string res = null;
            res = new Southpaw.Runtime.Clientside.Validation.Type.IntValidator().Validate(attributes["Id"], new Southpaw.Runtime.Clientside.Validation.Type.TypeValidatorOptions { Property = "Id" });
            if (res != null) this.Errors.AddError("Id", res);
            res = new Southpaw.Runtime.Clientside.Validation.RequiredValidator().Validate(attributes["Title"], new Southpaw.Runtime.Clientside.Validation.RequiredValidatorOptions { Property = "Title", AllowEmptyStrings = false, });
            if (res != null) this.Errors.AddError("Title", res);
            res = new Southpaw.Runtime.Clientside.Validation.RequiredValidator().Validate(attributes["Content"], new Southpaw.Runtime.Clientside.Validation.RequiredValidatorOptions { Property = "Content", AllowEmptyStrings = false, });
            if (res != null) this.Errors.AddError("Content", res);
            res = new Southpaw.Runtime.Clientside.Validation.LengthValidator().Validate(attributes["Content"], new Southpaw.Runtime.Clientside.Validation.LengthValidatorOptions { Property = "Content", MaximumLength = 100, MinimumLength = 4, });
            if (res != null) this.Errors.AddError("Content", res);
            res = new Southpaw.Runtime.Clientside.Validation.Type.DateValidator().Validate(attributes["PostedAt"], new Southpaw.Runtime.Clientside.Validation.Type.TypeValidatorOptions { Property = "PostedAt" });
            if (res != null) this.Errors.AddError("PostedAt", res);
            return !this.Errors.IsError;
        }
    }
}

