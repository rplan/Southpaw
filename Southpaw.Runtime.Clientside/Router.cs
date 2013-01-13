using System.Runtime.CompilerServices;

namespace Southpaw.Runtime.Clientside
{
    [Imported(IsRealType = true)]
    [IgnoreGenericArguments]
    public abstract class Router
    {
        public void Route(string route, string routeName, jQueryApi.jQueryEventHandler callback)
        {
        }

        public void Route(string route, jQueryApi.jQueryEventHandler callback)
        {
        }

        public void Navigate(string fragment)
        {
        }

        public void Navigate(string fragment, RouteNavigateOptions options)
        {}

        public string GetCurrentHash() { return null; }

        public void StartHistory() { }

        public void StopHistory(){}

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
    }


}