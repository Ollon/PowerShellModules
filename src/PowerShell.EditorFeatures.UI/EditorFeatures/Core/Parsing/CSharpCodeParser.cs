using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PowerShell.EditorFeatures.Core.Parsing
{
    /// <summary>
    /// Code parser for C# that outputs a <see cref="T:System.CodeDom.CodeCompileUnit"/>.
    /// </summary>
    public class CSharpCodeParser : AbstractCodeParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PowerShell.EditorFeatures.Core.Parsing.CSharpCodeParser"/> class.
        /// </summary>
        public CSharpCodeParser()
        {

        }

        /// <summary>
        /// Parses a given text string and outputs a <see cref="T:System.CodeDom.CodeCompileUnit"/>
        /// </summary>
        /// <param name="text">the text for the parser.</param>
        /// <returns>a <see cref="T:System.CodeDom.CodeCompileUnit"/></returns>
        /// <exception cref="FailedParseConversionException" />
        /// <exception cref="ArgumentNullException" />
        public override CodeCompileUnit ParseCodeCompileUnit(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }


            CodeDomConvertVisitor visitor = new CodeDomConvertVisitor();
            SyntaxTree syntaxTree = SyntaxTree.Parse(new StringReader(text));
            syntaxTree.FileName = "SyntaxTree.cs";
            CSharpUnresolvedFile unresolvedFile = syntaxTree.ToTypeSystem();
            CodeCompileUnit unit = visitor.Convert(
                new SimpleCompilation(
                    new DefaultUnresolvedAssembly(
                        Assembly.GetExecutingAssembly().GetName().FullName)), syntaxTree, unresolvedFile);

            if (unit == null)
            {
                throw new FailedParseConversionException();
            }

            return unit;
        }

        /// <summary>
        /// Parses a given <see cref="T:System.IO.TextReader"/> and outputs a <see cref="T:System.CodeDom.CodeCompileUnit"/>
        /// </summary>
        /// <param name="codeStream">the  <see cref="T:System.IO.TextReader"/> for the parser.</param>
        /// <returns>a <see cref="T:System.CodeDom.CodeCompileUnit"/></returns>
        /// <exception cref="FailedParseConversionException" />
        /// <exception cref="ArgumentNullException" />
        public override CodeCompileUnit Parse(TextReader codeStream)
        {
            string text = codeStream.ReadToEnd();
            return ParseCodeCompileUnit(text);
        }

        public override CompilationUnitSyntax ParseCompilationUnit(string text)
        {
            return SyntaxFactory.ParseCompilationUnit(text);
        }

        public override CodeCompileUnit ParseFile(string filePath)
        {
            string text = File.ReadAllText(filePath);
            return ParseCodeCompileUnit(text);
        }
    }
}
