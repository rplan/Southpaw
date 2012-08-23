using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.Build.BuildEngine;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Southpaw.Generator.Controller;
using Southpaw.Generator.Model;
using Project = Microsoft.Build.Evaluation.Project;

namespace Southpaw.Generator.MsBuild
{
    [Serializable]
    public class Generate : Task
    {
        [Required]
        public string BusinessObjectProjectFile
        {
            get;
            set;
        }

        [Required]
        public string ControllerProjectFile
        {
            get;
            set;
        }

        [Required]
        public string OutputProjectFile
        {
            get;
            set;
        }

        [Required]
        public string BusinessObjectNamespaceSubstitutionSource
        {
            get;
            set;
        }
        [Required]
        public string BusinessObjectNamespaceSubstitutionDestination
        {
            get;
            set;
        }

        [Required]
        public string ControllerNamespaceSubstitutionSource
        {
            get;
            set;
        }
        [Required]
        public string ControllerNamespaceSubstitutionDestination
        {
            get;
            set;
        }

        public override bool Execute()
        {
            if (!ValidateInput())
                return false;

            //XNamespace xns = "http://schemas.microsoft.com/developer/msbuild/2003";
            //XDocument xmldoc = XDocument.Load(BusinessObjectProjectFile);
            OutputProjectFile = Path.GetFullPath(OutputProjectFile);
            BusinessObjectProjectFile = Path.GetFullPath(BusinessObjectProjectFile);
            ControllerProjectFile = Path.GetFullPath(ControllerProjectFile);

            var currentProjects = Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection;
            var outputProject = currentProjects.GetLoadedProjects(OutputProjectFile).FirstOrDefault();
            if (outputProject == null)
            {
                outputProject = new Microsoft.Build.Evaluation.Project(OutputProjectFile);
                outputProject.FullPath = OutputProjectFile;
            }
            var businessLibraryProject = currentProjects.GetLoadedProjects(BusinessObjectProjectFile).FirstOrDefault();
            if (businessLibraryProject == null)
            {
                businessLibraryProject = new Microsoft.Build.Evaluation.Project(BusinessObjectProjectFile);
                businessLibraryProject.FullPath = BusinessObjectProjectFile;
            }
            var controllersProject = currentProjects.GetLoadedProjects(ControllerProjectFile).FirstOrDefault();
            if (controllersProject == null)
            {
                controllersProject = new Microsoft.Build.Evaluation.Project(ControllerProjectFile);
                controllersProject.FullPath = ControllerProjectFile;
            }

            if (!AddToProject(
                businessLibraryProject,
                outputProject, 
                BusinessObjectProjectFile, 
                OutputProjectFile, 
                BusinessObjectNamespaceSubstitutionSource, 
                BusinessObjectNamespaceSubstitutionDestination,
                () => new ViewModelGenerator(
                    new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>(ControllerNamespaceSubstitutionSource, ControllerNamespaceSubstitutionDestination) })
                ))
                return false;
            if (!AddToProject(
                controllersProject, 
                outputProject, 
                ControllerProjectFile, 
                OutputProjectFile, 
                ControllerNamespaceSubstitutionSource, 
                ControllerNamespaceSubstitutionDestination,
                () => new ServiceGenerator(
                    new ServiceGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>(ControllerNamespaceSubstitutionSource, ControllerNamespaceSubstitutionDestination) },
                    new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>(ControllerNamespaceSubstitutionSource, ControllerNamespaceSubstitutionDestination) })
                ))
                return false;
            
            /*
            // business objects:
            // load assembly 
            // -> get assembly name
            var assemblyNameNode = businessLibraryProject.AllEvaluatedProperties.FirstOrDefault(p => p.Name == "AssemblyName");
            //var assemblyNameNode = xmldoc.Descendants(xns + "AssemblyName").FirstOrDefault();
            if (assemblyNameNode == null)
            {
                Log.LogError("No assembly name found in Business Object project (" + BusinessObjectProjectFile + ")");
                return false;
            }
            var businessObjectsAssemblyName = assemblyNameNode.EvaluatedValue;

            // -> get output path
            var outputPathProperty = businessLibraryProject.AllEvaluatedProperties.FirstOrDefault(p => p.Name == "OutputPath");
            if (outputPathProperty == null || outputPathProperty.EvaluatedValue == "")
            {
                Log.LogError("No output path found in Business Object project (" + BusinessObjectProjectFile + ")");
                return false;
            }
            Log.LogMessage("Output path:" + outputPathProperty.EvaluatedValue);
            var businessObjectsOutput = outputPathProperty.EvaluatedValue;
            var businessObjectsFolder = Path.GetDirectoryName(BusinessObjectProjectFile);
            var businessObjectAssemblyFullPath = Path.Combine(businessObjectsFolder, businessObjectsOutput, businessObjectsAssemblyName + ".dll");
            if (!File.Exists(businessObjectAssemblyFullPath))
            {
                Log.LogError("Business Object assembly not found in derived output path '" + businessObjectAssemblyFullPath + "'");
                return false;
            }
            Log.LogMessage("Found business objects assembly " + businessObjectAssemblyFullPath);
            // -> find out which of bin/debug/assemmblyname.dll or bin/release.assemblyname.dll is most recent 
            // -> (or perhaps use current build configuration?)
            // -> load assembly from path
            var assembly = Assembly.LoadFrom(businessObjectAssemblyFullPath);
            var viewModelGenerator = new ViewModelGenerator(
                new ViewModelGeneratorOptions { NamespaceSubstitution = new Tuple<string, string>(BusinessObjectNamespaceSubstitutionSource, BusinessObjectNamespaceSubstitutionDestination) });
            var res = viewModelGenerator.Generate(assembly);
            var modelsOutputFolder = Path.GetDirectoryName(OutputProjectFile);
            //var businessObjectsOutputFolder = Path.Combine(Path.GetDirectoryName(OutputProjectFile), "ViewModels");
            foreach (var r in res)
            {
                Log.LogMessage("Found output file " + r.PathRelativeToSourceAssembly);
                var path = Path.Combine(modelsOutputFolder, r.PathRelativeToSourceAssembly);
                if (!File.Exists(path) || r.IsForceOverwrite)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    File.WriteAllText(path, r.Contents);
                }
                if (!outputProject.GetItems("Compile").Any(x => x.UnevaluatedInclude == r.PathRelativeToSourceAssembly))
                {
                    outputProject.AddItem("Compile", r.PathRelativeToSourceAssembly);
                }
            }
            */
            outputProject.Save();



            // pass assembly, parameters to generator
            // generate
            // for each generated file, figure out which folder to put it in
            // -> OutputRoot + "/Generated/GeneratedModelsBase.cs" for base file
            // -> OutputRoot + (classNamespace.Replace(assemblyName, "").ToFolderStructure() + "\" + className + ".cs"
            // add all generated files to the project if they don't already exist

            // controllers:
            // as above
            return true;
        }

        private bool AddToProject(
            Microsoft.Build.Evaluation.Project inputProject,
            Microsoft.Build.Evaluation.Project outputProject, 
            string inputProjectFilename,
            string outputProjectFilename,
            string namespaceSubstitutionSource,
            string namespaceSubstitutionDestination,
            Func<IGenerator> getGenerator)
        {
            Log.LogMessage("Generating classes for input project " + inputProjectFilename);
            var assemblyNameNode = inputProject.AllEvaluatedProperties.FirstOrDefault(p => p.Name == "AssemblyName");
            //var assemblyNameNode = xmldoc.Descendants(xns + "AssemblyName").FirstOrDefault();
            if (assemblyNameNode == null)
            {
                Log.LogError("No assembly name found in Business Object project (" + inputProjectFilename + ")");
                return false;
            }
            var assemblyName = assemblyNameNode.EvaluatedValue;

            // -> get output path
            var outputPathProperty = inputProject.AllEvaluatedProperties.FirstOrDefault(p => p.Name == "OutputPath");
            if (outputPathProperty == null || outputPathProperty.EvaluatedValue == "")
            {
                Log.LogError("No output path found in Business Object project (" + inputProjectFilename + ")");
                return false;
            }
            Log.LogMessage("Output path:" + outputPathProperty.EvaluatedValue);
            var output = outputPathProperty.EvaluatedValue;
            var folder = Path.GetDirectoryName(inputProjectFilename);
            var assemblyFullPath = Path.Combine(folder, output, assemblyName + ".dll");
            if (!File.Exists(assemblyFullPath))
            {
                Log.LogError("Input assembly not found in derived output path '" + assemblyFullPath + "'");
                return false;
            }
            //Log.LogMessage("Found input assembly " + assemblyFullPath);
            var viewModelGenerator = getGenerator();
            var modelsOutputFolder = Path.GetDirectoryName(outputProjectFilename);
            //Log.LogMessage("output path: " + modelsOutputFolder);
            // -> find out which of bin/debug/assemmblyname.dll or bin/release.assemblyname.dll is most recent 
            // -> (or perhaps use current build configuration?)
            // -> load assembly from path
            Log.LogMessage("________________________");
            Log.LogMessage("Current assemblies (before generation): " + AppDomain.CurrentDomain.GetAssemblies().Count());
            //foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
                //Log.LogMessage(ass.Location);
            AppDomain newAppDomain = AppDomain.CreateDomain("newAppDomain", null, new AppDomainSetup { 
                ApplicationName = "test app name", 
                //ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                ApplicationBase = Path.GetDirectoryName(assemblyFullPath)
                //PrivateBinPath = 
            });
            var proxyDomain = new ProxyDomain
                                  {
                                      AssemblyFullPath = assemblyFullPath,
                                      ModelsOutputFolder = modelsOutputFolder,
                                      OutputProject = outputProject,
                                      ViewModelGenerator = viewModelGenerator
                                  };
            try
            {
                newAppDomain.DoCallBack(proxyDomain.DoStuff);
                //DoGeneration();
                //var businessObjectsOutputFolder = Path.Combine(Path.GetDirectoryName(OutputProjectFile), "ViewModels");
                if (proxyDomain.Errors != null && proxyDomain.Errors.Count > 0)
                {
                    foreach (var error in proxyDomain.Errors)
                        Log.LogError(error);
                    return false;
                }
            }
            finally
            {
                AppDomain.Unload(newAppDomain);
            }
            Log.LogMessage("----------------");
            Log.LogMessage("Current assemblies (after generation): " + AppDomain.CurrentDomain.GetAssemblies().Count().ToString());
            //foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
                //Log.LogMessage(ass.Location);
            return true;
        }

        private bool ValidateInput()
        {
            if (!File.Exists(BusinessObjectProjectFile))
            {
                Log.LogError("The Business Object project file ('" + BusinessObjectProjectFile + "') does not exist!");
                return false;
            }
            if (!File.Exists(ControllerProjectFile))
            {
                Log.LogError("The Controller project file ('" + ControllerProjectFile + "') does not exist!");
                return false;
            }
            if (!File.Exists(OutputProjectFile))
            {
                Log.LogError("The Output project file ('" + OutputProjectFile + "') does not exist!");
                return false;
            }
            return true;
        }
    }


    [Serializable]
    class ProxyDomain : MarshalByRefObject
    {
        public IGenerator ViewModelGenerator { get; set; }
        public string AssemblyFullPath { get; set; }
        public string ModelsOutputFolder { get; set; }
        public Project OutputProject { get; set; }
        public List<string> Errors;

        public void DoStuff()
        {
            Errors = new List<string>();
            //try
            //{
            Assembly assembly = null;
            // TODO NCU: what happens when exceptions are thrown here?
            try
            {
                assembly = Assembly.LoadFrom(AssemblyFullPath);
                //assembly = newAppDomain.Load(Path.GetFileNameWithoutExtension(assemblyFullPath));
                //assembly = proxyDomain.GetAssembly(assemblyFullPath);
                assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                if (e.LoaderExceptions.Any(x => x.Message.Contains("Southpaw.Runtime")))
                {
                    Errors.Add("The target assembly " + AssemblyFullPath + " is missing a reference to Southpaw.Runtime - please add this and try again.");
                    return;
                }
                foreach (var x in e.LoaderExceptions)
                {
                    Errors.Add("Error loading assembly '" + AssemblyFullPath + "': " + x.Message);
                }
                return;
            }
            var res = ViewModelGenerator.Generate(assembly);
            foreach (var r in res)
            {
                var path = Path.Combine(ModelsOutputFolder, r.PathRelativeToSourceAssembly);
                //Log.LogMessage("Found output file " + path);
                if (!File.Exists(path) || r.IsForceOverwrite)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    File.WriteAllText(path, r.Contents);
                }
                if (!OutputProject.GetItems("Compile").Any(x => x.UnevaluatedInclude == r.PathRelativeToSourceAssembly))
                {
                    OutputProject.AddItem("Compile", r.PathRelativeToSourceAssembly);
                }
            }
        }
    }
}
