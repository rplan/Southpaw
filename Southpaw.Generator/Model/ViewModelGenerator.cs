using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Southpaw.Generator.Controller;

namespace Southpaw.Generator.Model
{
    [Serializable]
    public class InvalidModelTypeException : Exception
    {
        public InvalidModelTypeException()
        {
        }

        public InvalidModelTypeException(string message) : base(message)
        {
        }

        public InvalidModelTypeException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidModelTypeException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    public class ViewModelGenerator : IGenerator
    {
        private readonly ViewModelGeneratorOptions _options;
        private List<Type> _generatedTypes = new List<Type>();
        private List<Type> _enumsForGeneration = new List<Type>();
        internal List<Type> _nestedPropertyTypes = new List<Type>();

        public ViewModelGenerator(ViewModelGeneratorOptions options)
        {
            _options = options;
        }

        public List<GeneratedFileDescriptor> Generate(Assembly assembly)
        {
            StringBuilder baseClassesContent = new StringBuilder(@"using System;
using System.Collections.Generic;
using Southpaw.Runtime.Clientside;

");
            //Dictionary<Type, string> generatedSuperTypes = new Dictionary<Type, string>();
            var res = new List<GeneratedFileDescriptor>();

            // validate types
            foreach (var t in assembly.GetTypes().Where(t => t.GetCustomAttributes(typeof(HasViewModelAttribute), true).Any()))
            {
                ValidateType(assembly, t);
            }
            Func<Type, string> getViewModelPath = t =>
                {
                    //Console.WriteLine("Updating namespace for type " + t.FullName);
                    var s = t.Namespace.Substring(_options.NamespaceSubstitution.Item1.Length);
                    //while (s.Contains('.'))
                        //s = s.Replace('.', Path.DirectorySeparatorChar);
                    s = "ViewModels\\" + Regex.Replace(s, "\\.", Path.DirectorySeparatorChar.ToString());
                    s += "\\" + t.Name + "ViewModelBase.cs";
                    return s;
                };
            foreach (var t in assembly.GetTypes().Where(t => t.GetCustomAttributes(typeof(HasViewModelAttribute), true).Any()))
            {
                if (_generatedTypes.Contains(t))
                    continue;
                if (t.IsEnum)
                {
                    var x = GenerateEnum(t);
                    baseClassesContent.Append(x);
                    continue;
                }
                var baseClass = GenerateBase(t);
                var implementationClass = GenerateSuper(t);
                baseClassesContent.Append(baseClass);
                //generatedSuperTypes[t] = implementationClass;
                res.Add(new GeneratedFileDescriptor
                {
                    Contents = implementationClass,
                    PathRelativeToSourceAssembly = getViewModelPath(t)
                });
                _generatedTypes.Add(t);
            }
            while (_nestedPropertyTypes.Count != 0)
            {
                var currentNestedProperties = new List<Type>(_nestedPropertyTypes);
                _nestedPropertyTypes.Clear();
                foreach (var t in currentNestedProperties)
                {
                    if (_generatedTypes.Contains(t))
                        continue;
                    if (t.IsEnum)
                    {
                        var x = GenerateEnum(t);
                        baseClassesContent.Append(x);
                        continue;
                    }
                    var baseClass = GenerateBase(t);
                    var implementationClass = GenerateSuper(t);
                    baseClassesContent.Append(baseClass);
                    //generatedSuperTypes[t] = implementationClass;
                    res.Add(new GeneratedFileDescriptor
                    {
                        Contents = implementationClass,
                        PathRelativeToSourceAssembly = getViewModelPath(t)
                    });
                    _generatedTypes.Add(t);
                }
            }
            res.Add(new GeneratedFileDescriptor
            {
                Contents = baseClassesContent.ToString(),
                PathRelativeToSourceAssembly = "Generated\\ViewModels\\BaseClasses.cs",
                IsForceOverwrite = true
            });
            return res;
        }

        internal string GenerateEnum(Type type)
        {
            var outputWriter = new OutputWriter();
            outputWriter.Write("namespace ")
                .Write(Utils.GetNamespace(type.Namespace, _options.NamespaceSubstitution)).EndLine()
                .Write("{").EndLine()
                .Indent()
                .Write("public enum ").Write(type.Name).EndLine()
                .Write("{").EndLine()
                .Indent();
            var values = Enum.GetValues(type);
            for(var i = 0; i < values.Length; i++)
            {
                var name = Enum.GetName(type, values.GetValue(i));
                var value = Convert.ChangeType(values.GetValue(i), Enum.GetUnderlyingType(type));
                if (name.ToString() == value.ToString())
                    outputWriter.Write(name);
                else
                    outputWriter.Write(name).Write(" = ") .Write(value.ToString());
                if (i < values.Length - 1)
                    outputWriter.Write(",");
                outputWriter.EndLine();
            }
            outputWriter
                .Unindent()
                .Write("}").EndLine()
                .Unindent()
                .Write("}").EndLine().EndLine()
                .Unindent();

            return outputWriter.ToString();     
        }

        internal bool ValidateType(Assembly assembly, Type type)
        {
            var errors = new List<string>();
            Func<Type, bool> validatePropertyType = null;
            validatePropertyType = t =>
            {
                bool isValidPropertyType = false;
                if (Utils.PrimitiveTypes.Contains(t))
                    isValidPropertyType = true;
                else if (t.Name == "Nullable`1")
                {
                    var baseType = t.GetGenericArguments()[0];
                    if (Utils.PrimitiveTypes.Contains(baseType))
                        isValidPropertyType = true;
                }
                else if (t.Name == "List`1" || t.Name == "IList`1" || t.Name == "IEnumerable`1")
                {
                    var baseType = t.GetGenericArguments()[0];
                    if (validatePropertyType(baseType))
                        isValidPropertyType = true;
                }
                else if (t.Name == "Dictionary`2" || t.Name == "IDictionary`2")
                {
                    var baseType1 = t.GetGenericArguments()[0];
                    var baseType2 = t.GetGenericArguments()[1];
                    if (validatePropertyType(baseType1) && validatePropertyType(baseType2))
                        isValidPropertyType = true;
                }
                else if (t.IsArray)
                {
                    var baseType = t.GetElementType();
                    if (validatePropertyType(baseType))
                        isValidPropertyType = true;
                }
                else if (t.Name == "KeyValuePair`2")
                {
                    var baseType1 = t.GetGenericArguments()[0];
                    var baseType2 = t.GetGenericArguments()[1];
                    if (validatePropertyType(baseType1) && ValidateType(assembly, baseType2))
                        isValidPropertyType = true;
                }
                else if (assembly.GetTypes().Contains(t))
                    isValidPropertyType = true;
                return isValidPropertyType;
            };
            foreach(var p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                if (IsPropertyIgnored(p))
                    continue;
                bool isValidType = validatePropertyType(p.PropertyType);
                if (!isValidType)
                    errors.Add("Property " + p.Name + " in type " + type.FullName + " is of an invalid type (" + p.PropertyType.Name + ") - only primitive types, List<T>, or types within this assembly (" + assembly.FullName + ") can be used as property types");
            }

            if (type.BaseType != typeof(Object) && type.BaseType != typeof(Array))
            {
                if (!assembly.GetTypes().Contains(type.BaseType))
                {
                    errors.Add("Class " + type.FullName + " for property inherits from " + type.BaseType.FullName + ", which isn't a valid type to inherit from (it isn't in the same assembly)");
                }
            }
            if (errors.Count > 0)
                throw new InvalidModelTypeException(errors.Aggregate((x, y) => x + Environment.NewLine + y));
            return true;
        }


        internal string GenerateSuper(Type type)
        {
            var outputWriter = new OutputWriter();
            outputWriter.Write("namespace ")
                .Write(Utils.GetNamespace(type.Namespace, _options.NamespaceSubstitution)).EndLine()
                .Write("{").EndLine()
                .Indent()
                .Write("public class ").Write(Utils.GetViewModelTypeName(type.Name)).Write(" : ").Write(GetViewModelBaseTypeName(type.Name)).EndLine()
                .Write("{")
                .EndLine();

            outputWriter.Write("}").EndLine()
                .Unindent()
                .Write("}").EndLine().EndLine()
                .Unindent();

            return outputWriter.ToString();
        }

        internal string GenerateBase(Type type)
        {
            var outputWriter = new OutputWriter();
            outputWriter.Write("namespace ")
                .Write(Utils.GetNamespace(type.Namespace, _options.NamespaceSubstitution)).EndLine()
                .Write("{").EndLine()
                .Indent()
                .Write("public class ").Write(GetViewModelBaseTypeName(type.Name));

            if (type.BaseType != typeof(Object))
            {
                // assume this has already been validated 
                outputWriter.Write(" : ").Write(Utils.GetNamespace(type.BaseType.Namespace, _options.NamespaceSubstitution)).Write(".").Write(Utils.GetViewModelTypeName(type.BaseType.Name));
                _nestedPropertyTypes.Add(type.BaseType);
            }
            else
            {
                outputWriter.Write(" : ViewModel");
            }

            outputWriter
                .EndLine()
                .Write("{").EndLine()
                .Indent();

            // write getter/setters for individual properties
            foreach(var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (p.GetCustomAttributes(typeof(ViewModelIgnoreAttribute), true).Any())
                    continue;

                var propertyTypeName = GetPropertyTypeNameForMethodSignature(p.PropertyType);
                outputWriter.Write("public ").Write(propertyTypeName).Write(" ").Write(p.Name)
                    .EndLine()
                    .Write("{").EndLine()
                    .Indent()
                    .Write("get { return (").Write(propertyTypeName).Write(")GetProperty(\"").Write(p.Name).Write("\"); }").EndLine()
                    .Write("set { SetProperty(\"").Write(p.Name).Write("\", value); }").EndLine()
                    .Unindent()
                    .Write("}").EndLine();
                outputWriter.EndLine();
            }

            // write 'Set' method to update object from JSON
            var settableProperties = 0;
            foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (!ShouldHaveSpecialSetOverride(p))
                    continue;
                settableProperties++;
            }

            settableProperties = 1; // TODO NCU TMP
            if (settableProperties > 0)
            {
                outputWriter.Write("public bool SetFromJSON(JsDictionary<string, object> json, ViewSetOptions options)").EndLine()
                    .Write("{").EndLine()
                    .Indent()
                    .Write("if (json == null)").EndLine()
                    .Indent()
                    .Write("return true;").EndLine()
                    .Unindent();

                foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (!ShouldHaveSpecialSetOverride(p))
                        continue;

                    outputWriter.Write("if (json.ContainsKey(\"").Write(p.Name).Write("\"))").EndLine()
                        .Write("{").EndLine()
                        .Indent();
                    if (IsViewModelType(p.PropertyType))
                    {
                        // ViewModel
                        outputWriter.Write("if (this.").Write(p.Name).Write(" != null)").EndLine()
                            .Write("{").EndLine()
                            .Indent()
                            .Write("if (this.").Write(p.Name).Write(".SetFromJSON((JsDictionary<string, object>)json[\"").Write(p.Name).Write("\"], options))").EndLine()
                            .Indent()
                            .Write("json.Remove(\"").Write(p.Name).Write("\");").EndLine()
                            .Unindent()
                            .Write("else").EndLine()
                            .Indent()
                            .Write("return false;").EndLine()
                            .Unindent()
                            .Unindent()
                            .Write("}").EndLine()
                            .Write("else").EndLine()
                            .Write("{").EndLine()
                            .Indent()
                            .Write(GetPropertyTypeNameForMethodSignature(p.PropertyType)).Write(" x = new ").Write(GetPropertyTypeNameForMethodSignature(p.PropertyType)).Write("();").EndLine()
                            .Write("if (!x.SetFromJSON((JsDictionary<string, object>)json[\"").Write(p.Name).Write("\"], options))").EndLine()
                            .Indent()
                            .Write("return false;").EndLine()
                            .Unindent()
                            .Write("json[\"").Write(p.Name).Write("\"] = x;").EndLine()
                            //.Write("this.").Write(p.Name).Write(" = x;").EndLine()
                            .Unindent()
                            .Write("}").EndLine();
                    }
                    else
                    {
                        // ViewModelCollection
                        outputWriter
                            .Write(GetPropertyTypeNameForMethodSignature(p.PropertyType)).Write(" l = new ").Write(GetPropertyTypeNameForMethodSignature(p.PropertyType)).Write("();").EndLine()
                            .Write("if (this.").Write(p.Name).Write(" != null)").EndLine()
                            .Indent()
                            .Write("l = this.").Write(p.Name).Write(";").EndLine()
                            .Unindent()
                            .EndLine()
                            .Write("foreach(JsDictionary<string, object> itemJson in (List<JsDictionary<string, object>>)json[\"").Write(p.Name).Write("\"])").EndLine()
                            .Write("{").EndLine()
                            .Indent()
                            .Write(GetPropertyTypeNameForMethodSignature(p.PropertyType.GetGenericArguments()[0])).Write(" x = new ").Write(GetPropertyTypeNameForMethodSignature(p.PropertyType.GetGenericArguments()[0])).Write("();").EndLine()
                            .Write("if (!x.SetFromJSON(itemJson, options))").EndLine()
                            .Indent()
                            .Write("return false;").EndLine()
                            .Unindent()
                            .Write("l.Add(x);").EndLine()
                            .Unindent()
                            .Write("}").EndLine()
                            .Write("json[\"").Write(p.Name).Write("\"] = l;").EndLine();
                    }

                    outputWriter
                        .Unindent()
                        .Write("}").EndLine();
                }
                outputWriter.Write("return base.Set(json, options);").EndLine()
                        .Unindent()
                        .Write("}").EndLine();
            }

            // write 'SetXFromString' methods
            foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (p.GetCustomAttributes(typeof(ViewModelIgnoreAttribute), true).Any())
                    continue;

                if (IsPrimitiveOrNullableProperty(p.PropertyType))
                {
                    outputWriter.Write("public void Set").Write(p.Name).Write("FromString(string value)")
                        .EndLine()
                        .Write("{").EndLine()
                        .Indent()
                        .Write("SetPropertyFromString(\"").Write(p.Name).Write("\", value, typeof(").Write(GetPropertyTypeNameForJsTypeConversion(p.PropertyType)).Write("), ").Write(p.PropertyType.Name == "Nullable`1" ? "true" : "false").Write(");").EndLine()
                        .Unindent()
                        .Write("}").EndLine();
                    outputWriter.EndLine();
                }
                foreach (var propertyTypeForGeneration in GetPropertyTypeForGeneration(p.PropertyType))
                {
                    //Console.WriteLine("Adding nested property type " + propertyTypeForGeneration + " for property " + p.Name + " in " + type.FullName);
                    if (!_nestedPropertyTypes.Contains(propertyTypeForGeneration))
                        _nestedPropertyTypes.Add(propertyTypeForGeneration);
                }
            }

