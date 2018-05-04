using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PowerShell.EditorFeatures.Core.Parsing
{
    public abstract class AbstractCodeGenerator : ICodeGenerator
    {
        private const int ParameterMultilineThreshold = 15;
        private IndentedTextWriter output;
        protected bool inNestedBinary = false;
        protected bool usingsAlreadyWritten = false;

        protected AbstractCodeGenerator()
        {

        }

        protected AbstractCodeGenerator(TextWriter writer)
        {
            if (writer is IndentedTextWriter indentedTextWriter)
            {
                output = indentedTextWriter;
            }
            else
            {
                output = new IndentedTextWriter(writer);
            }
        }

        bool ICodeGenerator.Supports(GeneratorSupport supports)
        {
            return true;
        }

        void ICodeGenerator.GenerateCodeFromType(CodeTypeDeclaration e, TextWriter w, CodeGeneratorOptions o)
        {
            bool setLocal = false;
            if (output != null && w != output.InnerWriter)
            {
                throw new InvalidOperationException();
            }
            if (output == null)
            {
                setLocal = true;
                Options = (o == null) ? new CodeGeneratorOptions() : o;
                output = new IndentedTextWriter(w, Options.IndentString);
            }

            try
            {
                GenerateType(e);
            }
            finally
            {
                if (setLocal)
                {
                    output = null;
                    Options = null;
                }
            }
        }

        void ICodeGenerator.GenerateCodeFromExpression(CodeExpression e, TextWriter w, CodeGeneratorOptions o)
        {
            bool setLocal = false;
            if (output != null && w != output.InnerWriter)
            {
                throw new InvalidOperationException();
            }
            if (output == null)
            {
                setLocal = true;
                Options = (o == null) ? new CodeGeneratorOptions() : o;
                output = new IndentedTextWriter(w, Options.IndentString);
            }

            try
            {
                GenerateExpression(e);
            }
            finally
            {
                if (setLocal)
                {
                    output = null;
                    Options = null;
                }
            }
        }

        void ICodeGenerator.GenerateCodeFromCompileUnit(CodeCompileUnit e, TextWriter w, CodeGeneratorOptions o)
        {
            bool setLocal = false;
            if (output != null && w != output.InnerWriter)
            {
                throw new InvalidOperationException();
            }
            if (output == null)
            {
                setLocal = true;
                Options = (o == null) ? new CodeGeneratorOptions() : o;
                output = new IndentedTextWriter(w, Options.IndentString);
            }

            try
            {
                if (e is CodeSnippetCompileUnit)
                {
                    GenerateSnippetCompileUnit((CodeSnippetCompileUnit)e);
                }
                else
                {
                    GenerateCompileUnit(e);
                }
            }
            finally
            {
                if (setLocal)
                {
                    output = null;
                    Options = null;
                }
            }
        }

        void ICodeGenerator.GenerateCodeFromNamespace(CodeNamespace e, TextWriter w, CodeGeneratorOptions o)
        {
            bool setLocal = false;
            if (output != null && w != output.InnerWriter)
            {
                throw new InvalidOperationException();
            }
            if (output == null)
            {
                setLocal = true;
                Options = (o == null) ? new CodeGeneratorOptions() : o;
                output = new IndentedTextWriter(w, Options.IndentString);
            }

            try
            {
                GenerateNamespace(e);
            }
            finally
            {
                if (setLocal)
                {
                    output = null;
                    Options = null;
                }
            }
        }

        void ICodeGenerator.GenerateCodeFromStatement(CodeStatement e, TextWriter w, CodeGeneratorOptions o)
        {
            bool setLocal = false;
            if (output != null && w != output.InnerWriter)
            {
                throw new InvalidOperationException();
            }
            if (output == null)
            {
                setLocal = true;
                Options = (o == null) ? new CodeGeneratorOptions() : o;
                output = new IndentedTextWriter(w, Options.IndentString);
            }

            try
            {
                GenerateStatement(e);
            }
            finally
            {
                if (setLocal)
                {
                    output = null;
                    Options = null;
                }
            }
        }

        bool ICodeGenerator.IsValidIdentifier(string value)
        {
            return this.IsValidIdentifier(value);
        }
        void ICodeGenerator.ValidateIdentifier(string value)
        {
            this.ValidateIdentifier(value);
        }

        string ICodeGenerator.CreateEscapedIdentifier(string value)
        {
            return this.CreateEscapedIdentifier(value);
        }

        string ICodeGenerator.CreateValidIdentifier(string value)
        {
            return this.CreateValidIdentifier(value);
        }

        string ICodeGenerator.GetTypeOutput(CodeTypeReference type)
        {
            return this.GetTypeOutput(type);
        }

        public virtual void GenerateCodeFromMember(CodeTypeMember member, TextWriter writer, CodeGeneratorOptions Options)
        {
            if (this.output != null)
            {
                throw new InvalidOperationException("");
            }
            this.Options = (Options == null) ? new CodeGeneratorOptions() : Options;
            this.output = new IndentedTextWriter(writer, this.Options.IndentString);

            try
            {
                CodeTypeDeclaration dummyClass = new CodeTypeDeclaration();
                this.CurrentClass = dummyClass;
                GenerateTypeMember(member, dummyClass);
            }
            finally
            {
                this.CurrentClass = null;
                this.output = null;
                this.Options = null;
            }
        }

        public CodeTypeDeclaration CurrentClass { get; private set; }

        public string CurrentTypeName
        {
            get
            {
                if (CurrentClass != null)
                {
                    return CurrentClass.Name;
                }
                return "<% unknown %>";
            }
        }

        public CodeTypeMember CurrentMember { get; private set; }

        public string CurrentMemberName
        {
            get
            {
                if (CurrentMember != null)
                {
                    return CurrentMember.Name;
                }
                return "<% unknown %>";
            }
        }

        public bool IsCurrentInterface
        {
            get
            {
                if (CurrentClass != null && !(CurrentClass is CodeTypeDelegate))
                {
                    return CurrentClass.IsInterface;
                }
                return false;
            }
        }

        public bool IsCurrentClass
        {
            get
            {
                if (CurrentClass != null && !(CurrentClass is CodeTypeDelegate))
                {
                    return CurrentClass.IsClass;
                }
                return false;
            }
        }

        public bool IsCurrentStruct
        {
            get
            {
                if (CurrentClass != null && !(CurrentClass is CodeTypeDelegate))
                {
                    return CurrentClass.IsStruct;
                }
                return false;
            }
        }

        public bool IsCurrentEnum
        {
            get
            {
                if (CurrentClass != null && !(CurrentClass is CodeTypeDelegate))
                {
                    return CurrentClass.IsEnum;
                }
                return false;
            }
        }

        public bool IsCurrentDelegate
        {
            get
            {
                if (CurrentClass != null && CurrentClass is CodeTypeDelegate)
                {
                    return true;
                }
                return false;
            }
        }

        public int Indent
        {
            get
            {
                return output.Indent;
            }
            set
            {
                output.Indent = value;
            }
        }

        protected abstract string NullToken { get; }

        public TextWriter Output => output;

        public CodeGeneratorOptions Options { get; private set; } = new CodeGeneratorOptions { BracingStyle = "C" };

        public void GenerateType(CodeTypeDeclaration e)
        {
            CurrentClass = e;
            if (e.StartDirectives.Count > 0)
            {
                GenerateDirectives(e.StartDirectives);
            }
            GenerateCommentStatements(e.Comments);
            if (e.LinePragma != null) GenerateLinePragmaStart(e.LinePragma);
            GenerateTypeStart(e);
            if (Options.VerbatimOrder)
            {
                foreach (CodeTypeMember member in e.Members)
                {
                    GenerateTypeMember(member, e);
                }
            }
            else
            {
                GenerateFields(e);
                GenerateSnippetMembers(e);
                GenerateTypeConstructors(e);
                GenerateConstructors(e);
                GenerateProperties(e);
                GenerateEvents(e);
                GenerateMethods(e);
                GenerateNestedTypes(e);
            }
            CurrentClass = e;
            GenerateTypeEnd(e);
            if (e.LinePragma != null) GenerateLinePragmaEnd(e.LinePragma);
            if (e.EndDirectives.Count > 0)
            {
                GenerateDirectives(e.EndDirectives);
            }
        }

        protected virtual void GenerateDirectives(CodeDirectiveCollection directives)
        {
        }

        public void GenerateTypeMember(CodeTypeMember member, CodeTypeDeclaration declaredType)
        {
            if (Options.BlankLinesBetweenMembers)
            {
                Output.WriteLine();
            }
            if (member is CodeTypeDeclaration)
            {
                ((ICodeGenerator)this).GenerateCodeFromType((CodeTypeDeclaration)member, output.InnerWriter, Options);
                CurrentClass = declaredType;
                return;
            }
            if (member.StartDirectives.Count > 0)
            {
                GenerateDirectives(member.StartDirectives);
            }
            GenerateCommentStatements(member.Comments);
            if (member.LinePragma != null)
            {
                GenerateLinePragmaStart(member.LinePragma);
            }
            if (member is CodeMemberField)
            {
                GenerateField((CodeMemberField)member);
            }
            else if (member is CodeMemberProperty)
            {
                GenerateProperty((CodeMemberProperty)member, declaredType);
            }
            else if (member is CodeMemberMethod)
            {
                if (member is CodeConstructor)
                {
                    GenerateConstructor((CodeConstructor)member, declaredType);
                }
                else if (member is CodeTypeConstructor)
                {
                    GenerateTypeConstructor((CodeTypeConstructor)member);
                }
                else if (member is CodeEntryPointMethod)
                {
                    GenerateEntryPointMethod((CodeEntryPointMethod)member, declaredType);
                }
                else
                {
                    GenerateMethod((CodeMemberMethod)member, declaredType);
                }
            }
            else if (member is CodeMemberEvent)
            {
                GenerateEvent((CodeMemberEvent)member, declaredType);
            }
            else if (member is CodeSnippetTypeMember)
            {
                int savedIndent = Indent;
                Indent = 0;
                GenerateSnippetMember((CodeSnippetTypeMember)member);
                Indent = savedIndent;
                Output.WriteLine();
            }
            if (member.LinePragma != null)
            {
                GenerateLinePragmaEnd(member.LinePragma);
            }
            if (member.EndDirectives.Count > 0)
            {
                GenerateDirectives(member.EndDirectives);
            }
        }

        public void GenerateTypeConstructors(CodeTypeDeclaration e)
        {
            IEnumerator en = e.Members.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current is CodeTypeConstructor)
                {
                    CurrentMember = (CodeTypeMember)en.Current;
                    if (Options.BlankLinesBetweenMembers)
                    {
                        Output.WriteLine();
                    }
                    if (CurrentMember.StartDirectives.Count > 0)
                    {
                        GenerateDirectives(CurrentMember.StartDirectives);
                    }
                    GenerateCommentStatements(CurrentMember.Comments);
                    CodeTypeConstructor imp = (CodeTypeConstructor)en.Current;
                    if (imp.LinePragma != null) GenerateLinePragmaStart(imp.LinePragma);
                    GenerateTypeConstructor(imp);
                    if (imp.LinePragma != null) GenerateLinePragmaEnd(imp.LinePragma);
                    if (CurrentMember.EndDirectives.Count > 0)
                    {
                        GenerateDirectives(CurrentMember.EndDirectives);
                    }
                }
            }
        }

        public void GenerateNamespaces(CodeCompileUnit e)
        {
            foreach (CodeNamespace n in e.Namespaces)
            {
                ((ICodeGenerator)this).GenerateCodeFromNamespace(n, output.InnerWriter, Options);
            }
        }

        public void GenerateTypes(CodeNamespace e)
        {
            foreach (CodeTypeDeclaration c in e.Types)
            {
                if (Options.BlankLinesBetweenMembers)
                {
                    Output.WriteLine();
                }
                ((ICodeGenerator)this).GenerateCodeFromType(c, output.InnerWriter, Options);
            }
        }

        public void GenerateConstructors(CodeTypeDeclaration e)
        {
            IEnumerator en = e.Members.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current is CodeConstructor)
                {
                    CurrentMember = (CodeTypeMember)en.Current;
                    if (Options.BlankLinesBetweenMembers)
                    {
                        Output.WriteLine();
                    }
                    if (CurrentMember.StartDirectives.Count > 0)
                    {
                        GenerateDirectives(CurrentMember.StartDirectives);
                    }
                    GenerateCommentStatements(CurrentMember.Comments);
                    CodeConstructor imp = (CodeConstructor)en.Current;
                    if (imp.LinePragma != null) GenerateLinePragmaStart(imp.LinePragma);
                    GenerateConstructor(imp, e);
                    if (imp.LinePragma != null) GenerateLinePragmaEnd(imp.LinePragma);
                    if (CurrentMember.EndDirectives.Count > 0)
                    {
                        GenerateDirectives(CurrentMember.EndDirectives);
                    }
                }
            }
        }

        public void GenerateEvents(CodeTypeDeclaration e)
        {
            IEnumerator en = e.Members.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current is CodeMemberEvent)
                {
                    CurrentMember = (CodeTypeMember)en.Current;
                    if (Options.BlankLinesBetweenMembers)
                    {
                        Output.WriteLine();
                    }
                    if (CurrentMember.StartDirectives.Count > 0)
                    {
                        GenerateDirectives(CurrentMember.StartDirectives);
                    }
                    GenerateCommentStatements(CurrentMember.Comments);
                    CodeMemberEvent imp = (CodeMemberEvent)en.Current;
                    if (imp.LinePragma != null) GenerateLinePragmaStart(imp.LinePragma);
                    GenerateEvent(imp, e);
                    if (imp.LinePragma != null) GenerateLinePragmaEnd(imp.LinePragma);
                    if (CurrentMember.EndDirectives.Count > 0)
                    {
                        GenerateDirectives(CurrentMember.EndDirectives);
                    }
                }
            }
        }

        public void GenerateExpression(CodeExpression e)
        {
            if (e is CodeArrayCreateExpression)
            {
                GenerateArrayCreateExpression((CodeArrayCreateExpression)e);
            }
            else if (e is CodeBaseReferenceExpression)
            {
                GenerateBaseReferenceExpression((CodeBaseReferenceExpression)e);
            }
            else if (e is CodeBinaryOperatorExpression)
            {
                GenerateBinaryOperatorExpression((CodeBinaryOperatorExpression)e);
            }
            else if (e is CodeCastExpression)
            {
                GenerateCastExpression((CodeCastExpression)e);
            }
            else if (e is CodeDelegateCreateExpression)
            {
                GenerateDelegateCreateExpression((CodeDelegateCreateExpression)e);
            }
            else if (e is CodeFieldReferenceExpression)
            {
                GenerateFieldReferenceExpression((CodeFieldReferenceExpression)e);
            }
            else if (e is CodeArgumentReferenceExpression)
            {
                GenerateArgumentReferenceExpression((CodeArgumentReferenceExpression)e);
            }
            else if (e is CodeVariableReferenceExpression)
            {
                GenerateVariableReferenceExpression((CodeVariableReferenceExpression)e);
            }
            else if (e is CodeIndexerExpression)
            {
                GenerateIndexerExpression((CodeIndexerExpression)e);
            }
            else if (e is CodeArrayIndexerExpression)
            {
                GenerateArrayIndexerExpression((CodeArrayIndexerExpression)e);
            }
            else if (e is CodeSnippetExpression)
            {
                GenerateSnippetExpression((CodeSnippetExpression)e);
            }
            else if (e is CodeMethodInvokeExpression)
            {
                GenerateMethodInvokeExpression((CodeMethodInvokeExpression)e);
            }
            else if (e is CodeMethodReferenceExpression)
            {
                GenerateMethodReferenceExpression((CodeMethodReferenceExpression)e);
            }
            else if (e is CodeEventReferenceExpression)
            {
                GenerateEventReferenceExpression((CodeEventReferenceExpression)e);
            }
            else if (e is CodeDelegateInvokeExpression)
            {
                GenerateDelegateInvokeExpression((CodeDelegateInvokeExpression)e);
            }
            else if (e is CodeObjectCreateExpression)
            {
                GenerateObjectCreateExpression((CodeObjectCreateExpression)e);
            }
            else if (e is CodeParameterDeclarationExpression)
            {
                GenerateParameterDeclarationExpression((CodeParameterDeclarationExpression)e);
            }
            else if (e is CodeDirectionExpression)
            {
                GenerateDirectionExpression((CodeDirectionExpression)e);
            }
            else if (e is CodePrimitiveExpression)
            {
                GeneratePrimitiveExpression((CodePrimitiveExpression)e);
            }
            else if (e is CodePropertyReferenceExpression)
            {
                GeneratePropertyReferenceExpression((CodePropertyReferenceExpression)e);
            }
            else if (e is CodePropertySetValueReferenceExpression)
            {
                GeneratePropertySetValueReferenceExpression((CodePropertySetValueReferenceExpression)e);
            }
            else if (e is CodeThisReferenceExpression)
            {
                GenerateThisReferenceExpression((CodeThisReferenceExpression)e);
            }
            else if (e is CodeTypeReferenceExpression)
            {
                GenerateTypeReferenceExpression((CodeTypeReferenceExpression)e);
            }
            else if (e is CodeTypeOfExpression)
            {
                GenerateTypeOfExpression((CodeTypeOfExpression)e);
            }
            else if (e is CodeDefaultValueExpression)
            {
                GenerateDefaultValueExpression((CodeDefaultValueExpression)e);
            }
            else
            {
                if (e == null)
                {
                    throw new ArgumentNullException("e");
                }
                throw new ArgumentException();
            }
        }

        public void GenerateFields(CodeTypeDeclaration e)
        {
            IEnumerator en = e.Members.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current is CodeMemberField)
                {
                    CurrentMember = (CodeTypeMember)en.Current;
                    if (Options.BlankLinesBetweenMembers)
                    {
                        Output.WriteLine();
                    }
                    if (CurrentMember.StartDirectives.Count > 0)
                    {
                        GenerateDirectives(CurrentMember.StartDirectives);
                    }
                    GenerateCommentStatements(CurrentMember.Comments);
                    CodeMemberField imp = (CodeMemberField)en.Current;
                    if (imp.LinePragma != null) GenerateLinePragmaStart(imp.LinePragma);
                    GenerateField(imp);
                    if (imp.LinePragma != null) GenerateLinePragmaEnd(imp.LinePragma);
                    if (CurrentMember.EndDirectives.Count > 0)
                    {
                        GenerateDirectives(CurrentMember.EndDirectives);
                    }
                }
            }
        }

        public void GenerateSnippetMembers(CodeTypeDeclaration e)
        {
            IEnumerator en = e.Members.GetEnumerator();
            bool hasSnippet = false;
            while (en.MoveNext())
            {
                if (en.Current is CodeSnippetTypeMember)
                {
                    hasSnippet = true;
                    CurrentMember = (CodeTypeMember)en.Current;
                    if (Options.BlankLinesBetweenMembers)
                    {
                        Output.WriteLine();
                    }
                    if (CurrentMember.StartDirectives.Count > 0)
                    {
                        GenerateDirectives(CurrentMember.StartDirectives);
                    }
                    GenerateCommentStatements(CurrentMember.Comments);
                    CodeSnippetTypeMember imp = (CodeSnippetTypeMember)en.Current;
                    if (imp.LinePragma != null) GenerateLinePragmaStart(imp.LinePragma);
                    int savedIndent = Indent;
                    Indent = 0;
                    GenerateSnippetMember(imp);
                    Indent = savedIndent;
                    if (imp.LinePragma != null) GenerateLinePragmaEnd(imp.LinePragma);
                    if (CurrentMember.EndDirectives.Count > 0)
                    {
                        GenerateDirectives(CurrentMember.EndDirectives);
                    }
                }
            }
            if (hasSnippet)
            {
                Output.WriteLine();
            }
        }

        protected virtual void GenerateSnippetCompileUnit(CodeSnippetCompileUnit e)
        {
            GenerateDirectives(e.StartDirectives);
            if (e.LinePragma != null) GenerateLinePragmaStart(e.LinePragma);
            Output.WriteLine(e.Value);
            if (e.LinePragma != null) GenerateLinePragmaEnd(e.LinePragma);
            if (e.EndDirectives.Count > 0)
            {
                GenerateDirectives(e.EndDirectives);
            }
        }

        public void GenerateMethods(CodeTypeDeclaration e)
        {
            IEnumerator en = e.Members.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current is CodeMemberMethod && !(en.Current is CodeTypeConstructor) && !(en.Current is CodeConstructor))
                {
                    CurrentMember = (CodeTypeMember)en.Current;
                    if (Options.BlankLinesBetweenMembers)
                    {
                        Output.WriteLine();
                    }
                    if (CurrentMember.StartDirectives.Count > 0)
                    {
                        GenerateDirectives(CurrentMember.StartDirectives);
                    }
                    GenerateCommentStatements(CurrentMember.Comments);
                    CodeMemberMethod imp = (CodeMemberMethod)en.Current;
                    if (imp.LinePragma != null) GenerateLinePragmaStart(imp.LinePragma);
                    if (en.Current is CodeEntryPointMethod)
                    {
                        GenerateEntryPointMethod((CodeEntryPointMethod)en.Current, e);
                    }
                    else
                    {
                        GenerateMethod(imp, e);
                    }
                    if (imp.LinePragma != null) GenerateLinePragmaEnd(imp.LinePragma);
                    if (CurrentMember.EndDirectives.Count > 0)
                    {
                        GenerateDirectives(CurrentMember.EndDirectives);
                    }
                }
            }
        }

        public void GenerateNestedTypes(CodeTypeDeclaration e)
        {
            IEnumerator en = e.Members.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current is CodeTypeDeclaration)
                {
                    if (Options.BlankLinesBetweenMembers)
                    {
                        Output.WriteLine();
                    }
                    CodeTypeDeclaration currentClass = (CodeTypeDeclaration)en.Current;
                    ((ICodeGenerator)this).GenerateCodeFromType(currentClass, output.InnerWriter, Options);
                }
            }
        }

        protected abstract void GenerateCompileUnit(CodeCompileUnit e);

        protected virtual void GenerateNamespace(CodeNamespace e)
        {
            GenerateCommentStatements(e.Comments);
            GenerateNamespaceStart(e);
            GenerateNamespaceImports(e);
            Output.WriteLine("");
            GenerateTypes(e);
            GenerateNamespaceEnd(e);
        }

        public void GenerateNamespaceImports(CodeNamespace e)
        {
            IEnumerator en = e.Imports.GetEnumerator();
            while (en.MoveNext())
            {
                CodeNamespaceImport imp = (CodeNamespaceImport)en.Current;
                if (imp.LinePragma != null) GenerateLinePragmaStart(imp.LinePragma);
                GenerateNamespaceImport(imp);
                if (imp.LinePragma != null) GenerateLinePragmaEnd(imp.LinePragma);
            }
        }

        public void GenerateProperties(CodeTypeDeclaration e)
        {
            IEnumerator en = e.Members.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current is CodeMemberProperty)
                {
                    CurrentMember = (CodeTypeMember)en.Current;
                    if (Options.BlankLinesBetweenMembers)
                    {
                        Output.WriteLine();
                    }
                    if (CurrentMember.StartDirectives.Count > 0)
                    {
                        GenerateDirectives(CurrentMember.StartDirectives);
                    }
                    GenerateCommentStatements(CurrentMember.Comments);
                    CodeMemberProperty imp = (CodeMemberProperty)en.Current;
                    if (imp.LinePragma != null) GenerateLinePragmaStart(imp.LinePragma);
                    GenerateProperty(imp, e);
                    if (imp.LinePragma != null) GenerateLinePragmaEnd(imp.LinePragma);
                    if (CurrentMember.EndDirectives.Count > 0)
                    {
                        GenerateDirectives(CurrentMember.EndDirectives);
                    }
                }
            }
        }

        public void GenerateStatement(CodeStatement e)
        {
            if (e.StartDirectives.Count > 0)
            {
                GenerateDirectives(e.StartDirectives);
            }
            if (e.LinePragma != null)
            {
                GenerateLinePragmaStart(e.LinePragma);
            }
            if (e is CodeCommentStatement)
            {
                GenerateCommentStatement((CodeCommentStatement)e);
            }
            else if (e is CodeMethodReturnStatement)
            {
                GenerateMethodReturnStatement((CodeMethodReturnStatement)e);
            }
            else if (e is CodeConditionStatement)
            {
                GenerateConditionStatement((CodeConditionStatement)e);
            }
            else if (e is CodeTryCatchFinallyStatement)
            {
                GenerateTryCatchFinallyStatement((CodeTryCatchFinallyStatement)e);
            }
            else if (e is CodeAssignStatement)
            {
                GenerateAssignStatement((CodeAssignStatement)e);
            }
            else if (e is CodeExpressionStatement)
            {
                GenerateExpressionStatement((CodeExpressionStatement)e);
            }
            else if (e is CodeIterationStatement)
            {
                GenerateIterationStatement((CodeIterationStatement)e);
            }
            else if (e is CodeThrowExceptionStatement)
            {
                GenerateThrowExceptionStatement((CodeThrowExceptionStatement)e);
            }
            else if (e is CodeSnippetStatement)
            {
                int savedIndent = Indent;
                Indent = 0;
                GenerateSnippetStatement((CodeSnippetStatement)e);
                Indent = savedIndent;
            }
            else if (e is CodeVariableDeclarationStatement)
            {
                GenerateVariableDeclarationStatement((CodeVariableDeclarationStatement)e);
            }
            else if (e is CodeAttachEventStatement)
            {
                GenerateAttachEventStatement((CodeAttachEventStatement)e);
            }
            else if (e is CodeRemoveEventStatement)
            {
                GenerateRemoveEventStatement((CodeRemoveEventStatement)e);
            }
            else if (e is CodeGotoStatement)
            {
                GenerateGotoStatement((CodeGotoStatement)e);
            }
            else if (e is CodeLabeledStatement)
            {
                GenerateLabeledStatement((CodeLabeledStatement)e);
            }
            else
            {
                throw new ArgumentException();
            }
            if (e.LinePragma != null)
            {
                GenerateLinePragmaEnd(e.LinePragma);
            }
            if (e.EndDirectives.Count > 0)
            {
                GenerateDirectives(e.EndDirectives);
            }
        }

        public void GenerateStatements(CodeStatementCollection stms)
        {
            IEnumerator en = stms.GetEnumerator();
            while (en.MoveNext())
            {
                ((ICodeGenerator)this).GenerateCodeFromStatement((CodeStatement)en.Current, output.InnerWriter, Options);
            }
        }

        protected virtual void OutputAttributeDeclarations(CodeAttributeDeclarationCollection attributes)
        {
            if (attributes.Count == 0) return;
            GenerateAttributeDeclarationsStart(attributes);
            bool first = true;
            IEnumerator en = attributes.GetEnumerator();
            while (en.MoveNext())
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    ContinueOnNewLine(", ");
                }
                CodeAttributeDeclaration current = (CodeAttributeDeclaration)en.Current;
                Output.Write(current.Name);
                Output.Write("(");
                bool firstArg = true;
                foreach (CodeAttributeArgument arg in current.Arguments)
                {
                    if (firstArg)
                    {
                        firstArg = false;
                    }
                    else
                    {
                        Output.Write(", ");
                    }
                    OutputAttributeArgument(arg);
                }
                Output.Write(")");
            }
            GenerateAttributeDeclarationsEnd(attributes);
        }

        protected virtual void OutputAttributeArgument(CodeAttributeArgument arg)
        {
            if (arg.Name != null && arg.Name.Length > 0)
            {
                OutputIdentifier(arg.Name);
                Output.Write("=");
            }
            ((ICodeGenerator)this).GenerateCodeFromExpression(arg.Value, output.InnerWriter, Options);
        }

        protected virtual void OutputDirection(FieldDirection dir)
        {
            switch (dir)
            {
                case FieldDirection.In:
                    break;
                case FieldDirection.Out:
                    Output.Write("out ");
                    break;
                case FieldDirection.Ref:
                    Output.Write("ref ");
                    break;
            }
        }

        protected virtual void OutputFieldScopeModifier(MemberAttributes attributes)
        {
            switch (attributes & MemberAttributes.VTableMask)
            {
                case MemberAttributes.New:
                    Output.Write("new ");
                    break;
            }
            switch (attributes & MemberAttributes.ScopeMask)
            {
                case MemberAttributes.Final:
                    break;
                case MemberAttributes.Static:
                    Output.Write("static ");
                    break;
                case MemberAttributes.Const:
                    Output.Write("const ");
                    break;
                default:
                    break;
            }
        }

        protected virtual void OutputMemberAccessModifier(MemberAttributes attributes)
        {
            switch (attributes & MemberAttributes.AccessMask)
            {
                case MemberAttributes.Assembly:
                    Output.Write("internal ");
                    break;
                case MemberAttributes.FamilyAndAssembly:
                    Output.Write("internal ");
                    break;
                case MemberAttributes.Family:
                    Output.Write("protected ");
                    break;
                case MemberAttributes.FamilyOrAssembly:
                    Output.Write("protected internal ");
                    break;
                case MemberAttributes.Private:
                    Output.Write("private ");
                    break;
                case MemberAttributes.Public:
                    Output.Write("public ");
                    break;
            }
        }

        protected virtual void OutputMemberScopeModifier(MemberAttributes attributes)
        {
            switch (attributes & MemberAttributes.VTableMask)
            {
                case MemberAttributes.New:
                    Output.Write("new ");
                    break;
            }
            switch (attributes & MemberAttributes.ScopeMask)
            {
                case MemberAttributes.Abstract:
                    Output.Write("abstract ");
                    break;
                case MemberAttributes.Final:
                    Output.Write("");
                    break;
                case MemberAttributes.Static:
                    Output.Write("static ");
                    break;
                case MemberAttributes.Override:
                    Output.Write("override ");
                    break;
                default:
                    switch (attributes & MemberAttributes.AccessMask)
                    {
                        case MemberAttributes.Family:
                        case MemberAttributes.Public:
                            Output.Write("virtual ");
                            break;
                        default:
                            break;
                    }
                    break;
            }
        }

        protected abstract void OutputType(CodeTypeReference typeRef);

        protected virtual void OutputTypeAttributes(TypeAttributes attributes, bool isStruct, bool isEnum)
        {
            switch (attributes & TypeAttributes.VisibilityMask)
            {
                case TypeAttributes.Public:
                case TypeAttributes.NestedPublic:
                    Output.Write("public ");
                    break;
                case TypeAttributes.NestedPrivate:
                    Output.Write("private ");
                    break;
            }
            if (isStruct)
            {
                Output.Write("struct ");
            }
            else if (isEnum)
            {
                Output.Write("enum ");
            }
            else
            {
                switch (attributes & TypeAttributes.ClassSemanticsMask)
                {
                    case TypeAttributes.Class:
                        if ((attributes & TypeAttributes.Sealed) == TypeAttributes.Sealed)
                        {
                            Output.Write("sealed ");
                        }
                        if ((attributes & TypeAttributes.Abstract) == TypeAttributes.Abstract)
                        {
                            Output.Write("abstract ");
                        }
                        Output.Write("class ");
                        break;
                    case TypeAttributes.Interface:
                        Output.Write("interface ");
                        break;
                }
            }
        }

        protected virtual void OutputTypeNamePair(CodeTypeReference typeRef, string name)
        {
            OutputType(typeRef);
            Output.Write(" ");
            OutputIdentifier(name);
        }

        protected virtual void OutputIdentifier(string ident)
        {
            Output.Write(ident);
        }

        protected virtual void OutputExpressionList(CodeExpressionCollection expressions)
        {
            OutputExpressionList(expressions, false);
        }

        protected virtual void OutputExpressionList(CodeExpressionCollection expressions, bool newlineBetweenItems)
        {
            bool first = true;
            IEnumerator en = expressions.GetEnumerator();
            Indent++;
            while (en.MoveNext())
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    if (newlineBetweenItems)
                    {
                        ContinueOnNewLine(",");
                    }
                    else
                    {
                        Output.Write(", ");
                    }
                }
                ((ICodeGenerator)this).GenerateCodeFromExpression((CodeExpression)en.Current, output.InnerWriter, Options);
            }
            Indent--;
        }

        protected virtual void OutputOperator(CodeBinaryOperatorType op)
        {
            switch (op)
            {
                case CodeBinaryOperatorType.Add:
                    Output.Write("+");
                    break;
                case CodeBinaryOperatorType.Subtract:
                    Output.Write("-");
                    break;
                case CodeBinaryOperatorType.Multiply:
                    Output.Write("*");
                    break;
                case CodeBinaryOperatorType.Divide:
                    Output.Write("/");
                    break;
                case CodeBinaryOperatorType.Modulus:
                    Output.Write("%");
                    break;
                case CodeBinaryOperatorType.Assign:
                    Output.Write("=");
                    break;
                case CodeBinaryOperatorType.IdentityInequality:
                    Output.Write("!=");
                    break;
                case CodeBinaryOperatorType.IdentityEquality:
                    Output.Write("==");
                    break;
                case CodeBinaryOperatorType.ValueEquality:
                    Output.Write("==");
                    break;
                case CodeBinaryOperatorType.BitwiseOr:
                    Output.Write("|");
                    break;
                case CodeBinaryOperatorType.BitwiseAnd:
                    Output.Write("&");
                    break;
                case CodeBinaryOperatorType.BooleanOr:
                    Output.Write("||");
                    break;
                case CodeBinaryOperatorType.BooleanAnd:
                    Output.Write("&&");
                    break;
                case CodeBinaryOperatorType.LessThan:
                    Output.Write("<");
                    break;
                case CodeBinaryOperatorType.LessThanOrEqual:
                    Output.Write("<=");
                    break;
                case CodeBinaryOperatorType.GreaterThan:
                    Output.Write(">");
                    break;
                case CodeBinaryOperatorType.GreaterThanOrEqual:
                    Output.Write(">=");
                    break;
            }
        }

        protected virtual void OutputParameters(CodeParameterDeclarationExpressionCollection parameters)
        {
            bool first = true;
            bool multiline = parameters.Count > ParameterMultilineThreshold;
            if (multiline)
            {
                Indent += 3;
            }
            IEnumerator en = parameters.GetEnumerator();
            while (en.MoveNext())
            {
                CodeParameterDeclarationExpression current = (CodeParameterDeclarationExpression)en.Current;
                if (first)
                {
                    first = false;
                }
                else
                {
                    Output.Write(", ");
                }
                if (multiline)
                {
                    ContinueOnNewLine("");
                }
                GenerateExpression(current);
            }
            if (multiline)
            {
                Indent -= 3;
            }
        }

        protected abstract void GenerateArrayCreateExpression(CodeArrayCreateExpression e);

        protected abstract void GenerateBaseReferenceExpression(CodeBaseReferenceExpression e);

        protected virtual void GenerateBinaryOperatorExpression(CodeBinaryOperatorExpression e)
        {
            bool indentedExpression = false;
            Output.Write("(");
            GenerateExpression(e.Left);
            Output.Write(" ");
            if (e.Left is CodeBinaryOperatorExpression || e.Right is CodeBinaryOperatorExpression)
            {
                if (!inNestedBinary)
                {
                    indentedExpression = true;
                    inNestedBinary = true;
                    Indent += 3;
                }
                ContinueOnNewLine("");
            }
            OutputOperator(e.Operator);
            Output.Write(" ");
            GenerateExpression(e.Right);
            Output.Write(")");
            if (indentedExpression)
            {
                Indent -= 3;
                inNestedBinary = false;
            }
        }

        protected virtual void ContinueOnNewLine(string st)
        {
            Output.WriteLine(st);
        }

        protected abstract void GenerateCastExpression(CodeCastExpression e);

        protected abstract void GenerateDelegateCreateExpression(CodeDelegateCreateExpression e);

        protected abstract void GenerateFieldReferenceExpression(CodeFieldReferenceExpression e);

        protected abstract void GenerateArgumentReferenceExpression(CodeArgumentReferenceExpression e);

        protected abstract void GenerateVariableReferenceExpression(CodeVariableReferenceExpression e);

        protected abstract void GenerateIndexerExpression(CodeIndexerExpression e);

        protected abstract void GenerateArrayIndexerExpression(CodeArrayIndexerExpression e);

        protected abstract void GenerateSnippetExpression(CodeSnippetExpression e);

        protected abstract void GenerateMethodInvokeExpression(CodeMethodInvokeExpression e);

        protected abstract void GenerateMethodReferenceExpression(CodeMethodReferenceExpression e);

        protected abstract void GenerateEventReferenceExpression(CodeEventReferenceExpression e);

        protected abstract void GenerateDelegateInvokeExpression(CodeDelegateInvokeExpression e);

        protected abstract void GenerateObjectCreateExpression(CodeObjectCreateExpression e);

        protected virtual void GenerateParameterDeclarationExpression(CodeParameterDeclarationExpression e)
        {
            if (e.CustomAttributes.Count > 0)
            {
                OutputAttributeDeclarations(e.CustomAttributes);
                Output.Write(" ");
            }
            OutputDirection(e.Direction);
            OutputTypeNamePair(e.Type, e.Name);
        }

        protected virtual void GenerateDirectionExpression(CodeDirectionExpression e)
        {
            OutputDirection(e.Direction);
            GenerateExpression(e.Expression);
        }

        protected virtual void GeneratePrimitiveExpression(CodePrimitiveExpression e)
        {
            if (e.Value == null)
            {
                Output.Write(NullToken);
            }
            else if (e.Value is string)
            {
                Output.Write(QuoteSnippetString((string)e.Value));
            }
            else if (e.Value is char)
            {
                Output.Write("'" + e.Value + "'");
            }
            else if (e.Value is byte)
            {
                Output.Write(((byte)e.Value).ToString(CultureInfo.InvariantCulture));
            }
            else if (e.Value is short)
            {
                Output.Write(((short)e.Value).ToString(CultureInfo.InvariantCulture));
            }
            else if (e.Value is int)
            {
                Output.Write(((int)e.Value).ToString(CultureInfo.InvariantCulture));
            }
            else if (e.Value is long)
            {
                Output.Write(((long)e.Value).ToString(CultureInfo.InvariantCulture));
            }
            else if (e.Value is float)
            {
                GenerateSingleFloatValue((float)e.Value);
            }
            else if (e.Value is double)
            {
                GenerateDoubleValue((double)e.Value);
            }
            else if (e.Value is decimal)
            {
                GenerateDecimalValue((decimal)e.Value);
            }
            else if (e.Value is bool)
            {
                if ((bool)e.Value)
                {
                    Output.Write("true");
                }
                else
                {
                    Output.Write("false");
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }

        protected virtual void GenerateSingleFloatValue(float s)
        {
            Output.Write(s.ToString("R", CultureInfo.InvariantCulture));
        }

        protected virtual void GenerateDoubleValue(double d)
        {
            Output.Write(d.ToString("R", CultureInfo.InvariantCulture));
        }

        protected virtual void GenerateDecimalValue(decimal d)
        {
            Output.Write(d.ToString(CultureInfo.InvariantCulture));
        }

        protected virtual void GenerateDefaultValueExpression(CodeDefaultValueExpression e)
        {
            Output.Write("default(");
            OutputType(e.Type);
            Output.Write(")");
        }

        protected abstract void GeneratePropertyReferenceExpression(CodePropertyReferenceExpression e);

        protected abstract void GeneratePropertySetValueReferenceExpression(CodePropertySetValueReferenceExpression e);

        protected abstract void GenerateThisReferenceExpression(CodeThisReferenceExpression e);

        protected virtual void GenerateTypeReferenceExpression(CodeTypeReferenceExpression e)
        {
            OutputType(e.Type);
        }

        protected virtual void GenerateTypeOfExpression(CodeTypeOfExpression e)
        {
            Output.Write("typeof(");
            OutputType(e.Type);
            Output.Write(")");
        }

        protected abstract void GenerateExpressionStatement(CodeExpressionStatement e);

        protected abstract void GenerateIterationStatement(CodeIterationStatement e);

        protected abstract void GenerateThrowExceptionStatement(CodeThrowExceptionStatement e);

        protected virtual void GenerateCommentStatement(CodeCommentStatement e)
        {
            if (e.Comment == null)
            {
                throw new ArgumentException();
            }
            GenerateComment(e.Comment);
        }

        protected virtual void GenerateCommentStatements(CodeCommentStatementCollection e)
        {
            foreach (CodeCommentStatement comment in e)
            {
                GenerateCommentStatement(comment);
            }
        }

        protected abstract void GenerateComment(CodeComment e);

        protected abstract void GenerateMethodReturnStatement(CodeMethodReturnStatement e);

        protected abstract void GenerateConditionStatement(CodeConditionStatement e);

        protected abstract void GenerateTryCatchFinallyStatement(CodeTryCatchFinallyStatement e);

        protected abstract void GenerateAssignStatement(CodeAssignStatement e);

        protected abstract void GenerateAttachEventStatement(CodeAttachEventStatement e);

        protected abstract void GenerateRemoveEventStatement(CodeRemoveEventStatement e);

        protected abstract void GenerateGotoStatement(CodeGotoStatement e);

        protected abstract void GenerateLabeledStatement(CodeLabeledStatement e);

        protected virtual void GenerateSnippetStatement(CodeSnippetStatement e)
        {
            Output.WriteLine(e.Value);
        }

        protected abstract void GenerateVariableDeclarationStatement(CodeVariableDeclarationStatement e);

        protected abstract void GenerateLinePragmaStart(CodeLinePragma e);

        protected abstract void GenerateLinePragmaEnd(CodeLinePragma e);

        protected abstract void GenerateEvent(CodeMemberEvent e, CodeTypeDeclaration c);

        protected abstract void GenerateField(CodeMemberField e);

        protected abstract void GenerateSnippetMember(CodeSnippetTypeMember e);

        protected abstract void GenerateEntryPointMethod(CodeEntryPointMethod e, CodeTypeDeclaration c);

        protected abstract void GenerateMethod(CodeMemberMethod e, CodeTypeDeclaration c);

        protected abstract void GenerateProperty(CodeMemberProperty e, CodeTypeDeclaration c);

        protected abstract void GenerateConstructor(CodeConstructor e, CodeTypeDeclaration c);

        protected abstract void GenerateTypeConstructor(CodeTypeConstructor e);

        protected abstract void GenerateTypeStart(CodeTypeDeclaration e);

        protected abstract void GenerateTypeEnd(CodeTypeDeclaration e);

        protected virtual void GenerateCompileUnitStart(CodeCompileUnit e)
        {
            if (e.StartDirectives.Count > 0)
            {
                GenerateDirectives(e.StartDirectives);
            }

            WriteUsingsOutsideNamespaceIfApplicable(e);
        }

        private void WriteUsingsOutsideNamespaceIfApplicable(CodeCompileUnit e)
        {
            usingsAlreadyWritten = true;

            CodeNamespaceImportCollection c = GatherImports(e);

            if (c.Count > 0)
                foreach (CodeNamespaceImport i in c)
                {
                    GenerateNamespaceImport(i);
                }
        }

        private CodeNamespaceImportCollection GatherImports(CodeCompileUnit e)
        {
            CodeNamespaceImportCollection c = new CodeNamespaceImportCollection();

            if (e.Namespaces != null && e.Namespaces.Count > 1)
            {
                foreach (CodeNamespace ns in e.Namespaces)
                {
                    c.AddRange(ns.Imports.Cast<CodeNamespaceImport>().ToArray());
                }
            }

            return c;
        }

        protected virtual void GenerateCompileUnitEnd(CodeCompileUnit e)
        {
            if (e.EndDirectives.Count > 0)
            {
                GenerateDirectives(e.EndDirectives);
            }
        }

        protected abstract void GenerateNamespaceStart(CodeNamespace e);

        protected abstract void GenerateNamespaceEnd(CodeNamespace e);

        protected abstract void GenerateNamespaceImport(CodeNamespaceImport e);

        protected abstract void GenerateAttributeDeclarationsStart(CodeAttributeDeclarationCollection attributes);

        protected abstract void GenerateAttributeDeclarationsEnd(CodeAttributeDeclarationCollection attributes);

        protected abstract bool Supports(GeneratorSupport support);

        protected abstract bool IsValidIdentifier(string value);

        protected virtual void ValidateIdentifier(string value)
        {
        }

        protected abstract string CreateEscapedIdentifier(string value);

        protected abstract string CreateValidIdentifier(string value);

        protected abstract string GetTypeOutput(CodeTypeReference value);

        protected abstract string QuoteSnippetString(string value);

        protected virtual bool IsValidLanguageIndependentIdentifier(string value)
        {
            return IsValidTypeNameOrIdentifier(value, false);
        }

        protected virtual bool IsValidLanguageIndependentTypeName(string value)
        {
            return IsValidTypeNameOrIdentifier(value, true);
        }

        protected virtual bool IsValidTypeNameOrIdentifier(string value, bool isTypeName)
        {
            bool nextMustBeStartChar = true;
            if (value.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];
                UnicodeCategory uc = char.GetUnicodeCategory(ch);
                switch (uc)
                {
                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.TitlecaseLetter:
                    case UnicodeCategory.ModifierLetter:
                    case UnicodeCategory.LetterNumber:
                    case UnicodeCategory.OtherLetter:
                        nextMustBeStartChar = false;
                        break;
                    case UnicodeCategory.NonSpacingMark:
                    case UnicodeCategory.SpacingCombiningMark:
                    case UnicodeCategory.ConnectorPunctuation:
                    case UnicodeCategory.DecimalDigitNumber:
                        if (nextMustBeStartChar && ch != '_')
                        {
                            return false;
                        }
                        nextMustBeStartChar = false;
                        break;
                    default:
                        if (isTypeName && IsSpecialTypeChar(ch, ref nextMustBeStartChar))
                        {
                            break;
                        }
                        return false;
                }
            }
            return true;
        }

        protected virtual bool IsSpecialTypeChar(char ch, ref bool nextMustBeStartChar)
        {
            switch (ch)
            {
                case ':':
                case '.':
                case '$':
                case '+':
                case '<':
                case '>':
                case '-':
                case '[':
                case ']':
                case ',':
                case '&':
                case '*':
                    nextMustBeStartChar = true;
                    return true;
                case '`':
                    return true;
            }
            return false;
        }
    }
}
