using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Southpaw.DependencyInjection.ClientSide;

namespace Southpaw.DependencyInjection.ServerSide.Tests
{
    [TestFixture]
    public class DependencyAnalysisTests
    {
        private DependencyAnalysis _analysis;

        [SetUp]
        public void SetUp()
        {
            _analysis = new DependencyAnalysis();
        }

        [Test]
        public void WithSimpleDependencyDefinition_ShouldReturnDependency()
        {
            //var res = _analysis.Analyse(new [] { typeof(MySimpleDependency)}, new Dictionary<string, string>());
            var res = _analysis.Analyse(new Uri(GetType().Assembly.CodeBase).LocalPath, new Dictionary<string, string>());
            Assert.AreEqual(1, res.Count(kvp => kvp.Key == typeof(MySimpleDependency).FullName));
            Assert.AreEqual(typeof (MySimpleDependency).FullName, res.First(kvp => kvp.Key == typeof(MySimpleDependency).FullName).Value);
            
        }
        [Test]
        public void WithLessSimpleDependencyDefinition_ShouldReturnDependency()
        {
            var res = _analysis.Analyse(new Uri(GetType().Assembly.CodeBase).LocalPath, new Dictionary<string, string>());
            Assert.AreEqual(1, res.Count(kvp => kvp.Key == typeof(IDepInterface).FullName));
            Assert.AreEqual(typeof (MyLessSimpleDependency).FullName, res.First(kvp => kvp.Key == typeof(IDepInterface).FullName).Value);
        }

        [Test]
        public void WithComplexDefinition_ShouldReturnCorrectDependency()
        {
            var res = _analysis.Analyse(new Uri(GetType().Assembly.CodeBase).LocalPath, new Dictionary<string, string>{ { "Installation", "Test1"}});
            Assert.AreEqual(1, res.Count(kvp => kvp.Key == typeof(IDepInterface2).FullName));
            Assert.AreEqual(typeof (ComplexDep1).FullName, res.First(kvp => kvp.Key == typeof(IDepInterface2).FullName).Value);
            var res2 = _analysis.Analyse(new Uri(GetType().Assembly.CodeBase).LocalPath, new Dictionary<string, string>{ { "Installation", "Test2"}});
            Assert.AreEqual(1, res2.Count(kvp => kvp.Key == typeof(IDepInterface2).FullName));
            Assert.AreEqual(typeof (ComplexDep2).FullName, res2.First(kvp => kvp.Key == typeof(IDepInterface2).FullName).Value);
        }
    }

    [DependencyDefinitionAttribute]
    class MySimpleDependency { }

    interface IDepInterface { }

    [DependencyDefinitionAttribute(typeof(IDepInterface))]
    class MyLessSimpleDependency { }

    interface IDepInterface2 { }

    [DependencyDefinitionAttribute(typeof(IDepInterface2), "Installation=Test1")]
    class ComplexDep1 { }

    [DependencyDefinitionAttribute(typeof(IDepInterface2), "Installation=Test2")]
    class ComplexDep2 { }
}
