using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerShell.Infrastructure.Commands
{
    public partial class SplitStringCommand
    {
        protected override void ProcessRecord()
        {

            WriteObject(String.Split(Delimeter.ToArray()), true);

        }
    }
}
