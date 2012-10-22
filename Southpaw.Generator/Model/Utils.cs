using System;
using System.Collections.Generic;

namespace Southpaw.Generator.Model
{
    public class Utils
    {
        public static string GetNamespace(string originalNamespace, Tuple<string, string> namespaceSubstitution)
        {
            return originalNamespace.Replace(namespaceSubstitution.Item1, namespaceSubstitution.Item2);
        }

        public static readonly List<Type> PrimitiveTypes = new List<Type>
                                                               {
                                                                    typeof(int),
                                                                    typeof(float),
                                                                    typeof(double),
                                                                    typeof(string),
                                                                    typeof(DateTime),
                                                                    typeof(decimal),
                                                                    typeof(bool),
                                                                    typeof(char),
                                                                    typeof(short),
                                                                    typeof(long),
                                                                };

        internal static string GetPrimitiveTypeName(Type type)
        {
            if (type == typeof (DateTime))
                return "DateTime"; 
            if (type == typeof (Int32))
                return "int";
            if (type == typeof (Int64))
                return "long";
            if (type == typeof (Int16))
                return "short";
            if (type == typeof (Boolean))
                return "bool";
            return type.Name.ToLower();
        }

        public static string GetViewModelTypeName(Type originalModelType)
        {
            var n = originalModelType.Name;
            var isCorLib = originalModelType.Namespace.StartsWith("System");
            if (originalModelType.IsGenericType)
            {
                n = n.Substring(0, n.IndexOf('`'));
                n += "<";
                foreach (var gt in originalModelType.GetGenericArguments())
                    n += GetViewModelTypeName(gt) + ",";
                n = n.Substring(0, n.Length - 1);
                n += ">";
                return n;
            }
            return n + (isCorLib ? "" : "ViewModel");
        }
    }
}