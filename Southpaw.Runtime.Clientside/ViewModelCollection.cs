using System;
using System.Runtime.CompilerServices;
using jQueryApi;

namespace Southpaw.Runtime.Clientside
{
    [Imported]
    public class ViewModelCollection<T> : IEvents
    //public abstract class ViewModelCollection : IEvents
    {
        public void Add(T item) { }
        //TODO: causes scriptsharp compilation to fail
        //public void AddRange(ICollection<T> items) { }
        public void Remove(T item) { }
        public void RemoveAt(int index) { }
        public void Clear() { }
        public bool Contains(T item) { return false; }
        public T ElementAt(int idx) { throw new Exception("ignored"); }

        public int Count
        {
            get { return 0; }
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

    }
}