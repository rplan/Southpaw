using System;
using System.Collections.Generic;
using Southpaw.Runtime.Clientside;
using System.Runtime.CompilerServices;

namespace SampleApplication_ClientSide
{
    [IgnoreNamespace][Imported][ScriptName("Object")]
    public class IdServiceParam
    {
        [IntrinsicProperty]
        public int Id { get; set; }

    }
}

namespace SampleApplication_ClientSide
{
    public class BlogController_ShowServiceBase : Service
    {
        public virtual void Call(IdServiceParam query)
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
        DoCall();
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
        public virtual void Call(IdServiceParam query)
        {
            DoCall(query);
        }
        public override string GetUrl()
        {
            return "/Post/Show";
        }
    }
}


