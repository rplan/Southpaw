using System.Collections.Generic;
using System.Html;
using jQueryApi;
using System.Runtime.CompilerServices;

namespace Southpaw.Runtime.Clientside
{
    [Imported(IsRealType = true)]
    public class ViewOptions<TModel> : ViewOptions
    {
        [IntrinsicProperty]
        public TModel Model { get; set; }
    }

    [Imported(IsRealType = true)]
    public class ViewOptions
    {

        [IntrinsicProperty]
        public Element Element { get; set; }

        public string ElementSelector
        {
            set
            {
                Element = jQuery.Select(value)[0];
            }
        }

        public JsDictionary<string, object> ToJSON()
        {
            return null;
        }
    }
}