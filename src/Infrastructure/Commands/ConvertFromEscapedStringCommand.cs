using PowerShell.Infrastructure.Utilities;

namespace PowerShell.Infrastructure.Commands
{
    public partial class ConvertFromEscapedStringCommand
    {
        protected override void ProcessRecord()
        {
            if (InputObject != null)
            {
                switch (Style)
                {
                    case EscapeStyle.PowerShell:
                        break;
                    case EscapeStyle.CSharpString:
                        break;
                    case EscapeStyle.CSharpVerbatimString:
                        break;
                    case EscapeStyle.CSharpChar:
                        break;
                    case EscapeStyle.CSharpInterpolation:
                        break;
                    case EscapeStyle.CSharpVerbatimInterpolation:
                        break;
                    case EscapeStyle.CSharpFormat:
                        break;
                    default:
                        break;
                }


                string rtnString = (string)InputObject.BaseObject;
                WriteObject(rtnString.Escape(Style));
                WriteVerbose(rtnString.UnEscape());
            }
        }
    }
}
