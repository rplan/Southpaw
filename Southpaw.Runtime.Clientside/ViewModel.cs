using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using jQueryApi;

namespace Southpaw.Runtime.Clientside
{
    [Imported(IsRealType = true)]
    public abstract class ViewModel : IEvents
    {
        [IntrinsicProperty]
        // TODO: scriptsharp doesn't seem to like this being a generic
        public int Id
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

        protected void SetProperty(string propertyName, object value)
        {
        }

        protected object GetProperty(string propertyName)
        {
            return null;
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

        public Dictionary<string, object> ToJSON()
        { 
            return null;
        }
    }
}