            outputWriter.Unindent()
                .Write("}").EndLine()
                .Unindent()
                .Write("}").EndLine().EndLine();

            return outputWriter.ToString();
        }

        #region helper methods
        private bool IsPropertyIgnored(PropertyInfo propertyInfo)
        {
            var attrs = propertyInfo.GetCustomAttributes(typeof(ViewModelIgnoreAttribute), true);
            if (attrs.Length > 0)
                return true;
            return false;
        }

        // return null if the type doesn't need to be generated
        internal static List<Type> GetPropertyTypeForGeneration(Type t)
        {
            if (Utils.PrimitiveTypes.Contains(t))
                return new List<Type>();
            if (t.Name == "Nullable`1")
                return new List<Type>(); // validation should have checked the base type is a primitive
            if (t.Name == "List`1" || t.Name == "IList`1" || t.Name == "IEnumerable`1")
            {
                var res = new List<Type>();
                var baseType = t.GetGenericArguments()[0];
                res.AddRange(GetPropertyTypeForGeneration(baseType));
                return res;
            }
            if (t.Name == "Dictionary`2" || t.Name == "IDictionary`2")
            {
                var res = new List<Type>();
                var baseType1 = t.GetGenericArguments()[0];
                var baseType2 = t.GetGenericArguments()[1];
                res.AddRange(GetPropertyTypeForGeneration(baseType1));
                res.AddRange(GetPropertyTypeForGeneration(baseType2));
                return res;
            }
            if (t.IsArray)
            {
                var res = new List<Type>();
                res.AddRange(GetPropertyTypeForGeneration(t.GetElementType()));
                return res;
            }
            if (t.Name == "KeyValuePair`2")
            {
                var res = new List<Type>();
                var baseType1 = t.GetGenericArguments()[0];
                var baseType2 = t.GetGenericArguments()[1];
                res.AddRange(GetPropertyTypeForGeneration(baseType1));
                res.AddRange(GetPropertyTypeForGeneration(baseType2));
                return res;
            }
            return new List<Type> { t }; // the original type - assumed to be a model, if not another (previously validated) valid type
        }

