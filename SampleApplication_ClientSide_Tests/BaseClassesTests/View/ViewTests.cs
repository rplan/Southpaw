using System.Collections.Generic;
using jQueryApi;
using Southpaw.Runtime.Clientside;
using System.Testing;
using System.Html;

namespace SampleApplication_ClientSide_Tests.BaseClassesTests.View
{
    [TestFixture]
    public class ViewTests
    {
        [Test]
        public void Render_ShouldWork()
        {
            var v = new SimpleTestView();
            v.Initialise(null);
            v.DoRender();
            Assert.AreEqual("<p>hello there</p>", v.GetElement().InnerHTML);
        }

        [Test]
        public void Constructor_WithElementInOptions_ShouldSetElementOnView()
        {
            var element = Document.CreateElement("span");
            var v = new SimpleTestView();
            v.Initialise(new SimpleTestViewOptions{Element = element});
            Assert.IsTrue(v.GetElement() == element);
            Assert.AreEqual("span", v.GetElement().TagName.ToLower());
        }

        [Test]
        public void RegisterEvents_ShouldCauseEventsToBeRegistered()
        {
            var v = new SimpleTestView();
            v.Initialise(null);
            v.DoRender();
            jQuery.Select("p", v.GetElement()).Click();
            Assert.IsTrue(v._isEventCalled);
        }
    }

    public class SimpleModel : ViewModel<int>
    {
        public override bool SetFromJSON(JsDictionary<string, object> json, ViewSetOptions options)
        {
            return Set(json);
        }
    }

    public class SimpleTestView : Southpaw.Runtime.Clientside.View<SimpleTestViewOptions>
    {
        internal bool _isEventCalled;
        public SimpleTestView()
        {
        }

        protected override void Render()
        {
            Element.InnerHTML = "<p>hello there</p>";
        }

        public override void RegisterEvents()
        {
            RegisterEvent("click p", jqe => _isEventCalled = true);
        }
    }

    public class SimpleTestViewOptions : ViewOptions
    {
    }
}
