using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using jQueryApi;

namespace Southpaw.Runtime.Clientside
{
    public delegate void ModelEventHandler(IViewModel model);

    [IgnoreNamespace]
    [Imported]
    [ScriptName("Object")]
    public class ModelEventDetails
    {
        [IntrinsicProperty]
        public IViewModel Model { get; set; }
        [IntrinsicProperty]
        public string EventName { get; set; }
    }

    [IgnoreNamespace]
    [Imported]
    [ScriptName("Object")]
    public class ModelEventHandlerDetails
    {
        [IntrinsicProperty]
        public string EventName { get; set; }

        [IntrinsicProperty]
        public IViewModel Model  { get; set; }

        [IntrinsicProperty]
        public ModelEventHandler Handler { get; set; }
    }

    public class AddChildView<TChildView>
        where TChildView : View
    {
        [IntrinsicProperty]
        public TChildView ChildView { get; set; }

        [IntrinsicProperty]
        public View ParentView { get; set; }

        public TChildView RenderTo(Element el)
        {
            el.AppendChild(ChildView.DoRender());
            return ChildView;
        }
    }

    //[IgnoreGenericArguments]
    public abstract class View<TOptions> : View
            where TOptions : ViewOptions
    {
        protected TOptions Options;
        public virtual void Initialise(TOptions options)
        {
            Options = options;
            if (options != null && options.Element != null)
                SetElement(options.Element);
                    // TODO: maybe set when a pre-existing view is passed in, and attach a new element to the parent when that's the case?
            else
                SetElement(Document.CreateElement("div"));
            //if (options != null && options.Model != null)
            //Model = options.Model;
            //options.SetElement((Element)null);
            RegisterEvents();
            DelegateEvents();
        }

    }

    //[IgnoreGenericArguments]
    public abstract class View /*<TModel>*/ : IEvents
    {
        private static int _uniqueViewIdCounter = 0;
        private Element _element;
        private jQueryObject _jqElement;

        private readonly JsDictionary<string, List<jQueryEventHandler>> _eventHandlers =
            new JsDictionary<string, List<jQueryEventHandler>>();

        private string _uniqueId;
        private static readonly Regex EventSplitter = new Regex("^(\\S+)\\s*(.*)$");
        private readonly EventUtils _eventUtils = new EventUtils();
        private readonly List<View> _childViews = new List<View>();
        private readonly List<ModelEventHandlerDetails> _modelEventHandlers = new List<ModelEventHandlerDetails>();

        protected View()
        {
        }


        #region rendering

        public Element DoRender()
        {
            Render();
            return Element;
        }

        protected abstract void Render();

        #endregion

        #region child views

        public AddChildView<TChildView> AddChildView<TChildView>(TChildView v)
            where TChildView : View
        {
            _childViews.Add(v);
            return new AddChildView<TChildView> {ChildView = v, ParentView = this};
        }

        #endregion

        #region change listeners
        public View AddRenderOnChange(IViewModel model)
        {
            return AddChangeEventListener(model, evt => Render());
        }

        public View AddChangeEventListener(IViewModel model, ModelEventHandler handler)
        {
            return AddListenerOnModelEvent("change", model, handler);
        }

        public View AddListenerOnModelEvent(string eventName, IViewModel model, ModelEventHandler handler)
        {
            model.Bind(eventName, handler);
            _modelEventHandlers.Add(new ModelEventHandlerDetails {EventName = eventName, Model = model, Handler = handler});
            return this;
        }

        public void RemoveModelEventListeners()
        {
            foreach(var meh in _modelEventHandlers)
            {
                meh.Model.Unbind(meh.EventName, meh.Handler);
            }
        }

    #endregion

        //protected TModel Model { get; set; }

        public virtual void Remove()
        {
            RemoveModelEventListeners();
            UndelegateEvents();
            try
            {
                foreach (var v in _childViews)
                    v.Remove();
            }
            finally
            {
                JqElement.Remove();
            }
        }

        #region event delegation for actions
        public virtual void RegisterEvents()
        {
        }

        public void RegisterEvent(string selector, jQueryEventHandler handler)
        {
            if (!_eventHandlers.ContainsKey(selector))
                _eventHandlers[selector] = new List<jQueryEventHandler>();
            _eventHandlers[selector].Add(handler);
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
        #endregion

        #region get/set element
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

        protected void SetElement(Element element)
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
            get { return _jqElement ?? (_jqElement = jQuery.FromElement(Element)); }
            set { _jqElement = value; }
        }
        #endregion

        protected string UniqueId
        {
            get { return _uniqueId ?? (_uniqueId = "__View_" + (++_uniqueViewIdCounter).ToString()); }
        }

        #region IEvents implementation
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
        #endregion
    }
}