        private bool IsPrimitiveOrNullableProperty(Type t)
        {
            if (Utils.PrimitiveTypes.Contains(t))
                return true;
            if (t.Name == "Nullable`1")
            {
                var baseType = t.GetGenericArguments()[0];
                return Utils.PrimitiveTypes.Contains(baseType);
            }
            return false;
        }

        private bool IsEnumOrNullableEnumProperty(Type t)
        {
            if (t.IsEnum)
                return true;
            if (t.Name == "Nullable`1")
            {
                var baseType = t.GetGenericArguments()[0];
                return baseType.IsEnum;
            }
            return false;
        }

        private string GetViewModelBaseTypeName(string name)
        {
            return Utils.GetViewModelTypeName(name) + "Base";
        }

        private string GetPropertyTypeNameForMethodSignature(Type type)
        {
            if (Utils.PrimitiveTypes.Contains(type))
            {
                return Utils.GetPrimitiveTypeName(type);
            }
            if (type.Name == "Nullable`1")
            {
                var baseType = type.GetGenericArguments()[0];
                if (baseType == typeof(DateTime))
                    return "DateTime";
                if (baseType.IsEnum)
                    return "Nullable<" + GetPropertyTypeNameForMethodSignature(baseType) + ">";
                return GetPropertyTypeNameForMethodSignature(baseType) + "?";
            }
            if (type.Name == "List`1" || type.Name == "IList`1" || type.Name == "IEnumerable`1")
            {
                var baseType = type.GetGenericArguments()[0];
                if (IsViewModelType(baseType))
                    //return "ViewModelCollection<" + GetPropertyTypeNameForMethodSignature(baseType) + ">";
                    return "List<" + GetPropertyTypeNameForMethodSignature(baseType) + ">";
                return "List<" + GetPropertyTypeNameForMethodSignature(baseType) + ">";
            }
            if (type.Name == "Dictionary`2" || type.Name == "IDictionary`2")
            {
                var baseType1 = type.GetGenericArguments()[0];
                var baseType2 = type.GetGenericArguments()[1];
                return "Dictionary<" + GetPropertyTypeNameForJsTypeConversion(baseType1) + ", " + GetPropertyTypeNameForJsTypeConversion(baseType2) + ">";
            }
            if (type.IsArray)
            {
                return GetPropertyTypeNameForMethodSignature(type.GetElementType()) + "[]";
            }
            if (type.Name == "KeyValuePair`2")
            {
                var baseType1 = type.GetGenericArguments()[0];
                var baseType2 = type.GetGenericArguments()[1];
                return "KeyValuePair<" + GetPropertyTypeNameForMethodSignature(baseType1) + ", " + GetPropertyTypeNameForMethodSignature(baseType2) + ">";
            }
            if (type.IsEnum)
            {
                return Utils.GetNamespace(type.Namespace, _options.NamespaceSubstitution) + "." + type.Name;
            }
            return Utils.GetNamespace(type.Namespace, _options.NamespaceSubstitution) + "." + Utils.GetViewModelTypeName(type.Name);
        }
        
