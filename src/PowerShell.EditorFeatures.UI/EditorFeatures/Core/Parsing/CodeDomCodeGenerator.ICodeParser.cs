// -----------------------------------------------------------------------
// <copyright file="CodeDomCodeGenerator.ICodeParser.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.CodeDom;
using System.IO;
using System.Reflection;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace PowerShell.EditorFeatures.Core.Parsing
{
    public partial class CodeDomCodeGenerator
    {
        public CodeCompileUnit Parse(string text)
        {
            CodeDomConvertVisitor visitor = new CodeDomConvertVisitor();
            SyntaxTree syntaxTree = SyntaxTree.Parse(new StringReader(text));
            syntaxTree.FileName = "SyntaxTree.cs";
            CSharpUnresolvedFile unresolvedFile = syntaxTree.ToTypeSystem();
            CodeCompileUnit unit = visitor.Convert(
                new SimpleCompilation(
                    new DefaultUnresolvedAssembly(
                        Assembly.GetExecutingAssembly().GetName().FullName)), syntaxTree, unresolvedFile);

            return unit;
        }
        public CodeCompileUnit Parse(TextReader codeStream)
        {
            string text = codeStream.ReadToEnd();
            return Parse(text);
        }
    }
}
