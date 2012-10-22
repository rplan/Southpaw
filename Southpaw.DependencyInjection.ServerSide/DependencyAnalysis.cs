using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Southpaw.DependencyInjection.ClientSide;

namespace Southpaw.DependencyInjection.ServerSide
{
    public class DependencyAnalysis
    {
        public DependencyAnalysis()
        {
            Errors = new Dictionary<Type, List<string>>();
        }
        public Dictionary<Type, List<string>> Errors { get; private set; }

        public Dictionary<string, string> Analyse(string path, Dictionary<string, string> variables)
        {
            if(variables == null)
                variables = new Dictionary<string, string>();
            if (path == null)
                throw new ArgumentException("Path must be supplied to analyse");
            if (!File.Exists(path))
                throw new ArgumentException(path + " does not exist!");

            var assembly = AssemblyDefinition.ReadAssembly(path);
            var dict = GetDependencyDefinitions(assembly.MainModule.Types);
            var ret = EvaluateDependencyDefinitons(dict, variables);
            return ret;
        }

        internal static Dictionary<string, List<DependencyCandidate>> GetDependencyDefinitions(IEnumerable<TypeDefinition> types)
        {
            var dict = new Dictionary<string, List<DependencyCandidate>>();
            foreach (var type in types)
            {
                if (type.IsInterface || type.IsAbstract)
                    continue;
                // get dependencydefinition attributes (if any)
                string targetType = null;
                Dictionary<string, string> targetVars = null;
                DependencyCandidate candidate = null;
                var attr =
                    type.CustomAttributes.ToArray().FirstOrDefault(
                        ca => ca.AttributeType.FullName == typeof (DependencyDefinitionAttribute).FullName);
                if (attr != null)
                {
                    var argLen = attr.ConstructorArguments.Count;
                    if (argLen == 0)
                    {
                        targetType = GetJsTypeName(type);
                    }
                    else if (argLen == 1)
                    {
                        targetType = GetJsTypeName((TypeDefinition) attr.ConstructorArguments[0].Value);
                    }
                    else if (argLen == 2)
                    {
                        targetType = GetJsTypeName((TypeDefinition) attr.ConstructorArguments[0].Value);
                        targetVars = Utils.ParseVariables(attr.ConstructorArguments[1].Value.ToString());
                    }
                    candidate = new DependencyCandidate {TypeName = GetJsTypeName(type), Variables = targetVars};
                    if (!dict.ContainsKey(targetType))
                        dict[targetType] = new List<DependencyCandidate>();
                    dict[targetType].Add((candidate));
                }
            }
            return dict;
        }

        internal Dictionary<string, string> EvaluateDependencyDefinitons(
            Dictionary<string, List<DependencyCandidate>> candidates, Dictionary<string, string> variables)
        {
            var res = new Dictionary<string, string>();
            foreach( var candidate in candidates)
            {
                string candidateType = null;
                var candidatePoints = 0;
                foreach (var c in candidate.Value)
                {
                    var points = CalculatePoints(variables, c.Variables);
                    if (points >= candidatePoints)
                    {
                        candidateType = c.TypeName;
                        candidatePoints = points;
                    }
                }
                res[candidate.Key] = candidateType;
            }
            return res;
        }

        internal static string GetJsTypeName(Type t)
        {
            //return "$" + t.FullName.Replace(".", "_");
            return t.FullName;
        }

        internal static string GetJsTypeName(TypeDefinition t)
        {
            //return "$" + t.FullName.Replace(".", "_");
            return t.FullName;
        }

        internal static int CalculatePoints(Dictionary<string, string> compilationVars, Dictionary<string, string> candidateVars)
        {
            if (candidateVars == null)
                return 0;
            var points = 0;
            foreach(var kvp in compilationVars)
            {
                if (candidateVars.ContainsKey(kvp.Key))
                {
                    if (candidateVars[kvp.Key] != kvp.Value)
                        return -1;
                    points++;
                }
            }
            return points;
        }

    }
}