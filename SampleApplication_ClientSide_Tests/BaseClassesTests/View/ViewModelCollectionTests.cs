using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Testing;
using System.Collections.Generic;
using System.Testing;
using SampleApplication_ClientSide;
using Southpaw.Runtime.Clientside;

namespace SampleApplication_ClientSide_Tests.BaseClassesTests.View
{
    [TestFixture]
    public class ViewModelCollectionTests
    {
        [Test]
        public void Add_ShouldWork()
        {
            bool isChangeTriggered = false, isAddTriggered = false, isAllTriggered = false;
            var c = new ViewModelCollection<SimpleModel>(typeof(SimpleModel));
            c.Bind("change", e => isChangeTriggered = true);
            c.Bind("add", e => isAddTriggered = true);
            c.Bind("all", e => isAllTriggered = true);
            c.Add(new SimpleModel {Id = 3});
            Assert.AreEqual(c.Count, 1);
            Assert.AreEqual(c.ElementAt(0).Id, 3);
            Assert.AreEqual(isChangeTriggered, true, "change event should have been triggered");
            Assert.AreEqual(isAddTriggered, true, "add event should have been triggered");
            Assert.AreEqual(isAllTriggered, true, "all event should have been triggered");
        }
        
        [Test]
        public void AddRange_ShouldWork()
        {
            int changeTriggerCount = 0, addTriggerCount = 0, allTriggerCount = 0;
            var c = new ViewModelCollection<SimpleModel>(typeof(SimpleModel));
            c.Bind("change", e => changeTriggerCount++);
            c.Bind("add", e => addTriggerCount++);
            c.Bind("all", e => allTriggerCount++);
            c.AddRange(
                new []
                    {
                        new SimpleModel {Id = 3},
                        new SimpleModel {Id = 4},
                        new SimpleModel {Id = 5}
                    }
                );

            Assert.AreEqual(c.Count, 3);
            Assert.AreEqual(c.ElementAt(0).Id, 3);
            Assert.AreEqual(c.ElementAt(1).Id, 4);
            Assert.AreEqual(c.ElementAt(2).Id, 5);
            Assert.AreEqual(changeTriggerCount, 3, "change event should have been triggered");
            Assert.AreEqual(addTriggerCount, 3, "add event should have been triggered");
            Assert.AreEqual(allTriggerCount, 3, "all event should have been triggered");
        }
        
        [Test]
        public void Remove_ShouldWork()
        {
            bool isChangeTriggered = false, isRemoveTriggered = false, isAllTriggered = false;
            var c = new ViewModelCollection<SimpleModel>(typeof(SimpleModel));
            c.Bind("all", e => isAllTriggered = true);
            c.Bind("change", e => isChangeTriggered = true);
            c.Bind("remove", e => isRemoveTriggered = true);
            var m = new SimpleModel {Id = 3};
            c.Add(m, new ViewSetOptions {IsSilent = true});
            Assert.AreEqual(c.Count, 1);
            c.Remove(m);
            Assert.AreEqual(c.Count, 0);
            Assert.AreEqual(isChangeTriggered, true, "change event should have been triggered");
            Assert.AreEqual(isRemoveTriggered, true, "remove event should have been triggered");
            Assert.AreEqual(isAllTriggered, true, "all event should have been triggered");
        }

        [Test]
        public void Remove_WithSilent_ShouldNotTriggerChangeEvent()
        {
            bool isChangeTriggered = false, isRemoveTriggered = false, isAllTriggered = false;
            var c = new ViewModelCollection<SimpleModel>(typeof(SimpleModel));
            var m = new SimpleModel {Id = 3};
            c.Add(m);
            Assert.AreEqual(c.Count, 1);
            c.Bind("all", e => isAllTriggered = true);
            c.Bind("change", e => isChangeTriggered = true);
            c.Bind("remove", e => isRemoveTriggered = true);
            c.Remove(m);
            Assert.AreEqual(c.Count, 0);
            Assert.AreEqual(isChangeTriggered, true, "change event should not have been triggered");
            Assert.AreEqual(isRemoveTriggered, true, "remove event should not have been triggered");
            Assert.AreEqual(isAllTriggered, true, "all event should not have been triggered");
        }


        [Test]
        public void RemoveAt_ShouldWork()
        {
            bool isChangeTriggered = false, isRemoveTriggered = false, isAllTriggered = false;
            var c = new ViewModelCollection<SimpleModel>(typeof(SimpleModel));
            var m = new SimpleModel {Id = 3};
            c.Add(m);
            Assert.AreEqual(c.Count, 1);
            c.Bind("all", e => isAllTriggered = true);
            c.Bind("change", e => isChangeTriggered = true);
            c.Bind("remove", e => isRemoveTriggered = true);
            c.RemoveAt(0);
            Assert.AreEqual(c.Count, 0);
            Assert.AreEqual(isChangeTriggered, true, "change event should have been triggered");
            Assert.AreEqual(isRemoveTriggered, true, "remove event should have been triggered");
            Assert.AreEqual(isAllTriggered, true, "all event should have been triggered");
        }

        [Test]
        public void ElementAt_ShouldWork()
        {
            var c = new ViewModelCollection<SimpleModel>(typeof(SimpleModel));
            var m = new SimpleModel {Id = 3};
            c.Add(m);
            Assert.AreEqual(c.Count, 1);
            Assert.AreEqual(c.ElementAt(0).Id, m.Id);
        }

