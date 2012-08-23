using System.Html;
using jQueryApi;

namespace Southpaw.Runtime.Clientside
{
    public class ViewOptions
    {
        /* TODO: scriptsharp doesn't seem to support passing generic objects in as method arguments :-( 

        private TModel _model;

        public TModel Model
        {
            get { return _model; }
            set { _model = value; }
        }
         */
        private object _model;

        public object Model
        {
            get { return _model; }
            set { _model = value; }
        }

        private Element _element;
        public Element Element
        {
            get
            {
                return _element;
            }
            private set
            {
                _element = value;
            }
        }
        public void SetElement(Element element)
        {
            Element = element;
        }

        public void SetElementSelector(string elementSelector)
        {
            Element = jQuery.Select(elementSelector)[0];
        }
    }
}