using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PowerShell.EditorFeatures.Core.Parsing
{
    /// <summary>
    /// Provides a default implementation of the <see cref="CodeGenerator"/>.
    /// </summary>
    public abstract class CodeGeneratorBase : AbstractCodeGenerator
    {
        protected CodeGeneratorBase() : base()
        {

        }
        protected CodeGeneratorBase(TextWriter writer) : base(writer)
        {

        }

        protected override string NullToken
        {
            get
            {
                return "null";
            }
        }

        protected override string CreateEscapedIdentifier(string value)
        {
            return value;
        }

        protected override string CreateValidIdentifier(string value)
        {
            return value;
        }

        protected override void GenerateArgumentReferenceExpression(CodeArgumentReferenceExpression e)
        {

        }

        protected override void GenerateArrayCreateExpression(CodeArrayCreateExpression e)
        {

        }

        protected override void GenerateArrayIndexerExpression(CodeArrayIndexerExpression e)
        {

        }

        protected override void GenerateAssignStatement(CodeAssignStatement e)
        {

        }

        protected override void GenerateAttachEventStatement(CodeAttachEventStatement e)
        {

        }

        protected override void GenerateAttributeDeclarationsEnd(CodeAttributeDeclarationCollection attributes)
        {

        }

        protected override void GenerateAttributeDeclarationsStart(CodeAttributeDeclarationCollection attributes)
        {

        }

        protected override void GenerateBaseReferenceExpression(CodeBaseReferenceExpression e)
        {

        }

        protected override void GenerateCastExpression(CodeCastExpression e)
        {

        }

        protected override void GenerateComment(CodeComment e)
        {

        }

        protected override void GenerateConditionStatement(CodeConditionStatement e)
        {

        }

        protected override void GenerateConstructor(CodeConstructor e, CodeTypeDeclaration c)
        {

        }

        protected override void GenerateDelegateCreateExpression(CodeDelegateCreateExpression e)
        {

        }

        protected override void GenerateDelegateInvokeExpression(CodeDelegateInvokeExpression e)
        {

        }

        protected override void GenerateEntryPointMethod(CodeEntryPointMethod e, CodeTypeDeclaration c)
        {

        }

        protected override void GenerateEvent(CodeMemberEvent e, CodeTypeDeclaration c)
        {

        }

        protected override void GenerateEventReferenceExpression(CodeEventReferenceExpression e)
        {

        }

        protected override void GenerateExpressionStatement(CodeExpressionStatement e)
        {

        }

        protected override void GenerateField(CodeMemberField e)
        {

        }

        protected override void GenerateFieldReferenceExpression(CodeFieldReferenceExpression e)
        {

        }

        protected override void GenerateGotoStatement(CodeGotoStatement e)
        {

        }

        protected override void GenerateIndexerExpression(CodeIndexerExpression e)
        {

        }

        protected override void GenerateIterationStatement(CodeIterationStatement e)
        {

        }

        protected override void GenerateLabeledStatement(CodeLabeledStatement e)
        {

        }

        protected override void GenerateLinePragmaEnd(CodeLinePragma e)
        {

        }

        protected override void GenerateLinePragmaStart(CodeLinePragma e)
        {

        }

        protected override void GenerateMethod(CodeMemberMethod e, CodeTypeDeclaration c)
        {

        }

        protected override void GenerateMethodInvokeExpression(CodeMethodInvokeExpression e)
        {

        }

        protected override void GenerateMethodReferenceExpression(CodeMethodReferenceExpression e)
        {

        }

        protected override void GenerateMethodReturnStatement(CodeMethodReturnStatement e)
        {

        }

        protected override void GenerateNamespaceEnd(CodeNamespace e)
        {

        }

        protected override void GenerateNamespaceImport(CodeNamespaceImport e)
        {

        }

        protected override void GenerateNamespaceStart(CodeNamespace e)
        {

        }

        protected override void GenerateObjectCreateExpression(CodeObjectCreateExpression e)
        {

        }

        protected override void GenerateProperty(CodeMemberProperty e, CodeTypeDeclaration c)
        {

        }

        protected override void GeneratePropertyReferenceExpression(CodePropertyReferenceExpression e)
        {

        }

        protected override void GeneratePropertySetValueReferenceExpression(CodePropertySetValueReferenceExpression e)
        {

        }

        protected override void GenerateRemoveEventStatement(CodeRemoveEventStatement e)
        {

        }

        protected override void GenerateSnippetExpression(CodeSnippetExpression e)
        {

        }

        protected override void GenerateSnippetMember(CodeSnippetTypeMember e)
        {

        }

        protected override void GenerateThisReferenceExpression(CodeThisReferenceExpression e)
        {

        }

        protected override void GenerateThrowExceptionStatement(CodeThrowExceptionStatement e)
        {

        }

        protected override void GenerateTryCatchFinallyStatement(CodeTryCatchFinallyStatement e)
        {

        }

        protected override void GenerateTypeConstructor(CodeTypeConstructor e)
        {

        }

        protected override void GenerateTypeEnd(CodeTypeDeclaration e)
        {

        }

        protected override void GenerateTypeStart(CodeTypeDeclaration e)
        {

        }

        protected override void GenerateVariableDeclarationStatement(CodeVariableDeclarationStatement e)
        {

        }

        protected override void GenerateVariableReferenceExpression(CodeVariableReferenceExpression e)
        {

        }

        protected override string GetTypeOutput(CodeTypeReference value)
        {
            return string.Empty;
        }

        protected override bool IsValidIdentifier(string value)
        {
            return false;
        }

        protected override string QuoteSnippetString(string value)
        {
            return value;
        }

        protected override bool Supports(GeneratorSupport support)
        {
            return true;
        }
    }
}