        private string GetPropertyTypeNameForJsTypeConversion(Type type)
        {
            if (Utils.PrimitiveTypes.Contains(type))
            {
                return Utils.GetPrimitiveTypeName(type);
            }
            if (type.Name == "Nullable`1")
            {
                var baseType = type.GetGenericArguments()[0];
                if (baseType == typeof(DateTime))
                    return "DateTime";
                return GetPropertyTypeNameForJsTypeConversion(baseType);
            }
            if (type.Name == "List`1" || type.Name == "IList`1" || type.Name == "IEnumerable`1")
            {
                var baseType = type.GetGenericArguments()[0];
                return "List<" + GetPropertyTypeNameForJsTypeConversion(baseType) + ">";
            }
            if (type.Name == "Dictionary`2" || type.Name == "IDictionary`2")
            {
                var baseType1 = type.GetGenericArguments()[0];
                var baseType2 = type.GetGenericArguments()[1];
                return "Dictionary<" + GetPropertyTypeNameForJsTypeConversion(baseType1) + ", " + GetPropertyTypeNameForJsTypeConversion(baseType2) + ">";
            }
            if (type.IsArray)
            {
                return GetPropertyTypeNameForJsTypeConversion(type.GetElementType()) + "[]";
            }
            if (type.Name == "KeyValuePair`2")
            {
                var baseType1 = type.GetGenericArguments()[0];
                var baseType2 = type.GetGenericArguments()[1];
                return "KeyValuePair<" + GetPropertyTypeNameForJsTypeConversion(baseType1) + ", " + GetPropertyTypeNameForJsTypeConversion(baseType2) + ">";
            }
            throw new InvalidProgramException("Expected primitive property type, but got " + type.FullName);
        }

