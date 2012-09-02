using System.Html;
using jQueryApi;
using System.Runtime.CompilerServices;

namespace Southpaw.Runtime.Clientside
{
    //[IgnoreGenericArguments]
    public class ViewOptions//<TModel>
    {
        //public TModel Model { get; set; }
        public object Model { get; set; }

        public Element Element { get; set; }

        public string ElementSelector
        {
            set
            {
                Element = jQuery.Select(value)[0];
            }
        }
    }
}