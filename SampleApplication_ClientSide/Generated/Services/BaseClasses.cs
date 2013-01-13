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
    public class BlogController_ShowServiceBase : Service<IdServiceParam,SampleApplication_ClientSide.BlogViewModel>
    {
        public override void Call(IdServiceParam query)
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
    public class BlogController_IndexServiceBase : Service<object,System.Collections.Generic.List<SampleApplication_ClientSide.BlogViewModel>>
    {
        public override void Call(object ignored = null)
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
    public class PostController_ShowServiceBase : Service<IdServiceParam,SampleApplication_ClientSide.PostViewModel>
    {
        public override void Call(IdServiceParam query)
        {
            DoCall(query);
        }
        public override string GetUrl()
        {
            return "/Post/Show";
        }
    }
}


