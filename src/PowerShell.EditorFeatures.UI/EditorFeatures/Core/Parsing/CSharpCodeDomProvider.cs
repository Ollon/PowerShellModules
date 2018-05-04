using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PowerShell.EditorFeatures.Core.Parsing
{
    /// <inheritdoc/>
    public class CSharpCodeDomProvider : CodeDomProvider
    {

        public override ICodeCompiler CreateCompiler()
        {
            throw new NotImplementedException();
        }

        public override ICodeGenerator CreateGenerator()
        {
            return new CSharpCodeGenerator();
        }

        public override ICodeGenerator CreateGenerator(TextWriter output)
        {
            return new CSharpCodeGenerator(output);
        }

        public override ICodeGenerator CreateGenerator(string fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            StreamWriter output = new StreamWriter(stream);
            return new CSharpCodeGenerator(output);
        }

        public override ICodeParser CreateParser()
        {
            return new CSharpCodeParser();
        }

        public override void GenerateCodeFromCompileUnit(CodeCompileUnit compileUnit, TextWriter writer, CodeGeneratorOptions options)
        {
            ICodeGenerator generator = CreateGenerator(writer);
            generator.GenerateCodeFromCompileUnit(compileUnit, writer, options);
        }

        public override void GenerateCodeFromExpression(CodeExpression expression, TextWriter writer, CodeGeneratorOptions options)
        {
            ICodeGenerator generator = CreateGenerator(writer);
            generator.GenerateCodeFromExpression(expression, writer, options);
        }

        public override void GenerateCodeFromMember(CodeTypeMember member, TextWriter writer, CodeGeneratorOptions options)
        {
            CSharpCodeGenerator generator = (CSharpCodeGenerator)CreateGenerator();

            generator.GenerateCodeFromMember(member, writer, options);
        }

        public override void GenerateCodeFromNamespace(CodeNamespace codeNamespace, TextWriter writer, CodeGeneratorOptions options)
        {
            ICodeGenerator generator = CreateGenerator(writer);
            generator.GenerateCodeFromNamespace(codeNamespace, writer, options);
        }

        public override void GenerateCodeFromStatement(CodeStatement statement, TextWriter writer, CodeGeneratorOptions options)
        {
            ICodeGenerator generator = CreateGenerator(writer);
            generator.GenerateCodeFromStatement(statement, writer, options);
        }

        public override void GenerateCodeFromType(CodeTypeDeclaration codeType, TextWriter writer, CodeGeneratorOptions options)
        {
            ICodeGenerator generator = CreateGenerator(writer);
            generator.GenerateCodeFromType(codeType, writer, options);
        }

        public virtual CodeCompileUnit Parse(string text)
        {
            return CreateParser().Parse(new StringReader(text));
        }
        public override CodeCompileUnit Parse(TextReader codeStream)
        {
            return CreateParser().Parse(codeStream);
        }

    }
}
