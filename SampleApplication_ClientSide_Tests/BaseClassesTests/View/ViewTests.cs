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
            var v = new SimpleTestView(null);
            v.DoRender();
            Assert.AreEqual("<p>hello there</p>", v.GetElement().InnerHTML);
        }

        [Test]
        public void Constructor_WithElementInOptions_ShouldSetElementOnView()
        {
            var element = Document.CreateElement("span");
            var v = new SimpleTestView(new ViewOptions{Element = element});
            Assert.IsTrue(v.GetElement() == element);
            Assert.AreEqual("span", v.GetElement().TagName.ToLower());
        }

        [Test]
        public void RegisterEvents_ShouldCauseEventsToBeRegistered()
        {
            var v = new SimpleTestView(null);
            v.DoRender();
            jQuery.Select("p", v.GetElement()).Click();
            Assert.IsTrue(v._isEventCalled);
        }
    }

    public class SimpleModel : ViewModel
    {
        
    }

    public class SimpleTestView : Southpaw.Runtime.Clientside.View//<SimpleModel>
    {
        internal bool _isEventCalled;
        public SimpleTestView(ViewOptions/*<SimpleModel>*/ options) : base(options)
        {
        }

        protected override void Render(jQueryEvent evt)
        {
            Element.InnerHTML = "<p>hello there</p>";
        }

        public override void RegisterEvents()
        {
            RegisterEvent("click p", jqe => _isEventCalled = true);
        }
    }
}
