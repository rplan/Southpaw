using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Southpaw.Runtime.Clientside
{
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
    
    [Imported(IsRealType = true)]
    public abstract class Service
    {

        public void AddOnErrorCallback(Action onError)
        {
        }

        public void AddOnSuccessCallback(Action<JsDictionary<string, object>> onSuccess)
        {
        }

        // TODO: only required because of scriptsharp's poor handling of delegates
        protected void DoCall(object query)
        {
        }

        public abstract string GetUrl();
        public virtual string HttpMethod
        {
            get { return "GET"; }
        }
        //protected abstract object InstantiateModel(Dictionary<string, object> returnValue);
        //{
            //return null;
        //}
    }
}