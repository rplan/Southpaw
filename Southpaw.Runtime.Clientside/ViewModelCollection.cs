using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using jQueryApi;

namespace Southpaw.Runtime.Clientside
{
    [Imported(IsRealType = true)]
    //[IgnoreGenericArguments(false)]
    public class ViewModelCollection<T> : IEvents
    //public abstract class ViewModelCollection : IEvents
    {
        private Type _type;

        public ViewModelCollection(Type t)
        {
            _type = t;
        }
        public void Add(T item) { }
        public void AddRange(ICollection<T> items) { }
        public void Remove(T item) { }
        public void RemoveAt(int index) { }
        public void Clear() { }
        public bool Contains(T item) { return false; }
        public T ElementAt(int idx) { throw new Exception("ignored"); }

        public int Count
        {
            get { return 0; }
        }

        public bool Set(List<JsDictionary<string,object>> models)
        {
            return false;
        }

        public bool Set(List<JsDictionary<string,object>> models, ViewSetOptions options)
        {
            return false;
        }

        public bool SetFromJSON(List<JsDictionary<string,object>> models)
        {
            return false;
        }

        public bool SetFromJSON(List<JsDictionary<string,object>> models, ViewSetOptions options)
        {
            return false;
        }

        //[IntrinsicProperty]
        //public T this[int index] {
            //get
            //{
                //return default(T);
            //}
            //set {
            //}
        //}

        public JsDictionary<string, object> ToJSON()
        {
            return null;
        }


        public void Bind(string eventName, jQueryEventHandler callback)
        {
        }

        public void ClearEvents()
        {
        }

        public void Unbind(string eventName, jQueryEventHandler callback)
        {
        }

        public void Trigger(string eventName, jQueryEvent evt)
        {
        }

        public T GetById(int id)
        {
            return default(T);
        }

        public T GetByCid(int cid)
        {
            return default(T);
        }
    }
}