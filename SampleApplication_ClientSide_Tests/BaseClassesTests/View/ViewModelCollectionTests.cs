using System;
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
            var c = new ViewModelCollection<SimpleModel>(typeof(SimpleModel));
            c.Add(new SimpleModel {Id = 3});
            Assert.AreEqual(c.Count, 1);
            Assert.AreEqual(c.ElementAt(0).Id, 3);
        }
        
        [Test]
        public void AddRange_ShouldWork()
        {
            var c = new ViewModelCollection<SimpleModel>(typeof(SimpleModel));
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
        }
        
        [Test]
        public void Remove_ShouldWork()
        {
            var c = new ViewModelCollection<SimpleModel>(typeof(SimpleModel));
            var m = new SimpleModel {Id = 3};
            c.Add(m);
            Assert.AreEqual(c.Count, 1);
            c.Remove(m);
            Assert.AreEqual(c.Count, 0);
        }

        [Test]
        public void RemoveAt_ShouldWork()
        {
            var c = new ViewModelCollection<SimpleModel>(typeof(SimpleModel));
            var m = new SimpleModel {Id = 3};
            c.Add(m);
            Assert.AreEqual(c.Count, 1);
            c.RemoveAt(0);
            Assert.AreEqual(c.Count, 0);
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
            var c = new ViewModelCollection<SimpleModel>(typeof(SimpleModel));
            var m = new SimpleModel {Id = 3};
            c.Add(m);
            Assert.AreEqual(c.Count, 1);
            c.Clear();
            Assert.AreEqual(c.Count, 0);
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

    }

}