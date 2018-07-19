using System;
using System.Text;
using Microsoft.PowerShell.Host.ISE;
using PowerShell.Infrastructure.Utilities;

namespace PowerShell.Infrastructure.Commands
{
    public partial class ConvertToPascalCaseCommand
    {
        protected override void BeginProcessing()
        {
            if (IgnoreChars == null)
            {
                IgnoreChars = " _-";
            }
        }

        protected override void ProcessRecord()
        {
            StringBuilder sb = new StringBuilder();
            if (InputObject?.BaseObject is string stringObject)
            {
                if (ShouldSplitString(stringObject))
                {
                    string[] splits = stringObject.Split(IgnoreChars.ToCharArray());
                    for (int i = 0; i < splits.Length; i++)
                    {
                        string current = splits[i];
                        sb.Append(StringUtilities.MakePascal(current, !Full));
                    }
                }
                else
                {
                    sb.Append(StringUtilities.MakePascal(stringObject, !Full));
                }
                WriteObject(sb.ToString());
                WriteVerbose(sb.ToString());
            }
        }

        private bool ShouldSplitString(string stringObject)
        {
            if (!string.IsNullOrEmpty(IgnoreChars))
                foreach (char character in IgnoreChars)
                {
                    if (stringObject.Contains(character.ToString()))
                    {
                        return true;
                    }
                }

            return false;
        }
        
    }
}
