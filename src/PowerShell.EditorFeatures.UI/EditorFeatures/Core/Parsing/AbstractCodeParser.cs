using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PowerShell.EditorFeatures.Core.Parsing
{
    public abstract class AbstractCodeParser : ICodeParser
    {
        public abstract CodeCompileUnit ParseFile(string filePath);
        public abstract CodeCompileUnit ParseCodeCompileUnit(string text);
        public abstract CodeCompileUnit Parse(TextReader codeStream);
        public abstract CompilationUnitSyntax ParseCompilationUnit(string text);
    }
}
