using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Southpaw.Runtime.Clientside
{
    public interface IViewModel 
    {
        /// <summary>
        /// Whether or not the model has changed since the 
        /// last change event was fired.
        /// </summary>
        [PreserveName]
        bool HasChanged();

        [PreserveName]
        bool HasChanged(string propertyName);
        [PreserveName]
        bool Set(JsDictionary<string, object> attributes);
        [PreserveName]
        bool Set(JsDictionary<string, object> attributes, ViewSetOptions options);

        /// <summary>
        /// Can be overridden; returns true by default.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        [PreserveName]
        bool Validate(JsDictionary<string, object> attributes);

        [PreserveName]
        void Change();
        [PreserveName]
        void Clear();
        [ScriptName("toJSON")]
        JsDictionary<string, object> ToJSON();

        [PreserveName]
        ValidationResults Errors { get; }

        void Bind(string eventName, ModelEventHandler callback);
        void ClearEvents();
        void Unbind(string eventName, ModelEventHandler callback);
        void Trigger(string eventName);
    }

    [Imported(IsRealType = true)]
    [IgnoreGenericArguments]
    [ScriptName("ViewModel$1")]
    public abstract class ViewModel<TPrimaryKey> : IViewModel
    {
        public TPrimaryKey Id
        {
            [InlineCode("{this}.get('Id')")]
            get { return default(TPrimaryKey); }
            [InlineCode("{this}.set({{'Id': {value}}})")]
            set { }
        }

        /// <summary>
        /// Whether or not the model has changed since the 
        /// last change event was fired.
        /// </summary>
        public bool HasChanged()
        {
            return false;
        }

        public bool HasChanged(string propertyName)
        {
            return false;
        }

        [InlineCode("{this}.set({{{propertyName}: {value}}})")]
        [IgnoreGenericArguments]
        protected void SetProperty<T>(string propertyName, T value)
        {
        }

        [InlineCode("{this}.get({propertyName})")]
        [IgnoreGenericArguments]
        protected T GetProperty<T>(string propertyName)
        {
            return default(T);
        }

        protected void SetPropertyFromString(string propertyName, string value, Type propertyType, bool isNullable)
        {
        }

        public bool Set(JsDictionary<string, object> attributes)
        {
            return false;
        }

        public bool Set(JsDictionary<string, object> attributes, ViewSetOptions options)
        {
            return false;
        }

        protected bool HasPropertyChanged(string propertyName)
        {
            return false;
        }

        [PreserveCase]
        public abstract bool SetFromJSON(JsDictionary<string, object> json, ViewSetOptions options);

        /// <summary>
        /// Can be overridden; returns true by default.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public virtual bool Validate(JsDictionary<string, object> attributes)
        {
            return false;
        }

        public void Change()
        {
        }

        public void Clear()
        {
        }
        
        public void Bind(string eventName, ModelEventHandler callback)
        {
        }

        public void ClearEvents()
        {
        }

        public void Unbind(string eventName, ModelEventHandler callback)
        {
        }

        public void Trigger(string eventName)
        {
        }

        public JsDictionary<string, object> ToJSON()
        { 
            return null;
        }

        [IntrinsicProperty]
        public ValidationResults Errors { get; private set; }

        [IntrinsicProperty]
        public int Cid { get; set; }
    }
}