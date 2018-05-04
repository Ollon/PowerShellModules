using PowerShell.Infrastructure.Utilities;

namespace PowerShell.Infrastructure.Commands
{
    public partial class ConvertToEscapedStringCommand
    {
        protected override void ProcessRecord()
        {
            if (InputObject != null)
            {
                string rtnString = (string)InputObject.BaseObject;
                WriteObject(rtnString.Escape(Style));
                WriteVerbose(rtnString.Escape(Style));
            }
        }
    }
}
