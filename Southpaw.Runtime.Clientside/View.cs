using System;
using System.Collections.Generic;
using System.Html;
using System.Text.RegularExpressions;
using jQueryApi;
using System.Runtime.CompilerServices;

namespace Southpaw.Runtime.Clientside
{
    //[IgnoreGenericArguments]
    public abstract class View/*<TModel>*/ : IEvents
    {
        private static int UniqueViewIdCounter = 0;
        private Element _element;
        private jQueryObject _jqElement;
        private readonly Dictionary<string, List<jQueryEventHandler>> _eventHandlers = new Dictionary<string, List<jQueryEventHandler>>();
        private string _uniqueId;
        private static Regex EventSplitter = new Regex("^(\\S+)\\s*(.*)$");
        private readonly EventUtils _eventUtils = new EventUtils();

        public View(ViewOptions/*<TModel>*/ options)
        {
            if (options != null && options.Element != null)
                SetElement(options.Element); // TODO: maybe set when a pre-existing view is passed in, and attach a new element to the parent when that's the case?
            else
                SetElement(Document.CreateElement("div"));
            //if (options != null && options.Model != null)
                //Model = options.Model;
            //options.SetElement((Element)null);
            RegisterEvents();
            DelegateEvents();
        }


        public abstract void RegisterEvents();

        public void RegisterEvent(string selector, jQueryEventHandler handler)
        {
            if (!_eventHandlers.ContainsKey(selector))
                _eventHandlers[selector] = new List<jQueryEventHandler>();
            _eventHandlers[selector].Add(handler);
        }

        public Element DoRender()
        {
            Render(null);
            return Element;
        }

        protected abstract void Render(jQueryEvent evt);

        //protected TModel Model { get; set; }

        public virtual void Remove()
        {
            JqElement.Remove();
        }

        protected void DelegateEvents()
        {
            if (_eventHandlers.Count == 0)
                return;
            UndelegateEvents();
            foreach (KeyValuePair<string, List<jQueryEventHandler>> kvp in _eventHandlers)
            {
                string[] split = EventSplitter.Exec(kvp.Key);
                string eventName = split[1] + ".delegateEvents" + UniqueId;
                string selector = split[2];
                foreach (jQueryEventHandler action in kvp.Value)
                {
                    if (selector == "")
                    {
                        JqElement.Bind(eventName, action);
                    }
                    else
                    {
                        JqElement.Delegate(selector, eventName, action);
                    }
                }
            }
        }

        protected void UndelegateEvents()
        {
            if (_element != null)
                JqElement.Unbind(".delegateEvents" + UniqueId);
        }

        protected Element Element
        {
            get { return _element; }
            set {
                UndelegateEvents();
                _element = value;
                _jqElement = null;
                DelegateEvents();
            }
        }

        private void SetElement(Element element)
        {
            Element = element;
            _jqElement = null;
        }

        public void SetElementSelector(string elementSelector)
        {
            Element = jQuery.Select(elementSelector)[0];
        }

        /// <summary>
        /// Gets the DOM element containing the HTML for the view. Note that no references should be maintained to the DOM element outside the view,
        /// as the view may replace the DOM element on render.
        /// </summary>
        /// <returns></returns>
        public Element GetElement()
        {
            return _element;
        }

        protected jQueryObject JqElement
        {
            get {
                if (_jqElement == null)
                    _jqElement = jQuery.FromElement(Element);
                return _jqElement; 
            }
            set { _jqElement = value; }
        }

        protected string UniqueId
        {
            get
            {
                if (_uniqueId == null)
                    _uniqueId = (++UniqueViewIdCounter).ToString();
                return _uniqueId;
            }
        }

        public void Bind(string eventName, jQueryEventHandler callback)
        {
            _eventUtils.Bind(eventName, callback);
        }

        public void ClearEvents()
        {
            _eventUtils.Clear();
        }

        public void Unbind(string eventName, jQueryEventHandler callback)
        {
            _eventUtils.Unbind(eventName, callback);
        }

        public void Trigger(string eventName, jQueryEvent evt)
        {
            _eventUtils.Trigger(eventName, evt);
        }
    }
}
