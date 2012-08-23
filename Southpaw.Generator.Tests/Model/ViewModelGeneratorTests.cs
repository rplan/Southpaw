using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Southpaw.Generator.Model;

namespace Southpaw.Generator.Tests
{
    public enum TestEnum
    {
        Hello,
        There
    }
    public enum TestOtherEnum
    {
        Hello = 10,
        There = 20
    }
    public class TestModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
    }
    
    public class ModelWithNullableProperty
    {
        public int? Age { get; set; }
    }

    public class ModelWithDateTimeProperty
    {
        public DateTime Prop{ get; set; }
    }
    
    public class ModelWithInvalidPropertyType
    {
        public Uri Prop{ get; set; }
    }
    
    public class ModelWithNestedAssemblyPropertyType
    {
        public TestModel Prop{ get; set; }
    }

    public class ModelInheritingFromAnother : ModelWithDateTimeProperty
    {
        public string Prop2 { get; set; }
    }
    
    public class ModelWithIgnoredProperty
    {
        [ViewModelIgnore]
        public string Prop{ get; set; }
    }

    public class ModelWithInvalidIgnoredProperty
    {
        [ViewModelIgnore]
        public Uri Prop{ get; set; }
    }

    public class ModelInheritingFromAnInvalidType : ArrayList
    {
    }

    public class ModelWithListProperty
    {
        public IList<string> Prop { get; set; }
    }

    public class ModelWithListOfModelsProperty
    {
        public IList<ModelWithDateTimeProperty> Prop { get; set; }
    }

    public struct TestStruct { }
    public class ModelWithInvalidNullableProperty
    {
        public Nullable<TestStruct> Prop { get; set; }
    }

    public class ModelWithArrayProperty
    {
        public int[] Prop { get; set; }
    }

    public class ModelWithNullableArrayProperty
    {
        public decimal?[] Prop { get; set; }
    }

    public class ModelWithInvalidArrayProperty
    {
        public Uri[] Prop { get; set; }
    }

    public class ModelWithEnumProperty
    {
        public TestEnum Prop { get; set; }
    }

    public class ModelWithInvalidEnumProperty
    {
        public RegexOptions Prop { get; set; }
    }

    public class ModelWithDictionaryProperty
    {
        public Dictionary<int, string> Prop { get; set; } 
    }

    public class ModelWithDictionaryWithNullableValueProperty
    {
        public Dictionary<int, decimal?> Prop { get; set; } 
    }

    public class ModelWithDictionaryWithNullableArrayValue
    {
        public Dictionary<int, decimal?[]> Prop { get; set; } 
    }

    public class ModelWithEnumerableKeyValuePairWithNullableArrayValue
    {
        public IEnumerable<KeyValuePair<int, decimal?[]>> Prop { get; set; } 
    }

    
    
    [TestFixture]
    public class ViewModelGeneratorTests
    {
        [Test]
        public void GenerateSuper_WithSimpleClass_ShouldGenerateEmptyClassInheritingFromBase()
        {
            var actual = new ViewModelGenerator(new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") }).GenerateSuper(typeof(TestModel));
            var expected = @"namespace Southpaw.Output.Tests
{
    public class TestModelViewModel : TestModelViewModelBase
    {
    }
}";
            Console.Write(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateBase_WithSimpleClass_ShouldWork()
        {
            var actual = new ViewModelGenerator(new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") }).GenerateBase(typeof(TestModel));
            var expected = @"namespace Southpaw.Output.Tests
{
    public class TestModelViewModelBase : ViewModel
    {
        public string Name
        {
            get { return (string)GetProperty(""Name""); }
            set { SetProperty(""Name"", value); }
        }

        public string Email
        {
            get { return (string)GetProperty(""Email""); }
            set { SetProperty(""Email"", value); }
        }

        public int Id
        {
            get { return (int)GetProperty(""Id""); }
            set { SetProperty(""Id"", value); }
        }

        public bool SetFromJSON(Dictionary<string, object> json, ViewSetOptions options)
        {
            if (json == null)
                return true;
            return base.Set(json, options);
        }
        public void SetNameFromString(string value)
        {
            SetPropertyFromString(""Name"", value, typeof(string), false);
        }

        public void SetEmailFromString(string value)
        {
            SetPropertyFromString(""Email"", value, typeof(string), false);
        }

        public void SetIdFromString(string value)
        {
            SetPropertyFromString(""Id"", value, typeof(int), false);
        }

    }
}";
            Console.Write(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateBase_ClassWithNullableProperty_ShouldSetThePropertyAsNullable()
        {
            var actual = new ViewModelGenerator(new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") }).GenerateBase(typeof(ModelWithNullableProperty));
            var expected = @"namespace Southpaw.Output.Tests
{
    public class ModelWithNullablePropertyViewModelBase : ViewModel
    {
        public int? Age
        {
            get { return (int?)GetProperty(""Age""); }
            set { SetProperty(""Age"", value); }
        }

        public bool SetFromJSON(Dictionary<string, object> json, ViewSetOptions options)
        {
            if (json == null)
                return true;
            return base.Set(json, options);
        }
        public void SetAgeFromString(string value)
        {
            SetPropertyFromString(""Age"", value, typeof(int), true);
        }

    }
}";
            Console.Write(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateBase_ClassWithDateTimeProperty_ShouldConvertThePropertyToTheDateType()
        {
            var actual = new ViewModelGenerator(
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .GenerateBase(typeof(ModelWithDateTimeProperty));
            var expected = @"namespace Southpaw.Output.Tests
{
    public class ModelWithDateTimePropertyViewModelBase : ViewModel
    {
        public Date Prop
        {
            get { return (Date)GetProperty(""Prop""); }
            set { SetProperty(""Prop"", value); }
        }

        public bool SetFromJSON(Dictionary<string, object> json, ViewSetOptions options)
        {
            if (json == null)
                return true;
            return base.Set(json, options);
        }
        public void SetPropFromString(string value)
        {
            SetPropertyFromString(""Prop"", value, typeof(Date), false);
        }

    }
}";
            Console.Write(actual);
            Assert.AreEqual(expected, actual);
        }
        
        
        [Test]
        public void GenerateBase_ClassWithPropertyTypeInTheSameAssembly_ShouldUseViewModelAsAReference()
        {
            var actual = new ViewModelGenerator(
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .GenerateBase(typeof(ModelWithDateTimeProperty));
            var expected = @"namespace Southpaw.Output.Tests
{
    public class ModelWithDateTimePropertyViewModelBase : ViewModel
    {
        public Date Prop
        {
            get { return (Date)GetProperty(""Prop""); }
            set { SetProperty(""Prop"", value); }
        }

        public bool SetFromJSON(Dictionary<string, object> json, ViewSetOptions options)
        {
            if (json == null)
                return true;
            return base.Set(json, options);
        }
        public void SetPropFromString(string value)
        {
            SetPropertyFromString(""Prop"", value, typeof(Date), false);
        }

    }
}";
            Console.Write(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateBase_ClassWithPropertyTypeInTheSameAssembly_ShouldUseGeneratedViewModelType()
        {
            var actual = new ViewModelGenerator(new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .GenerateBase(typeof(ModelWithNestedAssemblyPropertyType));
            var expected = @"namespace Southpaw.Output.Tests
{
    public class ModelWithNestedAssemblyPropertyTypeViewModelBase : ViewModel
    {
        public Southpaw.Output.Tests.TestModelViewModel Prop
        {
            get { return (Southpaw.Output.Tests.TestModelViewModel)GetProperty(""Prop""); }
            set { SetProperty(""Prop"", value); }
        }

        public bool SetFromJSON(Dictionary<string, object> json, ViewSetOptions options)
        {
            if (json == null)
                return true;
            if (json.ContainsKey(""Prop""))
            {
                if (this.Prop != null)
                {
                    if (this.Prop.SetFromJSON((Dictionary<string, object>)json[""Prop""], options))
                        json.Remove(""Prop"");
                    else
                        return false;
                }
                else
                {
                    Southpaw.Output.Tests.TestModelViewModel x = new Southpaw.Output.Tests.TestModelViewModel();
                    if (!x.SetFromJSON((Dictionary<string, object>)json[""Prop""], options))
                        return false;
                    json[""Prop""] = x;
                }
            }
            return base.Set(json, options);
        }
    }
}";
            Console.Write(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateBase_ClassWithPropertyTypeInTheSameAssembly_ShouldRecordNestedClassForGeneration()
        {
            var generator = new ViewModelGenerator(new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") });
            generator.GenerateBase(typeof(ModelWithNestedAssemblyPropertyType));
            Assert.Contains(typeof(TestModel), generator._nestedPropertyTypes);
        }

        [Test]
        public void GenerateBase_ClassInheritingFromAnother_ShouldRecordNestedClassForGeneration()
        {
            var generator = new ViewModelGenerator(new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") });
            generator.GenerateBase(typeof(ModelInheritingFromAnother));
            Assert.Contains(typeof(ModelWithDateTimeProperty), generator._nestedPropertyTypes);
        }
        
        
        [Test]
        public void GenerateBase_ClassInheritingFromAnother_ShouldSetUpInheritanceChainCorrectly()
        {
            var actual = new ViewModelGenerator(new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .GenerateBase(typeof(ModelInheritingFromAnother));
            var expected = @"namespace Southpaw.Output.Tests
{
    public class ModelInheritingFromAnotherViewModelBase : Southpaw.Output.Tests.ModelWithDateTimePropertyViewModel
    {
        public string Prop2
        {
            get { return (string)GetProperty(""Prop2""); }
            set { SetProperty(""Prop2"", value); }
        }

        public bool SetFromJSON(Dictionary<string, object> json, ViewSetOptions options)
        {
            if (json == null)
                return true;
            return base.Set(json, options);
        }
        public void SetProp2FromString(string value)
        {
            SetPropertyFromString(""Prop2"", value, typeof(string), false);
        }

    }
}";
            Console.Write(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateBase_ClassWithIgnoredProperty_ShouldNotIncludeTheGeneratedPropertyInTheOutput()
        {
            var actual = new ViewModelGenerator(new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .GenerateBase(typeof(ModelWithIgnoredProperty));
            var expected = @"namespace Southpaw.Output.Tests
{
    public class ModelWithIgnoredPropertyViewModelBase : ViewModel
    {
        public bool SetFromJSON(Dictionary<string, object> json, ViewSetOptions options)
        {
            if (json == null)
                return true;
            return base.Set(json, options);
        }
    }
}";
            Console.Write(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateBase_ClassWithListProperty_ShouldWork()
        {
            var actual = new ViewModelGenerator(new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .GenerateBase(typeof(ModelWithListProperty));
            var expected = @"namespace Southpaw.Output.Tests
{
    public class ModelWithListPropertyViewModelBase : ViewModel
    {
        public List<string> Prop
        {
            get { return (List<string>)GetProperty(""Prop""); }
            set { SetProperty(""Prop"", value); }
        }

        public bool SetFromJSON(Dictionary<string, object> json, ViewSetOptions options)
        {
            if (json == null)
                return true;
            return base.Set(json, options);
        }
    }
}";
            Console.Write(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateBase_ClassWithListOfModelsProperty_ShouldWork()
        {
            var actual = new ViewModelGenerator(new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .GenerateBase(typeof(ModelWithListOfModelsProperty));
            var expected = @"namespace Southpaw.Output.Tests
{
    public class ModelWithListOfModelsPropertyViewModelBase : ViewModel
    {
        public List<Southpaw.Output.Tests.ModelWithDateTimePropertyViewModel> Prop
        {
            get { return (List<Southpaw.Output.Tests.ModelWithDateTimePropertyViewModel>)GetProperty(""Prop""); }
            set { SetProperty(""Prop"", value); }
        }

        public bool SetFromJSON(Dictionary<string, object> json, ViewSetOptions options)
        {
            if (json == null)
                return true;
            if (json.ContainsKey(""Prop""))
            {
                List<Southpaw.Output.Tests.ModelWithDateTimePropertyViewModel> l = new List<Southpaw.Output.Tests.ModelWithDateTimePropertyViewModel>();
                if (this.Prop != null)
                    l = this.Prop;

                foreach(Dictionary<string, object> itemJson in (List<Dictionary<string, object>>)json[""Prop""])
                {
                    Southpaw.Output.Tests.ModelWithDateTimePropertyViewModel x = new Southpaw.Output.Tests.ModelWithDateTimePropertyViewModel();
                    if (!x.SetFromJSON(itemJson, options))
                        return false;
                    l.Add(x);
                }
                json[""Prop""] = l;
            }
            return base.Set(json, options);
        }
    }
}";
            Console.Write(actual);
            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public void GenerateBase_ClassWithEnumProperty_ShouldWork()
        {
            var actual = new ViewModelGenerator(new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .GenerateBase(typeof(ModelWithEnumProperty));
            var expected = @"namespace Southpaw.Output.Tests
{
    public class ModelWithEnumPropertyViewModelBase : ViewModel
    {
        public Southpaw.Output.Tests.TestEnum Prop
        {
            get { return (Southpaw.Output.Tests.TestEnum)GetProperty(""Prop""); }
            set { SetProperty(""Prop"", value); }
        }

        public bool SetFromJSON(Dictionary<string, object> json, ViewSetOptions options)
        {
            if (json == null)
                return true;
            return base.Set(json, options);
        }
    }
}";
            Console.Write(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateEnum_ShouldWork()
        {
            var actual = new ViewModelGenerator(new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .GenerateEnum(typeof(TestEnum));
            var expected = @"namespace Southpaw.Output.Tests
{
    public enum TestEnum
    {
        Hello = 0,
        There = 1
    }
}";
            Console.Write(actual);
            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public void GenerateEnum_WithExplicitValues_ShouldWork()
        {
            var actual = new ViewModelGenerator(new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .GenerateEnum(typeof(TestOtherEnum));
            var expected = @"namespace Southpaw.Output.Tests
{
    public enum TestOtherEnum
    {
        Hello = 10,
        There = 20
    }
}";
            Console.Write(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [ExpectedException(typeof(InvalidModelTypeException))]
        public void ValidateType_ClassWithInvalidPropertyType_ShouldThrow()
        {
            var actual = new ViewModelGenerator(
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .ValidateType(this.GetType().Assembly, typeof(ModelWithInvalidPropertyType));
        }
        
        [Test]
        public void ValidateType_ClassWithPrimitivePropertyType_ShouldNotThrow()
        {
            var actual = new ViewModelGenerator(
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .ValidateType(this.GetType().Assembly, typeof(ModelWithDateTimeProperty));
        }

        [Test]
        public void ValidateType_ClassWithPropertyTypeInTheSameAssembly_ShouldNotThrow()
        {
            var actual = new ViewModelGenerator(
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .ValidateType(this.GetType().Assembly, typeof(ModelWithNestedAssemblyPropertyType));
        }

        [Test]
        public void ValidateType_ClassWithIgnoredInvalidProperty_ShouldNotThrow()
        {
            var actual = new ViewModelGenerator(
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .ValidateType(this.GetType().Assembly, typeof(ModelWithInvalidIgnoredProperty));
        }
        
        [Test]
        public void ValidateType_ClassNullableTypeInTheSameAssembly_ShouldNotThrow()
        {
            var actual = new ViewModelGenerator(
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .ValidateType(this.GetType().Assembly, typeof(ModelWithNullableProperty));
        }

        [Test]
        [ExpectedException(typeof(InvalidModelTypeException))]
        public void ValidateType_ClassInheritingFromAnInvalidType_ShouldThrow()
        {
            var actual = new ViewModelGenerator(
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .ValidateType(this.GetType().Assembly, typeof(ModelInheritingFromAnInvalidType));
        }

        [Test]
        [ExpectedException(typeof(InvalidModelTypeException))]
        public void ValidateType_ModelWithInvalidNullableProperty_ShouldThrow()
        {
            var actual = new ViewModelGenerator(
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .ValidateType(this.GetType().Assembly, typeof(ModelWithInvalidNullableProperty));
        }

        [Test]
        [ExpectedException(typeof(InvalidModelTypeException))]
        public void ValidateType_ModelWithInvalidArrayProperty_ShouldThrow()
        {
            var actual = new ViewModelGenerator(
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .ValidateType(this.GetType().Assembly, typeof(ModelWithInvalidArrayProperty));
        }

        [Test]
        public void ValidateType_ModelWithValidArrayProperty_ShouldNotThrow()
        {
            var actual = new ViewModelGenerator(
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .ValidateType(this.GetType().Assembly, typeof(ModelWithArrayProperty));
        }

        [Test]
        public void ValidateType_ModelWithValidNullableArrayProperty_ShouldNotThrow()
        {
            var actual = new ViewModelGenerator(
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .ValidateType(this.GetType().Assembly, typeof(ModelWithNullableArrayProperty));
        }

        [Test]
        public void ValidateType_ModelWithValidDictionaryProperty_ShouldNotThrow()
        {
            var actual = new ViewModelGenerator(
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .ValidateType(this.GetType().Assembly, typeof(ModelWithDictionaryProperty));
        }

        [Test]
        public void ValidateType_ModelWithValidDictionaryWithNullableValueProperty_ShouldNotThrow()
        {
            var actual = new ViewModelGenerator(
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .ValidateType(this.GetType().Assembly, typeof(ModelWithDictionaryWithNullableValueProperty));
        }

        [Test]
        public void ValidateType_ModelWithValidDictionaryWithNullableArrayValueProperty_ShouldNotThrow()
        {
            var actual = new ViewModelGenerator(
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .ValidateType(this.GetType().Assembly, typeof(ModelWithDictionaryWithNullableArrayValue));
        }
        
        [Test]
        public void ValidateType_ModelWithEnumerableKeyValuePairWithNullableArrayValue_ShouldNotThrow()
        {
            var actual = new ViewModelGenerator(
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") })
                .ValidateType(this.GetType().Assembly, typeof(ModelWithEnumerableKeyValuePairWithNullableArrayValue));
        }

        [TestCase(typeof(int), "int")]
        [TestCase(typeof(Int32), "int")]
        [TestCase(typeof(Int64), "long")]
        [TestCase(typeof(Int16), "short")]
        [TestCase(typeof(bool), "bool")]
        [TestCase(typeof(Boolean), "bool")]
        [TestCase(typeof(DateTime), "Date")]
        [TestCase(typeof(string), "string")]
        [TestCase(typeof(String), "string")]
        public void GetPrimitiveTypeName_ShouldMapCorrectly(Type primitive, string expected)
        {
            Assert.AreEqual(expected, Utils.GetPrimitiveTypeName(primitive));
        }

        [Test]
        public void GenerateBase_ClassWithListOfModelsProperty_ShouldAddModelInListForGeneration()
        {
            var generator = new ViewModelGenerator(new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>("Southpaw.Generator.Tests", "Southpaw.Output.Tests") });
            generator.GenerateBase(typeof(ModelWithListOfModelsProperty));
            Assert.AreEqual(1, generator._nestedPropertyTypes.Count);
            Assert.AreEqual(typeof(ModelWithDateTimeProperty), generator._nestedPropertyTypes[0]);
        }

        [Test]
        public void GetPropertyTypeForGeneration_ShouldNotReturnNullableTypes()
        {
            Assert.AreEqual(ViewModelGenerator.GetPropertyTypeForGeneration(typeof(Nullable<int>)).Count, 0);
        }

    }

}