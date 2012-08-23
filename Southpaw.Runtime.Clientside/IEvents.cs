using jQueryApi;

namespace Southpaw.Runtime.Clientside
{
    /// <summary>
    /// Useful as a convention, rather than to force a public signature. In most cases, it may be preferable for implementations of this interface to be protected.
    /// </summary>
    public interface IEvents
    {
        void Bind(string eventName, jQueryEventHandler callback);
        void ClearEvents();
        void Unbind(string eventName, jQueryEventHandler callback);
        void Trigger(string eventName, jQueryEvent evt);
    }
}