        private bool IsViewModelType(Type t)
        {
            if (IsPrimitiveOrNullableProperty(t))
                return false;
            if (IsEnumOrNullableEnumProperty(t))
                return false;
            if (ShouldBeConvertedToList(t))
                return false;
            if (IsDictionary(t))
                return false;
            if (t.IsEnum)
                return false;
            if (t.Name == "KeyValuePair`2")
                return false;
            return true;
        }

        private bool IsDictionary(Type type)
        {
            if (type.Name == "Dictionary`2" || type.Name == "IDictionary`2")
                return true;
            return false;
        }

        private bool ShouldHaveSpecialSetOverride(PropertyInfo p)
        {
            if (IsViewModelType(p.PropertyType))
                return true;
            if (ShouldBeConvertedToList(p.PropertyType))
            {
                var type = p.PropertyType;
                Type baseType = null;
                if (type.Name == "List`1" || type.Name == "IList`1" || type.Name == "IEnumerable`1")
                    baseType = type.GetGenericArguments()[0];
                else if (type.IsArray)
                    baseType = type.GetElementType();
                return IsViewModelType(baseType);
            }
            return false;
        }

        private bool ShouldBeConvertedToList(Type type)
        {
            if (type.Name == "List`1" || type.Name == "IList`1" || type.Name == "IEnumerable`1")
                return true;
            if (type.IsArray)
                return true;
            return false;
        }
        #endregion
    }
}