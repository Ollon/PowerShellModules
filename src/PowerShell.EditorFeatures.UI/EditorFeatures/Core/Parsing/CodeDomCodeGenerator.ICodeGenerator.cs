// -----------------------------------------------------------------------
// <copyright file="CodeDomCodeGenerator.ICodeGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace PowerShell.EditorFeatures.Core.Parsing
{
    public partial class CodeDomCodeGenerator
    {
        public void ValidateIdentifier(string value)
        {
        }

        public string GetTypeOutput(CodeTypeReference type)
        {
            throw new NotSupportedException();
        }

        public string CreateEscapedIdentifier(string value)
        {
            return value;
        }

        public bool IsValidIdentifier(string value)
        {
            return true;
        }

        public bool Supports(GeneratorSupport supports)
        {
            return true;
        }

        public void GenerateCodeFromType(CodeTypeDeclaration e, TextWriter w, CodeGeneratorOptions o)
        {
            GenerateCodeTypeDeclaration(null, e, w, o);
        }

        public string CreateValidIdentifier(string value)
        {
            return GetNextVar(value);
        }

        public void GenerateCodeFromNamespace(CodeNamespace e, TextWriter w, CodeGeneratorOptions o)
        {
            GenerateCodeNamespace(null, e, w, o);
        }

        public void GenerateCodeFromCompileUnit(CodeCompileUnit e, TextWriter w, CodeGeneratorOptions o)
        {
            GenerateCodeCompileUnit(e, w, o);
        }

        public void GenerateCodeFromExpression(CodeExpression e, TextWriter w, CodeGeneratorOptions o)
        {
            GenerateCodeExpression(null, e, w, o);
        }

        public void GenerateCodeFromStatement(CodeStatement e, TextWriter w, CodeGeneratorOptions o)
        {
            GenerateCodeStatement(null, e, w, o);
        }

        public void GenerateCodeFromMember(CodeTypeMember e, TextWriter w, CodeGeneratorOptions o)
        {
            GenerateCodeTypeMember(null, e, w, o);
        }
    }
}
