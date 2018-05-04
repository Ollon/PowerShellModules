using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace PowerShell.EditorFeatures.Core.Parsing
{
    /// <summary>
    /// Concrete implementation of a CodeDom CodeGenerator for C#.
    /// </summary>
    public class CSharpCodeGenerator : AbstractCSharpCodeGenerator
    {
        /// <summary>
        /// Initializes a new Instance of the <see cref="T:PowerShell.EditorFeatures.Core.Parsing.CSharpCodeGenerator"/> class.
        /// </summary>
        public CSharpCodeGenerator() : base()
        {

        }

        /// <summary>
        /// Initializes a new Instance of the <see cref="T:PowerShell.EditorFeatures.Core.Parsing.CSharpCodeGenerator"/> class with
        /// the specified <see cref="T:System.IO.TextWriter"/>.
        /// </summary>
        /// <param name="output"></param>
        public CSharpCodeGenerator(TextWriter output) : base(output)
        {

        }
    }
}
