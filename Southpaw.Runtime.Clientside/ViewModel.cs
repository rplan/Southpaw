using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using jQueryApi;

namespace Southpaw.Runtime.Clientside
{
    [Imported(IsRealType = true)]
    public abstract class ViewModel<TPrimaryKey> : IEvents
    {
        [IntrinsicProperty]
        public TPrimaryKey Id
        {
            get;
            set;
        }

        [IntrinsicProperty]
        public bool HasChanged
        {
            get;
            set; 
        }

        [InlineCode("this.set({{{propertyName}: {value}}})")]
        [IgnoreGenericArguments]
        protected void SetProperty<T>(string propertyName, T value)
        {
        }

        [InlineCode("this.get({propertyName})")]
        [IgnoreGenericArguments]
        protected T GetProperty<T>(string propertyName)
        {
            return default(T);
        }

        protected void SetPropertyFromString(string propertyName, string value, Type propertyType, bool isNullable)
        {
        }

        public bool Set(JsDictionary<string, object> attributes, ViewSetOptions options)
        {
            return false;
        }

        protected bool HasPropertyChanged(string propertyName)
        {
            return false;
        }

        //public abstract bool SetFromJSON(Dictionary<string, object> json);

        /// <summary>
        /// Can be overridden; returns true by default.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public bool Validate(Dictionary<string, object> attributes)
        {
            return false;
        }

        public void Change()
        {
        }

        public void Clear()
        {
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

        public JsDictionary<string, object> ToJSON()
        { 
            return null;
        }
    }
}