using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Southpaw.Generator.Model;
using Southpaw.Runtime.Serverside;

namespace Southpaw.Generator.Controller
{
    public interface IGenerator
    {
        List<GeneratedFileDescriptor> Generate(Assembly assembly);
    }

    public class ServiceGenerator : IGenerator
    {
        private readonly ServiceGeneratorOptions _options;
        private readonly ViewModelGeneratorOptions _viewModelGeneratorOptions;

        public ServiceGenerator(ServiceGeneratorOptions options, ViewModelGeneratorOptions viewModelGeneratorOptions)
        {
            _options = options;
            _viewModelGeneratorOptions = viewModelGeneratorOptions;
        }

        public List<GeneratedFileDescriptor> Generate(Assembly assembly)
        {
            // validate types
            foreach (var t in assembly.GetTypes())
                ValidateControllerType(t);
            // get all controllers
            var res = new List<GeneratedFileDescriptor>();
            var baseClassesContent = new StringBuilder(@"using System;
using System.Collections.Generic;
using Southpaw.Runtime.Clientside;
using System.Runtime.CompilerServices;

");

            var superClassesUsings = @"
using Southpaw.DependencyInjection.ClientSide;

";
            Func<string, string> getServicePath = t =>
                {
                    var s = "Services." + t.Substring(_options.NamespaceSubstitution.Item2.Length);
                    //while (s.Contains('.'))
                    //s = s.Replace('.', Path.DirectorySeparatorChar);
                    s = Regex.Replace(s, "\\.", Path.DirectorySeparatorChar.ToString());
                    s += ".cs";
                    Console.WriteLine("Generated service path for " + t + ": " + s);
                    return s;
                };

            foreach (var t in assembly.GetTypes())
            {
                RegisterServiceArguments(t);
            }
            var argsClass = GenerateServiceArgs();
            if (argsClass != null)
            {
                baseClassesContent.AppendLine(argsClass);
            }

            foreach (var t in assembly.GetTypes())
            {
                var baseClass = GenerateBase(t);
                if (baseClass != null)
                {
                    baseClassesContent.AppendLine(baseClass);
                }
                var implementationClasses = GenerateSuper(t);
                if (implementationClasses != null)
                foreach (var implementationClass in implementationClasses)
                {
                    res.Add(new GeneratedFileDescriptor
                    {
                        Contents = superClassesUsings + "\r\n\r\n" + implementationClass.Value,
                        PathRelativeToSourceAssembly = getServicePath(implementationClass.Key)
                    });
                }
            }
            res.Add(new GeneratedFileDescriptor
            {
                Contents = baseClassesContent.ToString(),
                PathRelativeToSourceAssembly = "Generated\\Services\\BaseClasses.cs",
                IsForceOverwrite = true
            });
            return res;
        }

        internal bool ValidateControllerType(Type type)
        {
            if (!IsApplicableForGeneration(type))
                return true;
            var errors = new List<string>();
            foreach (var method in GetApplicableControllerMethods(type))
            {
                if (method.GetParameters().Length > 1)
                    errors.Add("Methods with multiple arguments are not supported - please wrap the arguments in a container object (method '" + method.Name + "' in class '" + type.FullName + "')");

            }
            if (errors.Count > 0)
                throw new InvalidServiceTypeException(errors.Aggregate((x, y) => x + Environment.NewLine + y));
            return true;
        }

        internal Dictionary<string, string> GenerateSuper(Type type)
        {
            if (!IsApplicableForGeneration(type))
                return null;
            var ret = new Dictionary<string, string>();
            foreach (var method in GetApplicableControllerMethods(type))
            {
                var outputWriter = new OutputWriter();
                outputWriter.Write("namespace ")
                    .Write(Utils.GetNamespace(type.Namespace, _options.NamespaceSubstitution)).EndLine()
                    .Write("{").EndLine()
                    .Indent();

                outputWriter.Write("[DependencyDefinition]")
                    .EndLine();
                outputWriter.Write("public class ").Write(GetServiceTypeName(type.Name, method.Name))
                    .Write(" : ").Write(GetServiceBaseTypeName(type.Name, method.Name)).EndLine()
                    .Write("{").EndLine()
                    .Write("}").EndLine();

                outputWriter
                    .Unindent()
                    .Write("}").EndLine().EndLine()
                    .Unindent();

                ret[Utils.GetNamespace(type.Namespace, _options.NamespaceSubstitution) + "." + GetServiceTypeName(type.Name, method.Name)] = outputWriter.ToString();
            }
            return ret;
        }

        private Dictionary<string, ServiceArguments> _argumentTypes = new Dictionary<string, ServiceArguments>();
        internal void RegisterServiceArguments(Type type)
        {
            if (!IsApplicableForGeneration(type))
                return;
            foreach (var method in GetApplicableControllerMethods(type))
            {
                var methodParameters = method.GetParameters();
                var serviceArgs = new ServiceArguments();
                foreach (var param in methodParameters)
                {
                    serviceArgs.Arguments.Add(new ServiceArgumentType
                                                  {
                                                      Name = param.Name,
                                                      Type = param.ParameterType
                                                  });
                }
                _argumentTypes.Add(type.FullName + ":" + method.Name, serviceArgs);
            }
        }

        internal string GenerateServiceArgs()
        {
            foreach (var serviceArguments in _argumentTypes)
            {
                KeyValuePair<string, ServiceArguments> arguments = serviceArguments;
                var hasOthers = _argumentTypes.Count(kvp => kvp.Value.HasArgumentsWithSameNameButDifferentTypes(arguments.Value)) > 1;
                if (arguments.Value.Arguments.Count == 0)
                    serviceArguments.Value.GeneratedTypeName = null;
                else if (arguments.Value.Arguments.Count == 1
                    && arguments.Value.Arguments[0].Name.ToLower() == "id"
                    && arguments.Value.Arguments[0].Type == typeof(int))
                // special case for Id
                {
                    serviceArguments.Value.GeneratedTypeName = "IdServiceParam";
                    var ns = serviceArguments.Key.Substring(0, serviceArguments.Key.IndexOf(':'));
                    ns = ns.Substring(0, ns.LastIndexOf('.'));
                    // TODO NCU this is incorrect - find the common base between Item1 and Item2 and use that.
                    if (_options.NamespaceSubstitution != null && _options.NamespaceSubstitution.Item1 != null)
                        ns = ns.Substring(0, _options.NamespaceSubstitution.Item1.Length);
                    serviceArguments.Value.GeneratedTypeNamespace = ns;
                }
                else
                {
                    serviceArguments.Value.GeneratedTypeName = serviceArguments.Key.Substring(serviceArguments.Key.LastIndexOf('.') + 1).Replace(":", "_") + "ServiceParam";
                    var ns = serviceArguments.Key.Substring(0, serviceArguments.Key.IndexOf(':'));
                    ns = ns.Substring(0, ns.LastIndexOf('.'));
                    serviceArguments.Value.GeneratedTypeNamespace = ns;
                }
            }
            var generated = new List<string>();
            var outputWriter = new OutputWriter();
            foreach (var serviceArguments in _argumentTypes)
            {
                if (serviceArguments.Value.Arguments.Count == 0)
                    continue;
                if (generated.Contains(serviceArguments.Value.GeneratedTypeName))
                    continue;
                outputWriter.Write("namespace ")
                    .Write(Utils.GetNamespace(serviceArguments.Value.GeneratedTypeNamespace, _options.NamespaceSubstitution)).EndLine()
                    .Write("{").EndLine()
                    .Indent();
                outputWriter
                    .Write("[IgnoreNamespace]")
                    .Write("[Imported]")
                    .Write("[ScriptName(\"Object\")]")
                    .EndLine();
                outputWriter.Write("public class ").Write(serviceArguments.Value.GeneratedTypeName).EndLine()
                    .Write("{").EndLine()
                    .Indent();
                foreach(var param in serviceArguments.Value.Arguments)
                {
                    outputWriter
                        .Write("[IntrinsicProperty]")
                        .EndLine();
                    outputWriter
                        .Write("public ")
                        .Write(GetParameterTypeForCallMethod(param.Type))
                        .Write(" ").Write(GetParameterName(param.Name))
                        .Write(" { get; set; }")
                        .EndLine()
                        .EndLine();
                }
                outputWriter
                    .Unindent()
                    .Write("}").EndLine()
                    .Unindent()
                    .Write("}").EndLine();
                generated.Add(serviceArguments.Value.GeneratedTypeName);
            }
            return outputWriter.ToString();
        }

        private string GetParameterName(string name)
        {
            var n = name[0].ToString().ToUpper();
            if (name.Length > 1)
                n += name.Substring(1);
            return n;
        }

        internal string GenerateBase(Type type)
        {
            if (!IsApplicableForGeneration(type))
                return null;
            var outputWriter = new OutputWriter();
            foreach (var method in GetApplicableControllerMethods(type))
            {
                var clientServiceAttribute = (ClientServiceAttribute)method.GetCustomAttributes(typeof(ClientServiceAttribute), true)[0];

                var returnType = clientServiceAttribute.ReturnType;
                var args = _argumentTypes[type.FullName + ":" + method.Name];

                outputWriter.Write("namespace ")
                    .Write(Utils.GetNamespace(type.Namespace, _options.NamespaceSubstitution)).EndLine()
                    .Write("{").EndLine()
                    .Indent();
                outputWriter.Write("public class ").Write(GetServiceBaseTypeName(type.Name, method.Name))
                    .Write(" : Service")
                    .Write("<");
                if (args.Arguments.Count > 0)
                    outputWriter.Write(args.GeneratedTypeName).Write(",");
                else
                    outputWriter.Write("object,");
                outputWriter.Write(Utils.GetNamespace(returnType.Namespace, _options.NamespaceSubstitution) + "." + Utils.GetViewModelTypeName(returnType)).Write(">")
                    .EndLine()
                    .Write("{").EndLine()
                    .Indent();


                // Call method
                outputWriter.Write("public override void Call(");
                if (args.Arguments.Count > 0)
                    outputWriter.Write(args.GeneratedTypeName).Write(" query");
                else
                    outputWriter.Write("object ignored = null");

                outputWriter.Write(")").EndLine()
                    .Write("{").EndLine()
                    .Indent();

                if (args.Arguments.Count > 0)
                    outputWriter.Write("DoCall(query);").EndLine();
                else
                    outputWriter.Write("DoCall();").EndLine();

                outputWriter
                    .Unindent()
                    .Write("}").EndLine();

                // AddOnCompleteCallback method
                /*
                var returnType = clientServiceAttribute.ReturnType;
                outputWriter.Write("public void AddOnCompleteCallback(Action<Dictionary<string, object>> callback)").EndLine()
                    .Write("{").EndLine()
                    .Indent()
                    .Write("DoAddOnSuccessCallback(callback);").EndLine()
                    .Unindent()
                    .Write("}").EndLine();
                 */

                // overridable HttpMethod { get; } property
                var isPost = method.GetCustomAttributes(typeof(HttpPostAttribute), true).Length != 0;
                var acceptVerbsAttributes = (AcceptVerbsAttribute)method.GetCustomAttributes(typeof(AcceptVerbsAttribute), true).FirstOrDefault();
                if (acceptVerbsAttributes != null && acceptVerbsAttributes.Verbs.Select(x => x.ToUpper()).Any(x => x == "POST"))
                    isPost = true;

                if (isPost)
                {
                    outputWriter.Write("public override string HttpMethod").EndLine()
                        .Write("{").EndLine()
                        .Indent()
                        .Write("get { return \"").Write("POST").Write("\"; }").EndLine()
                        .Unindent()
                        .Write("}").EndLine();
                }

                // overrideable InstantiateModel method
                // if using a model
                /*
                if (!Utils.PrimitiveTypes.Contains(returnType))
                {
                    outputWriter.Write("protected override object InstantiateModel(Dictionary<string, object> returnValue)").EndLine()
                        .Write("{").EndLine()
                        .Indent()
                        .Write(GetReturnTypeForInstantiateMethod(returnType)).Write(" x = new ").Write(GetReturnTypeForInstantiateMethod(returnType)).Write("();").EndLine()
                        .Write("x.Set(returnValue, null);").EndLine()
                        .Write("return x;").EndLine()
                        .Unindent()
                        .Write("}").EndLine();
                }
                 */

                outputWriter.Write("public override string GetUrl()").EndLine()
                    .Write("{").EndLine()
                    .Indent()
                    .Write("return \"/")
                        .Write(type.Name.Replace("Controller", ""))
                        .Write("/")
                        .Write(method.Name)
                        .Write("\";").EndLine()
                    .Unindent()
                    .Write("}").EndLine();

                // end class
                outputWriter
                    .Unindent()
                    .Write("}").EndLine();

                // end namespace
                outputWriter
                    .Unindent()
                    .Write("}").EndLine().EndLine();
            }
            return outputWriter.ToString();
        }

        private string GetReturnTypeForInstantiateMethod(Type type)
        {
            return GetParameterTypeForCallMethod(type);
        }

        private string GetParameterTypeForCallMethod(Type parameterType)
        {
            if (Utils.PrimitiveTypes.Contains(parameterType))
            {
                return Utils.GetPrimitiveTypeName(parameterType);
            }
            if (parameterType.Name == "Nullable`1")
            {
                var baseType = parameterType.GetGenericArguments()[0];
                if (baseType.IsEnum)
                    return "System.Nullable<" +GetParameterTypeForCallMethod(baseType) + ">";
                return GetParameterTypeForCallMethod(baseType) + "?";
            }
            return Utils.GetNamespace(parameterType.Namespace, _viewModelGeneratorOptions.NamespaceSubstitution) + "." + Utils.GetViewModelTypeName(parameterType);
        }

        #region helper methods
        internal bool IsApplicableForGeneration(Type t)
        {
            if (!typeof(System.Web.Mvc.Controller).IsAssignableFrom(t))
                return false;
            var validMethods = 0;
            foreach (var m in t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                if (m.GetCustomAttributes(typeof(ClientServiceAttribute), true).Length > 0)
                    validMethods++;
            }
            return validMethods > 0;
        }

        private string GetServiceBaseTypeName(string typeName, string methodName)
        {
            return typeName + "_" + methodName + "ServiceBase";
        }

        private string GetServiceTypeName(string typeName, string methodName)
        {
            return typeName + "_" + methodName + "Service";
        }
        #endregion

        private static IEnumerable<MethodInfo> GetApplicableControllerMethods(Type type)
        {
            //return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(m);
            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(m => m.GetCustomAttributes(typeof(ClientServiceAttribute), true).Length > 0);
        }
    }

    interface IInterface
    {

    }

    internal class ServiceArguments
    {
        public ServiceArguments()
        {
            Arguments = new List<ServiceArgumentType>();
        }

        public IList<ServiceArgumentType> Arguments { get; set; }
        public string GeneratedTypeName { get; set; }
        public string GeneratedTypeNamespace { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;
            var other = (ServiceArguments) obj;
            if (other.Arguments.Count != Arguments.Count)
                return false;
            for (var i = 0; i < other.Arguments.Count; i++)
            {
                if (!other.Arguments[i].Equals(Arguments[i]))
                    return false;
            }
            return true;
        }

        public bool HasArgumentsWithSameNameButDifferentTypes(ServiceArguments other)
        {
            if (other.Arguments.Count != Arguments.Count)
                return false;
            for (var i = 0; i < other.Arguments.Count; i++)
            {
                if (other.Arguments[i].Name == Arguments[i].Name
                    && other.Arguments[i].Type != Arguments[i].Type)
                    return true;
            }
            return false;
        }
    }

    internal class ServiceArgumentType
    {
        public Type Type { get; set; }
        public string Name { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;
            var other = (ServiceArgumentType) obj;
            return other.Type == Type && other.Name == Name;
        }
    }

}