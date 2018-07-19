// -----------------------------------------------------------------------
// <copyright file="NewEnumStructureCommand.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.CSharp;
using PowerShell.Infrastructure.Utilities;

namespace PowerShell.Infrastructure.Commands
{
    public partial class NewEnumStructureCommand
    {
        /// <summary>
        /// When overridden in the derived class, performs execution
        /// of the command.
        /// </summary>
        /// <exception cref="T:System.Exception">
        /// This method is overridden in the implementation of
        /// individual Cmdlets, and can throw literally any exception.
        /// </exception>
        [STAThread]
        protected override void ProcessRecord()
        {
            EnumContext context = new EnumContext(Namespace, Internal, Name, Verb, EnumName, EnumMembers);

            CodeCompileUnit unit = EnumGenerator.GenerateEnumStructure(context);
            StringBuilder sb = new StringBuilder();
            using (CSharpCodeProvider provider = new CSharpCodeProvider())
            using (StringWriter writer = new StringWriter(sb))
            {
                CodeGeneratorOptions options = new CodeGeneratorOptions
                {
                    BlankLinesBetweenMembers = true,
                    BracingStyle = "C",
                    ElseOnClosing = true,
                    VerbatimOrder = true
                };

                provider.GenerateCodeFromCompileUnit(unit, writer, options);
            }

            WriteObject(sb.ToString());

            if (Clip)
            {
                Clipboard.SetText(sb.ToString());
            }
        }
    }
}
