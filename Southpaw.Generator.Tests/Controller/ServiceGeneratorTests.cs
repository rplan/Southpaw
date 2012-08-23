using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using NUnit.Framework;
using Southpaw.Generator.Controller;
using Southpaw.Generator.Model;
using Southpaw.Runtime.Serverside;

namespace Southpaw.Generator.Tests.Controller
{

    public class ControllerWithPrimitiveType : System.Web.Mvc.Controller
    {
        [ClientService(typeof(string))]
        public ActionResult TestGetMethod() { return null; }
    }

    public class ControllerWithModel : System.Web.Mvc.Controller
    {
        [ClientService(typeof(TestModel))]
        public ActionResult TestGetMethod() { return null; }
    }
    
    public class ControllerWithModelAndArgument : System.Web.Mvc.Controller
    {
        [ClientService(typeof(TestModel))]
        [HttpPost]
        public ActionResult TestGetMethod(ModelWithDateTimeProperty arg) { return null; }
    }
    
    public class ControllerWithMultipleMethods : System.Web.Mvc.Controller
    {
        [ClientService(typeof(string))]
        public ActionResult Method1() { return null; }

        [ClientService(typeof(string))]
        public ActionResult Method2() { return null; }
    }

    public class ControllerWithMultipleArguments : System.Web.Mvc.Controller
    {
        [ClientService(typeof(TestModel))]
        [HttpPost]
        public ActionResult Method(int a, int b) { return null; }
    }

    [TestFixture]
    public class ServiceGeneratorTests
    {
        [Test]
        public void GenerateSuper_ShouldGenerateClassInheritingFromBase()
        {
            var expected = new List<string> { @"namespace Southpaw.Tests.Output.Services
{
    public class ControllerWithPrimitiveType_TestGetMethodService : ControllerWithPrimitiveType_TestGetMethodServiceBase
    {
    }
}"};
            AssertSuperEqual(expected, typeof(ControllerWithPrimitiveType));
        }

        [Test]
        public void GenerateSuper_ShouldGenerateClassInheritingFromBaseForAllControllerMethods()
        {
            var expected = new List<string> { @"namespace Southpaw.Tests.Output.Services
{
    public class ControllerWithMultipleMethods_Method1Service : ControllerWithMultipleMethods_Method1ServiceBase
    {
    }
}",  @"namespace Southpaw.Tests.Output.Services
{
    public class ControllerWithMultipleMethods_Method2Service : ControllerWithMultipleMethods_Method2ServiceBase
    {
    }
}"};
            AssertSuperEqual(expected, typeof(ControllerWithMultipleMethods));
        }
        
        [Test]
        public void GenerateBase_WithControllerWithPrimitiveReturnType_ShouldWork()
        {
            var expected = @"namespace Southpaw.Tests.Output.Services
{
    public class ControllerWithPrimitiveType_TestGetMethodServiceBase : Service
    {
        public virtual void Call()
        {
            DoCall(null);
        }
        public override string GetUrl()
        {
            return ""/WithPrimitiveType/TestGetMethod"";
        }
    }
}";
            AssertBaseEqual(expected, typeof(ControllerWithPrimitiveType));
        }

         [Test]
        public void GenerateBase_WithControllerWithModelReturnType_ShouldWork()
        {
            var expected = @"namespace Southpaw.Tests.Output.Services
{
    public class ControllerWithModel_TestGetMethodServiceBase : Service
    {
        public virtual void Call()
        {
            DoCall(null);
        }
        public override string GetUrl()
        {
            return ""/WithModel/TestGetMethod"";
        }
    }
}";
            AssertBaseEqual(expected, typeof(ControllerWithModel));
        }
        
        
        [Test]
        public void GenerateBase_WithControllerWithModelReturnTypeAndArgument_ShouldWork()
        {
            var expected = @"namespace Southpaw.Tests.Output.Services
{
    public class ControllerWithModelAndArgument_TestGetMethodServiceBase : Service
    {
        public virtual void Call(Southpaw.Tests.Output.Models.ModelWithDateTimePropertyViewModel query)
        {
            DoCall(query);
        }
        public override string HttpMethod
        {
            get { return ""POST""; }
        }
        public override string GetUrl()
        {
            return ""/WithModelAndArgument/TestGetMethod"";
        }
    }
}";
            AssertBaseEqual(expected, typeof(ControllerWithModelAndArgument));
        }

        [Test]
        [ExpectedException(typeof(InvalidServiceTypeException))]
        public void ValidateControllerType_ControllerWithMultipleArguments_ShouldThrow()
        {
            var actual = new ServiceGenerator(
                new ServiceGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests.Controller", "Southpaw.Tests.Output.Services") },
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Tests.Output.Models") }
            ).ValidateControllerType(typeof(ControllerWithMultipleArguments));
        }
        
        [Test]
        public void ValidateControllerType_WithValidController_ShouldNotThrow()
        {
            var actual = new ServiceGenerator(
                new ServiceGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests.Controller", "Southpaw.Tests.Output.Services") },
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Tests.Output.Models") }
            ).ValidateControllerType(typeof(ControllerWithModelAndArgument));
        }

        private void AssertSuperEqual(List<string> expected, Type t)
        {
            var actual = new ServiceGenerator(
                new ServiceGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests.Controller", "Southpaw.Tests.Output.Services") },
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Tests.Output.Models") }
            ).GenerateSuper(t);
            for (var i = 0; i < actual.Count; i++)
            {
                Console.Write(actual[actual.Keys.ElementAt(i)]);
                Assert.AreEqual(expected[i], actual[actual.Keys.ElementAt(i)]);
            }
        }
        
        private void AssertBaseEqual(string expected, Type t)
        {
            var actual = new ServiceGenerator(
                new ServiceGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests.Controller", "Southpaw.Tests.Output.Services") },
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Tests.Output.Models") }
            ).GenerateBase(t);
            Console.Write(actual);
            Assert.AreEqual(expected, actual);
        }
    }
}
