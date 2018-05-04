using System.IO;
using System.Management.Automation;
using System.Reflection;

namespace PowerShell.Infrastructure.Commands
{
    public partial class ResolveAssemblyCommand
    {
        protected override void ProcessRecord()
        {

            if (ParameterSetName == nameof(Path))
            {
                if (File.Exists(Path))
                {
                    Assembly assembly = Assembly.LoadFrom(Path);
                    WriteObject(assembly);
                    WriteVerbose($"Assembly '{assembly.FullName}' Loaded Successfully...");
                }
            }

            if (ParameterSetName == nameof(PartialName))
            {
#pragma warning disable CS0618 // Type or member is obsolete
                Assembly assembly = Assembly.LoadWithPartialName(PartialName);
#pragma warning restore CS0618 // Type or member is obsolete


                WriteObject(assembly);
                WriteVerbose($"Assembly '{assembly.FullName}' Loaded Successfully...");
            }


        }
    }
}
