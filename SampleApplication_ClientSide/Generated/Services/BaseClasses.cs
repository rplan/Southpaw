using System;
using System.Collections.Generic;
using Southpaw.Runtime.Clientside;

namespace SampleApplication_ClientSide
{
    public class BlogController_ShowServiceBase : Service
    {
        public virtual void Call(int query)
        {
            DoCall(query);
        }
        public override string GetUrl()
        {
            return "/Blog/Show";
        }
    }
}

    namespace SampleApplication_ClientSide
    {
public class BlogController_IndexServiceBase : Service
{
    public virtual void Call()
    {
        DoCall(null);
    }
    public override string GetUrl()
    {
        return "/Blog/Index";
    }
}
    }


namespace SampleApplication_ClientSide
{
    public class PostController_ShowServiceBase : Service
    {
        public virtual void Call(int query)
        {
            DoCall(query);
        }
        public override string GetUrl()
        {
            return "/Post/Show";
        }
    }
}


