using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using jQueryApi;

namespace Southpaw.Runtime.Clientside
{
    [Imported(IsRealType = true)]
    //[IgnoreGenericArguments(false)]
    public class ViewModelCollection<T> : IEvents, IEnumerator<T>, IEnumerable<T>
        //public abstract class ViewModelCollection : IEvents
    {
        private Type _type;

        public ViewModelCollection(Type t)
        {
            _type = t;
        }
        public void Add(T item, ViewSetOptions options = null) { }
        public void AddRange(ICollection<T> items, ViewSetOptions options = null) { }
        public void Remove(T item, ViewSetOptions options = null) { }
        public void RemoveAt(int index, ViewSetOptions options = null) { }
        public void Clear(ViewSetOptions options = null) { }
        public bool Contains(T item) { return false; }
        public T ElementAt(int idx) { throw new Exception("ignored"); }

        public int Count
        {
            get { return 0; }
        }

        public bool Set(List<JsDictionary<string,object>> models, ViewSetOptions options = null)
        {
            return false;
        }

        public bool SetFromJSON(List<JsDictionary<string,object>> models, ViewSetOptions options = null)
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

        #region IEnumerator implementation
        public void Dispose()
        {
        }

        bool IEnumerator.MoveNext()
        {
            return false;
        }

        void IEnumerator<T>.Reset()
        {
        }

        public T Current { get; private set; }

        bool IEnumerator<T>.MoveNext()
        {
            return false;
        }

        void IEnumerator.Reset()
        {
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
        #endregion

        public IEnumerator<T> GetEnumerator()
        {
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}