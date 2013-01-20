using System;
using System.Collections.Generic;
using System.Html;
using System.Testing;
using Southpaw.Runtime.Clientside;

namespace SampleApplication_ClientSide_Tests.BaseClassesTests.View
{
    [TestFixture]
    public class RouterTests
    {
        private Location _location;
        private Backbone.History2 _history;
        public RouterTests()
        {

            Script.Eval(@"
      window.Location = function(href) {
          this.replace(href);
      };
      _.extend(window.Location.prototype, {
        replace: function(href) {
            _.extend(this, _.pick($('<a></a>', {href: href})[0],
                'href',
                'hash',
                'host',
                'search',
                'fragment',
                'pathname',
                'protocol'
            ));
            if (!/^\//.test(this.pathname)) this.pathname = '/' + this.pathname;
        },

        toString: function() {
            return this.href;
        }
      });
");
        }

        public void Setup()
        {
            Script.Eval(@"
      this.$_location = new window.Location('http://example.com');
      Backbone.history = this.$_history = _.extend(new Backbone.History2, {location: this.$_location});
      Backbone.history.location = this.$_location;
      Backbone.history.interval = 9;
      Backbone.history.start({pushState: false});
      lastRoute = null;
      lastArgs = [];
");
        }

        public void Teardown()
        {
            Script.Eval(@"
      Backbone.history.stop();
");
        }

        [Test]
        public void BasicRouting_ShouldWork()
        {
            Setup();
            try
            {
                var counter = 0;
                var x = new TestRouter();
                x.Route("counter", e => counter++);
                x.Initialise();
                _location.Replace("http://www.example.com#counter");
                _history.CheckUrl();
                Assert.AreEqual(counter, 1, "expect counter to have incremented");
            }
            finally
            {
                Teardown();
            }
        }

        [Test]
        public void BasicRouting_WithParameters_ShouldWork()
        {
            Setup();
            try
            {
                JsDictionary<string, string> routeParams = null;
                var x = new TestRouter();
                x.Route("search/:term", e => routeParams = e);
                x.Initialise();
                _location.Replace("http://www.example.com#search/hello");
                _history.CheckUrl();
                Assert.AreEqual(routeParams["term"], "hello");
            }
            finally
            {
                Teardown();
            }
        }

        [Test]
        public void BasicRouting_WithMultipleParameters_ShouldWork()
        {
            Setup();
            try
            {
                JsDictionary<string, string> routeParams = null;
                var x = new TestRouter();
                x.Route("search/:term/p:page", e => routeParams = e);
                x.Initialise();
                _location.Replace("http://www.example.com#search/hello/p2");
                _history.CheckUrl();
                Assert.AreEqual(routeParams["term"], "hello");
                Assert.AreEqual(routeParams["page"], "2");
            }
            finally
            {
                Teardown();
            }
        }

        [Test]
        public void BasicRouting_WithMultipleParameters_ViaNavigate_ShouldWork()
        {
            Setup();
            try
            {
                JsDictionary<string, string> routeParams = null;
                var x = new TestRouter();
                x.Route("search/:term/p:page", e => routeParams = e);
                x.Initialise();
                _history.Navigate("search/hello/p2", new RouteNavigateOptions {TriggerCallback = true});
                Assert.AreEqual(routeParams["term"], "hello");
                Assert.AreEqual(routeParams["page"], "2");
            }
            finally
            {
                Teardown();
            }
        }
    }

    internal class TestRouter : Router
    {

    }
}