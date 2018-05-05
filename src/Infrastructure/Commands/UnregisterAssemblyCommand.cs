using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using PowerShell.Infrastructure.Fusion;

namespace PowerShell.Infrastructure.Commands
{
    public partial class UnregisterAssemblyCommand : AbstractPSCmdlet, IDynamicParameters
    {
        protected override void BeginProcessing()
        {
            if (!string.IsNullOrEmpty(Path) && Path.StartsWith(".\\", StringComparison.Ordinal))
            {
                Path = System.IO.Path.Combine(PathEval.CurrentDirectory, Path.Substring(2));
            }
        }

        protected override void ProcessRecord()
        {
            AssemblyName assemblyName = null;
            if (ParameterSetName == "ByName")
            {
                assemblyName = GetAssemblyByName(NameParameterValue).GetName();
            }
            else
            {
                assemblyName = System.Reflection.AssemblyName.GetAssemblyName(Path);
            }
            if (assemblyName != null)
            {
                GlobalAssemblyCache cache = GlobalAssemblyCache.GetCache();
                UninstallDisposition result = cache.UninstallAssembly(assemblyName);
                WriteObject(result);
            }
        }

        private static IEnumerable<string> GetAssemblyNames()
        {
            IEnumerable<string> assemblyNames = GlobalAssemblyCache.GetCache()
                .Select(a => a.Name);
            return assemblyNames;
        }

        private Assembly GetAssemblyByName(string name)
        {
            Assembly result = null;
            Assembly[] assemblies = GlobalAssemblyCache.GetAssemblies().ToArray();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly.GetName().Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    result = assembly;
                }
            }
            return result;
        }

        public RuntimeDefinedParameterDictionary GetDynamicParameters()
        {
            IEnumerable<string> assemblyNames = GetAssemblyNames();
            Collection<Attribute> attributes = new Collection<Attribute>
            {
                new ParameterAttribute
                {
                    ParameterSetName = "ByName"
                },
                new ValidateSetAttribute(assemblyNames.ToArray())
            };
            RuntimeDefinedParameter nameParameter = new RuntimeDefinedParameter("Name", typeof(string), attributes);
            DynamicParameters = new RuntimeDefinedParameterDictionary
            {
                {"Name", nameParameter}
            };
            return DynamicParameters;

        }


        object IDynamicParameters.GetDynamicParameters()
        {
            return GetDynamicParameters();
        }
    }
}
