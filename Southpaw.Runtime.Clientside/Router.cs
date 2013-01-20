using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Southpaw.Runtime.Clientside;

namespace Southpaw.Runtime.Clientside
{
    public delegate void RoutingHandler(JsDictionary<string, string> routeParameters);

    [Imported(IsRealType = true)]
    [IgnoreGenericArguments]
    public abstract class Router
    {
        public Router Route(string route, string routeName, jQueryApi.jQueryEventHandler callback)
        {
            return this;
        }

        public Router Route(string route, RoutingHandler callback)
        {
            return this;
        }

        public Router Resource(string route, string routeName, Router nestedRouter)
        {
            return this;
        }


        public void Navigate(string fragment)
        {
        }

        public void Navigate(string fragment, RouteNavigateOptions options)
        {}

        public string GetCurrentHash() { return null; }

        public void StartHistory() { }

        public void StopHistory(){}

        public virtual Router Initialise()
        {
            return this;
        }

    }

    [Imported]
    [ScriptName("Object")]
    public class RouteNavigateOptions
    {
        [IntrinsicProperty]
        [ScriptName("trigger")]
        public bool TriggerCallback { get; set; }

        [IntrinsicProperty]
        [ScriptName("replace")]
        public bool IsReplace { get; set; }

        [IntrinsicProperty]
        public string Root { get; set; }
    }
}

namespace Backbone
{
    ///
    /// For testing only
    /// 
    [Imported(IsRealType = true)]
    public class History2
    {
        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void CheckUrl()
        {
            
        }

        public void Navigate(string hash)
        {
        }

        public void Navigate(string hash, RouteNavigateOptions routeNavigateOptions)
        {
        }
    }


}