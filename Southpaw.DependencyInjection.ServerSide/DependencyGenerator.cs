using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Southpaw.DependencyInjection.ServerSide
{
    //public class DependencyGenerator :  AppDomainIsolatedTask
    public class DependencyGenerator :  Task
    {
        [Required]
        public string Dll { get; set; }
        [Required]
        public string OutputFile { get; set; }
        [Required]
        public string Variables { get; set; }

        public override bool Execute()
        {
            if (!File.Exists(Dll))
            {
                Log.LogError(Dll + " does not exist!");
                return false;
            }
            if (!Directory.GetParent(OutputFile).Exists)
            {
                Log.LogError(Directory.GetParent(OutputFile) + " does not exist!");
                return false;
            }

            var vars = Utils.ParseVariables(Variables);
            foreach(var var in vars)
                Log.LogMessage(string.Format("Adding variable '{0}' with value '{1}'", var.Key, var.Value));
            try
            {
                var res = new DependencyAnalysis().Analyse(Dll, vars);
                File.WriteAllText(OutputFile, new JsGenerator().GenerateJs(res));
                Log.LogMessage("JS dependencies written to " + OutputFile);
                return true;
            }
            catch (ReflectionTypeLoadException e)
            {
               Log.LogErrorFromException(e); 
                if (e.InnerException != null)
                    Log.LogErrorFromException(e.InnerException);
                foreach(var ex in e.LoaderExceptions)
                    Log.LogErrorFromException(ex);

                throw;
            }
            return false;
        }
    }

    public class JsGenerator
    {
        public string GenerateJs(Dictionary<string, string> dependencyDefinitions)
        {
            var sb = new StringBuilder();
            foreach(var dd in dependencyDefinitions)
            {
                sb.Append(string.Format("$DI.r(\"{0}\",{1});", dd.Key, dd.Value));
            }
            return sb.ToString();
        }
    }
}
