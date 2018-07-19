using System.Text;
using PowerShell.Infrastructure.Utilities;

namespace PowerShell.Infrastructure.Commands
{
    public partial class ConvertToCamelCaseCommand
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
                        sb.Append(i == 0 ? StringUtilities.MakeCamel(current, !Full) : StringUtilities.MakePascal(current, !Full));
                    }
                }
                else
                {
                    sb.Append(StringUtilities.MakeCamel(stringObject, !Full));
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
