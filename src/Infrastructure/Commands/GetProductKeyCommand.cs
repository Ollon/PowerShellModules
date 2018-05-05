using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerShell.Infrastructure.Utilities;

namespace PowerShell.Infrastructure.Commands
{
    public partial class GetProductKeyCommand
    {
        /// <summary>
        /// When overridden in the derived class, performs execution
        /// of the command.
        /// </summary>
        /// <exception cref="T:System.Exception">
        /// This method is overridden in the implementation of
        /// individual Cmdlets, and can throw literally any exception.
        /// </exception>
        protected override void ProcessRecord()
        {
            if (All)
            {
                PageResults(ProductKeyFactory.All().AsEnumerable());
            }
            else
            {
                WriteObject(ProductKeyFactory.GetProductKey(ProductKey));
            }
        }
    }
}
