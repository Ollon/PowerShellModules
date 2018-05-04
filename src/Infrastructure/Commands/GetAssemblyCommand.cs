using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace PowerShell.Infrastructure.Commands
{
    public partial class GetAssemblyCommand
    {
        
        protected override void ProcessRecord()
        {

            switch (ParameterSetName)
            {

            }

            if (MyInvocation.BoundParameters.ContainsKey("Load"))
            {
                if (Load.StartsWith(".\\", StringComparison.Ordinal))
                {
                    Load = Path.Combine(PathEval.CurrentDirectory, LoadFrom.Substring(2));
                }
                PageResults(Assembly.Load(Load));
            }
            else if (MyInvocation.BoundParameters.ContainsKey("LoadFrom"))
            {
                if (LoadFrom.StartsWith(".\\", StringComparison.Ordinal))
                {
                    LoadFrom = Path.Combine(PathEval.CurrentDirectory, LoadFrom.Substring(2));
                }
                PageResults(Assembly.LoadFrom(LoadFrom));
            }
            else if (MyInvocation.BoundParameters.ContainsKey("All"))
            {
                PageResults(AppDomain.CurrentDomain.GetAssemblies());
            }
            else if (MyInvocation.BoundParameters.ContainsKey("Name"))
            {
                PageResults(GetAssemblyByName(NameParameterValue));
            }
        }

        public object GetDynamicParameters()
        {
            IEnumerable<string> assemblyNames = GetAssemblyNames();
            Collection<Attribute> attributes = new Collection<Attribute>
            {
                new ParameterAttribute(),
                new ValidateSetAttribute(assemblyNames.ToArray())
            };
            RuntimeDefinedParameter nameParameter = new RuntimeDefinedParameter("Name", typeof(string), attributes);
            _dynamicParameters = new RuntimeDefinedParameterDictionary
            {
                {"Name", nameParameter}
            };
            return _dynamicParameters;
        }

        private static IEnumerable<string> GetAssemblyNames()
        {
            IEnumerable<string> assemblyNames = AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetName().Name);
            return assemblyNames;
        }

        private Assembly GetAssemblyByName(string name)
        {
            Assembly result = null;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly.GetName().Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    result = assembly;
                }
            }
            return result;
        }
    }
}
