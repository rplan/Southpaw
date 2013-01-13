using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using jQueryApi;

namespace Southpaw.Runtime.Clientside
{
    public enum ServiceSettingName
    {
        JsonReviver
    }
    public enum ViewSettingName
    {
    }
    public enum ViewModelSettingName
    {
    }

    [Imported(IsRealType = true)]
    public class GlobalSettings
    {
        [IntrinsicProperty]
        public static JsDictionary<ServiceSettingName, object> ServiceSettings { get { return null; } }
        [IntrinsicProperty]
        public static JsDictionary<ViewSettingName, object> ViewSettings { get { return null; } }
        [IntrinsicProperty]
        public static JsDictionary<ViewModelSettingName, object> ViewModelSettings { get { return null; } }
    }
    /*
    public abstract class Service<TQuery, TResults>
    {
        public void AddOnCompleteCallback(Action<TResults> onComplete)
        {
        }

        public void AddOnErrorCallback(Action onError)
        {
        }

        public virtual void Call(TQuery query)
        {
        }

        public abstract string GetUrl();
        public abstract string HttpMethod
        {
            get;
        }
    }
     */
    public delegate void OnCompleteCallback(object queryObject);

    //public interface IService<TResponse> : IService
    //{
        //void Call();
    //}
//
    //public interface IService<TQuery, TResponse> : IService
        //where TQuery: class
    //{
        //void Call(TQuery query = null);
    //}

    [IgnoreGenericArguments]
    public interface IService<TQuery, TResponse>
        where TQuery: class
    {
        void AddOnErrorCallback(Action<JsDictionary<string, object>, jQueryApi.jQueryXmlHttpRequest>  onError);
        void AddOnSuccessCallback(Action<JsDictionary<string, object>> onSuccess);
        string GetUrl();
        string HttpMethod { get; }
        object GetJsonReviver();
        void Initialise(ServiceOptions options = null);
        void Call(TQuery query = null);
    }

    [ScriptName("object")]
    public class ServiceOptions
    {
        [IntrinsicProperty]
        public string HttpMethod { get; set; }
        [IntrinsicProperty]
        public string Url { get; set; }
        [IntrinsicProperty]
        public object JsonReviver { get; set; }
    }

    //[Imported(IsRealType = true)]
    //public abstract class Service<TQuery, TResponse> : Service, IService<TQuery, TResponse>
        //where TQuery: class
    //{
        //public abstract void Call(TQuery query = null);
    //}
//
    //[Imported(IsRealType = true)]
    //public abstract class Service<TResponse> : Service, IService<TResponse>
    //{
        //public abstract void Call();
    //}
//

    [Imported(IsRealType = true)]
    //public abstract class Service : IService
    [IgnoreGenericArguments]
    public abstract class Service<TQuery, TResponse> : IService<TQuery, TResponse>
        where TQuery: class
    {

        public abstract void Call(TQuery query = null);

        public void AddOnErrorCallback(Action<JsDictionary<string, object>, jQueryXmlHttpRequest> onError)
        {
        }

        public void AddOnSuccessCallback(Action<JsDictionary<string, object>> onSuccess)
        {
        }

        public virtual void Initialise(ServiceOptions options = null)
        {
            
        }

        // TODO: only required because of scriptsharp's poor handling of delegates
        //protected void DoCall() {}
        protected void DoCall(object query = null) { }

        public abstract string GetUrl();
        public virtual string HttpMethod
        {
            get { return "GET"; }
        }

        public virtual object GetJsonReviver()
        {
            return null;
        }

        //protected abstract object InstantiateModel(Dictionary<string, object> returnValue);
        //{
            //return null;
        //}
    }
}