        [Test]
        public void Clear_ShouldWork()
        {
            bool isClearTriggered = false, isAllTriggered = false;
            var c = new ViewModelCollection<SimpleModel>(typeof(SimpleModel));
            var m = new SimpleModel {Id = 3};
            c.Add(m);
            Assert.AreEqual(c.Count, 1);
            c.Bind("all", e => isAllTriggered = true);
            c.Bind("clear", e => isClearTriggered = true);
            c.Clear();
            Assert.AreEqual(c.Count, 0);
            Assert.AreEqual(isClearTriggered, true, "clear event should have been triggered");
            Assert.AreEqual(isAllTriggered, true, "all event should have been triggered");
        }

        [Test]
        public void Contains_ShouldWork()
        {
            var c = new ViewModelCollection<SimpleModel>(typeof(SimpleModel));
            var m = new SimpleModel {Id = 3};
            c.Add(m);
            Assert.AreEqual(c.Count, 1);
            Assert.IsTrue(c.Contains(m));
        }

        [Test]
        public void Set_ShouldWork()
        {
            var c = new ViewModelCollection<SimpleModel>(typeof(SimpleModel));
            var json = new SimpleModel {Id = 3}.ToJSON();
            var list = new List<JsDictionary<string, object>> {json};
            c.Set(list);
            Assert.AreEqual(c.Count, 1);
            Assert.AreEqual(c.ElementAt(0).Id, 3);
        }

        [Test]
        public void SetFromJSON_ShouldWork()
        {
            var c = new ViewModelCollection<BlogViewModel>(typeof(BlogViewModel));
            var json = new BlogViewModel
                           {
                               Id = 3,
                               Owner = new UserViewModel
                                           {
                                               Id = 33,
                                               FirstName = "John"
                                           }
                           }.ToJSON();
            var list = new List<JsDictionary<string, object>> {json};
            c.SetFromJSON(list);
            Assert.AreEqual(c.Count, 1);
            Assert.AreEqual(c.ElementAt(0).Id, 3);
            Assert.AreEqual(c.ElementAt(0).Owner.Id, 33);
        }

        [Test]
        public void GetById_ShouldWork()
        {
            var c = new ViewModelCollection<SimpleModel>(typeof(SimpleModel));
            var model = new SimpleModel {Id = 3};
            c.Add(model);
            Assert.AreEqual(c.Count, 1);
            var a = c.GetById(model.Id);
            Assert.AreEqual(a.Id, model.Id);
            Assert.AreEqual(a, model);
        }

        [Test]
        public void GetByCid_ShouldWork()
        {
            var c = new ViewModelCollection<SimpleModel>(typeof(SimpleModel));
            var model = new SimpleModel {Id = 3};
            c.Add(model);
            Assert.AreEqual(c.Count, 1);
            var a = c.GetByCid(model.Cid);
            Assert.AreEqual(a.Id, model.Id);
            Assert.AreEqual(a.Cid, model.Cid);
            Assert.AreEqual(a, model);
        }

        #region enumerable
        [Test]
        public void EnumerableImplementation_ShouldWork()
        {
            var c = new ViewModelCollection<SimpleModel>(typeof(SimpleModel))
                        {
                            new SimpleModel {Id = 3}, 
                            new SimpleModel {Id = 6}, 
                            new SimpleModel {Id = 10}
                        };
            var x = 0;
            foreach(var m in c)
            {
                x += m.Id;
            }
            Assert.AreEqual(x, 19);
        }

        [Test]
        public void EnumerableImplementation_ShouldWorkWithLinq()
        {
            var c = new ViewModelCollection<SimpleModel>(typeof(SimpleModel))
                        {
                            new SimpleModel {Id = 3}, 
                            new SimpleModel {Id = 6}, 
                            new SimpleModel {Id = 10}
                        };
            var x = (from m in c
                     select m.Id).Sum();
            Assert.AreEqual(x, 19);
        }
        #endregion

        [Test]
        public void ChangeEvent_OnModelInsideCollection_ShouldFireCollectionChangeEvent()
        {
            var firstModel = new SimpleModel {Id = 3};
            var c = new ViewModelCollection<SimpleModel>(typeof(SimpleModel))
                        {
                            firstModel,
                            new SimpleModel {Id = 6}, 
                            new SimpleModel {Id = 10}
                        };
            var collectionHasChanged = false;
            c.Bind("change", e => collectionHasChanged = true);
            firstModel.Trigger("change");
            Assert.IsTrue(collectionHasChanged);
        }

        [Test]
        public void ChangeEvent_OnModelInsideCollection_ShouldNotFireCollectionChangeEventAfterModelWasRemovedFromCollection()
        {
            var firstModel = new SimpleModel {Id = 3};
            var c = new ViewModelCollection<SimpleModel>(typeof(SimpleModel))
                        {
                            firstModel,
                            new SimpleModel {Id = 6}, 
                            new SimpleModel {Id = 10}
                        };
            var collectionHasChanged = false;
            c.Bind("change", e => collectionHasChanged = true);
            c.Remove(firstModel, new ViewSetOptions {IsSilent = true});
            Assert.IsFalse(collectionHasChanged, "Remove with silent should not trigger change");
            firstModel.Trigger("change");
            Assert.IsFalse(collectionHasChanged, "triggering the change event on an item that has been removed from a collection should not trigger the change event on the collection");
        }
    }

}