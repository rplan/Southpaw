using System;
using System.Collections.Generic;
using System.Testing;
using Southpaw.Runtime.Clientside;
using jQueryApi;

namespace SampleApplication_ClientSide_Tests.BaseClassesTests.View
{
    [TestFixture]
    public class ViewModelTests
    {
        [Test]
        public void GetSetId_ShouldWork()
        {
            var v = new SimpleViewModel();
            v.Id = 12;
            Assert.AreEqual(12, v.Id);
        }

        [Test]
        public void Set_WithSilent_ShouldMarkInstanceAsChanged()
        {
            var v = new SimpleViewModel();
            var dict = new JsDictionary<string, object>();
            dict["id"] = 12;
            v.Set(dict, new ViewSetOptions { IsSilent = true});
            Assert.IsTrue(v.HasChanged());
        }

        [Test]
        public void Set_WithoutSilent_ShouldNotMarkInstanceAsChanged()
        {
            var v = new SimpleViewModel();
            var dict = new JsDictionary<string, object>();
            dict["id"] = 12;
            v.Set(dict, null);
            Assert.IsFalse(v.HasChanged());
        }

        [Test]
        public void GetSetId_ShouldTriggerChangeEventForIdProperty()
        {
            var v = new SimpleViewModel();
            var isEventTriggered = false;
            v.Bind("change:id", evt => isEventTriggered = true);
            v.Id = 12;
            Assert.IsTrue(isEventTriggered);
        }

        [Test]
        public void GetSetId_ShouldTriggerChangeEventForInstance()
        {
            var v = new SimpleViewModel();
            var isEventTriggered = false;
            v.Bind("change", evt => isEventTriggered = true);
            v.Id = 12;
            Assert.IsTrue(isEventTriggered);
        }

        [Test]
        public void Set_ShouldTriggerChangeEventForIdProperty()
        {
            var v = new SimpleViewModel();
            var isEventTriggered = false;
            v.Bind("change:id", evt => isEventTriggered = true);
            var dict = new JsDictionary<string, object>();
            dict["id"] = 12;
            v.Set(dict, null);
            Assert.IsTrue(isEventTriggered);
        }

        [Test]
        public void Set_ShouldTriggerChangeEventForInstance()
        {
            var v = new SimpleViewModel();
            var isEventTriggered = false;
            v.Bind("change", evt => isEventTriggered = true);
            var dict = new JsDictionary<string, object>();
            dict["id"] = 12;
            v.Set(dict, null);
            Assert.IsTrue(isEventTriggered);
        }

        [Test]
        public void Set_WithSilent_ShouldNotTriggerChangeEventForProperty()
        {
            var v = new SimpleViewModel();
            var isEventTriggered = false;
            v.Bind("change:id", evt => isEventTriggered = true);
            var dict = new JsDictionary<string, object>();
            dict["id"] = 12;
            v.Set(dict, new ViewSetOptions { IsSilent = true});
            Assert.IsFalse(isEventTriggered);
        }

        [Test]
        public void Set_WithSilent_ShouldNotTriggerChangeEventForInstance()
        {
            var v = new SimpleViewModel();
            var isEventTriggered = false;
            v.Bind("change", evt => isEventTriggered = true);
            var dict = new JsDictionary<string, object>();
            dict["id"] = 12;
            v.Set(dict, new ViewSetOptions { IsSilent = true});
            Assert.IsFalse(isEventTriggered);
        }

        [Test]
        public void Change_WithoutPreviousChanges_ShouldNotTriggerChangeEvent()
        {
            var v = new SimpleViewModel();
            var isEventTriggered = false;
            v.Bind("change", evt => isEventTriggered = true);
            v.Change();
            Assert.IsFalse(isEventTriggered);
        }


        [Test]
        public void Change_FollowingSetWithSilent_ShouldTriggerChangeEvent()
        {
            var v = new SimpleViewModel();
            var isEventTriggered = false;
            v.Bind("change", evt => isEventTriggered = true);
            var dict = new JsDictionary<string, object>();
            dict["id"] = 12;
            v.Set(dict, new ViewSetOptions { IsSilent = true});
            v.Change();
            Assert.IsTrue(isEventTriggered);
        }


        [Test]
        public void Unbind_ShouldUnbindListener()
        {
            var v = new SimpleViewModel();
            var isEventTriggered = false;
            ModelEventHandler listener = evt => isEventTriggered = true;
            v.Bind("change", listener);
            var dict = new JsDictionary<string, object>();
            dict["id"] = 12;
            v.Set(dict);
            Assert.IsTrue(isEventTriggered);
            isEventTriggered = false;
            v.Unbind("change", listener);
            v.Change();
            Assert.IsFalse(isEventTriggered);
        }

        [Test]
        public void Unbind_WithPropertyListener_ShouldUnbindListener()
        {
            var v = new SimpleViewModel();
            var isEventTriggered = false;
            ModelEventHandler listener = evt => isEventTriggered = true;
            v.Bind("change:id", listener);
            var dict = new JsDictionary<string, object>();
            dict["id"] = 12;
            v.Set(dict);
            Assert.IsTrue(isEventTriggered);
            isEventTriggered = false;
            v.Unbind("change:id", listener);
            dict["id"] = 13;
            v.Set(dict);
            Assert.IsFalse(isEventTriggered);
        }

        [Test]
        public void Set_SettingPropertyToSameValueAsExisting_ShouldNotTriggerInstanceChangeEvent()
        {
            var v = new SimpleViewModel();
            var isEventTriggered = false;
            ModelEventHandler listener = evt => isEventTriggered = true;
            v.Bind("change", listener);
            var dict = new JsDictionary<string, object>();
            dict["id"] = 12;
            v.Set(dict);
            Assert.IsTrue(isEventTriggered);
            isEventTriggered = false;
            v.Set(dict);
            Assert.IsFalse(isEventTriggered);
        }

        [Test]
        public void Set_SettingPropertyToSameValueAsExisting_ShouldNotTriggerPropertyChangeEvent()
        {
            var v = new SimpleViewModel();
            var isEventTriggered = false;
            ModelEventHandler listener = evt => isEventTriggered = true;
            v.Bind("change:id", listener);
            var dict = new JsDictionary<string, object>();
            dict["id"] = 12;
            v.Set(dict);
            Assert.IsTrue(isEventTriggered);
            isEventTriggered = false;
            v.Set(dict);
            Assert.IsFalse(isEventTriggered);
        }


        [Test]
        public void Set_WithSilent_ThenChange_ShouldTriggerPropertyChangeEvent()
        {
            var v = new SimpleViewModel();
            var isEventTriggered = false;
            ModelEventHandler listener = evt => isEventTriggered = true;
            v.Bind("change:id", listener);
            var dict = new JsDictionary<string, object>();
            dict["id"] = 12;
            v.Set(dict, new ViewSetOptions { IsSilent = true});
            Assert.IsFalse(isEventTriggered);
            v.Change();
            Assert.IsTrue(isEventTriggered);
        }


    }

    public class SimpleViewModel : ViewModel<int>
    {
        
    }
}

