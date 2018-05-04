// -----------------------------------------------------------------------
// <copyright file="CodeDomCodeGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2018 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace PowerShell.EditorFeatures.Core.Parsing
{
    public partial class CodeDomCodeGenerator : ICodeGenerator, ICodeParser
    {
        private readonly Hashtable codedomVar = new Hashtable();


        public static CodeGeneratorOptions DefaultOptions;

        private string MakeName(string name)
        {
            string retVal = CodeIdentifier.MakeCamel(CodeIdentifier.MakeValid(name));
            Debug.Print($"Making name {name,-50} -> '{retVal}'");
            return retVal;
        }

        private string GetNextVar(string name)
        {
            if (name == string.Empty)
            {
                name = "codedom";
            }

            if (codedomVar.ContainsKey(name))
            {
                int id = (int) codedomVar[name] + 0b1;
                codedomVar[name] = id;
                return MakeName(name) + id;
            }

            codedomVar[name] = 0b1;
            return MakeName(name);
        }

        private string GenerateCodeAttributeArgument(string context, CodeAttributeArgument argument, TextWriter w, CodeGeneratorOptions o)
        {
            string name = GetNextVar(argument.Name + "Argument");
            string exprName = GenerateCodeExpression(null, argument.Value, w, o);
            Debug.Assert(exprName != null);
            w.WriteLine(@"CodeAttributeArgument {0} = new CodeAttributeArgument(""{1}"", {2});", name, argument.Name, exprName);
            if (!string.IsNullOrEmpty(context))
            {
                w.WriteLine("{0}.Add({1});", context, name);
                w.WriteLine();
            }

            return name;
        }

        private void GenerateCodeAttributeArguments(string context,
            CodeAttributeArgumentCollection arguments,
            TextWriter w,
            CodeGeneratorOptions o)
        {
            if (arguments.Count > 0b0)
            {
                foreach (CodeAttributeArgument arg in arguments)
                {
                    context = GenerateCodeAttributeArgument(context, arg, w, o);
                }
            }
        }

        private string GenerateCodeAttributeDeclaration(string context,
            CodeAttributeDeclaration attribute,
            TextWriter w,
            CodeGeneratorOptions o)
        {
            string name = GetNextVar(attribute.Name + "Attribute");
            w.WriteLine(@"CodeAttributeDeclaration {0} = new CodeAttributeDeclaration(""{1}"");", name, attribute.Name);
            GenerateCodeAttributeArguments(string.Format("{0}.Arguments"), attribute.Arguments, w, o);
            if (context != null && context.Length > 0b0)
            {
                w.WriteLine("{0}.Add({1});", context, name);
                w.WriteLine();
            }

            return name;
        }

        private void GenerateCodeAttributeDeclarations(string context,
            CodeAttributeDeclarationCollection attributes,
            TextWriter w,
            CodeGeneratorOptions o)
        {
            if (attributes.Count > 0b0)
            {
                foreach (CodeAttributeDeclaration attr in attributes)
                {
                    context = GenerateCodeAttributeDeclaration(context, attr, w, o);
                }
            }
        }

        private string GenerateCodeCatchClause(string context, CodeCatchClause catchclause, TextWriter w, CodeGeneratorOptions o)
        {
            string name = GetNextVar(catchclause.LocalName + "Exception");
            w.WriteLine("CodeCatchClause {0} = new CodeCatchClause();", name);
            if (!string.IsNullOrEmpty(catchclause.LocalName))
            {
                w.WriteLine(@"{0}.LocalName = ""{1}"";", name, catchclause.LocalName);
            }

            w.WriteLine("{0}.CatchExceptionType = {1};", name, GenerateCodeTypeReference(null, catchclause.CatchExceptionType, w, o));
            GenerateCodeStatements(string.Format("{0}.Statements", name), catchclause.Statements, w, o);
            if (!string.IsNullOrEmpty(context))
            {
                w.WriteLine("{0}.Add({1});", context, name);
                w.WriteLine();
            }

            return name;
        }

        private void GenerateCodeCatchClauses(string context, CodeCatchClauseCollection catchclauses, TextWriter w, CodeGeneratorOptions o)
        {
            if (catchclauses.Count > 0b0)
            {
                foreach (CodeCatchClause catchclause in catchclauses)
                {
                    context = GenerateCodeCatchClause(context, catchclause, w, o);
                }
            }
        }

        private string GenerateCodeComment(string context, CodeComment comment, TextWriter w, CodeGeneratorOptions o)
        {
            string name = GetNextVar("comment");
            w.WriteLine("CodeComment {0} = new CodeComment();", name);
            if (comment.DocComment)
            {
                w.WriteLine("{0}.DocComment = {1};", name, comment.DocComment.ToString().ToLower());
            }

            if (!string.IsNullOrEmpty(comment.Text))
            {
                w.WriteLine(@"{0}.Text = ""{1}"";", name, comment.Text);
            }

            if (!string.IsNullOrEmpty(context))
            {
                w.WriteLine("{0}.Add({1});", context, name);
                w.WriteLine();
            }

            return name;
        }

        private void GenerateCodeComments(string context, CodeCommentStatementCollection comments, TextWriter w, CodeGeneratorOptions o)
        {
            if (comments.Count > 0b0)
            {
                foreach (CodeCommentStatement comment in comments)
                {
                    w.WriteLine(@"{0}.Add(new CodeCommentStatement(""{1}"", {2}));",
                        context,
                        comment.Comment.Text,
                        comment.Comment.DocComment);
                }
            }
        }

        private void GenerateCodeCompileUnit(CodeCompileUnit e, TextWriter w, CodeGeneratorOptions o)
        {
            string name = GetNextVar("compileunit");
            w.WriteLine("// CodeDom Compile Unit");
            w.WriteLine("CodeCompileUnit {0} = new CodeCompileUnit();", name);
            if (e.ReferencedAssemblies.Count > 0b0)
            {
                if (o.BlankLinesBetweenMembers)
                {
                    w.WriteLine();
                }

                w.WriteLine("// Referenced Assemblies");
                foreach (string refassembly in e.ReferencedAssemblies)
                {
                    w.WriteLine(@"{0}.ReferencedAssemblies.Add(""{1}"");", name, refassembly);
                }
            }

            GenerateCodeNamespaces(string.Format("{0}.Namespaces", name), e.Namespaces, w, o);
        }

        private string GenerateCodeEventReferenceExpression(string context,
            CodeEventReferenceExpression e,
            TextWriter w,
            CodeGeneratorOptions o)
        {
            CodeEventReferenceExpression expr = e;
            string name = GetNextVar("event");
            w.WriteLine("CodeEventReferenceExpression {0} = new CodeEventReferenceExpression();", name);
            w.WriteLine(@"{0}.EventName = ""{1}"";", name, expr.EventName);
            w.WriteLine("{0}.TargetObject = {1};", name, GenerateCodeExpression(null, expr.TargetObject, w, o));
            if (!string.IsNullOrEmpty(context))
            {
                w.WriteLine("{0}.Add({1});", context, name);
                w.WriteLine();
            }

            return name;
        }

        private string GenerateCodeExpression(string context, CodeExpression e, TextWriter w, CodeGeneratorOptions o)
        {
            if (e == null)
            {
                return "null";
            }

            if (e is CodeArrayCreateExpression)
            {
                CodeArrayCreateExpression expr = (CodeArrayCreateExpression) e;
                string name = GetNextVar("arr");
                w.WriteLine("CodeArrayCreateExpression {0} = new CodeArrayCreateExpression();", name);
                if (expr.CreateType != null)
                {
                    string type = GenerateCodeTypeReference(null, expr.CreateType, w, o);
                    w.WriteLine("{0}.CreateType = {1};", name, type);
                }

                GenerateCodeExpressions(string.Format("{0}.Initializers", name), expr.Initializers, w, o);
                w.WriteLine("{0}.Size = {1};", name, expr.Size);
                if (expr.SizeExpression != null)
                {
                    string sizeexpr = GenerateCodeExpression(null, expr.SizeExpression, w, o);
                    w.WriteLine("{0}.SizeExpression = {1};", name, sizeexpr);
                }

                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeBaseReferenceExpression)
            {
                CodeBaseReferenceExpression expr = (CodeBaseReferenceExpression) e;
                string name = GetNextVar("baseExpr");
                w.WriteLine("CodeBaseReferenceExpression {0} = new CodeBaseReferenceExpression();", name);
                if (!string.IsNullOrEmpty(context))
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeBinaryOperatorExpression)
            {
                CodeBinaryOperatorExpression expr = (CodeBinaryOperatorExpression) e;
                string name = GetNextVar("binop");
                w.WriteLine("CodeBinaryOperatorExpression {0} = new CodeBinaryOperatorExpression();", name);
                w.WriteLine("{0}.Left = {1};", name, GenerateCodeExpression(null, expr.Left, w, o));
                w.WriteLine("{0}.Operator = {1};", name, GetCodeBinaryOperatorType(expr.Operator));
                w.WriteLine("{0}.Right = {1};", name, GenerateCodeExpression(null, expr.Right, w, o));
                if (!string.IsNullOrEmpty(context))
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeCastExpression)
            {
                CodeCastExpression expr = (CodeCastExpression) e;
                string name = GetNextVar("castExpr");
                w.WriteLine("CodeCastExpression {0} = new CodeCastExpression();", name);
                w.WriteLine("{0}.Expression = {1};", name, GenerateCodeExpression(null, expr.Expression, w, o));
                w.WriteLine("{0}.TargetType = {1};", name, GenerateCodeTypeReference(null, expr.TargetType, w, o));
                if (!string.IsNullOrEmpty(context))
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeDelegateCreateExpression)
            {
                CodeDelegateCreateExpression expr = (CodeDelegateCreateExpression) e;
                string name = GetNextVar("delegateExpr");
                w.WriteLine("CodeDelegateCreateExpression {0} = new CodeDelegateCreateExpression();", name);
                w.WriteLine("{0}.DelegateType = {1};", name, GenerateCodeTypeReference(null, expr.DelegateType, w, o));
                w.WriteLine(@"{0}.MethodName = ""{1}"";", name, expr.MethodName);
                w.WriteLine("{0}.TargetObject = {1};", name, GenerateCodeExpression(null, expr.TargetObject, w, o));
                if (!string.IsNullOrEmpty(context))
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeFieldReferenceExpression)
            {
                CodeFieldReferenceExpression expr = (CodeFieldReferenceExpression) e;
                string name = GetNextVar("fieldRefExpr");
                w.WriteLine("CodeFieldReferenceExpression {0} = new CodeFieldReferenceExpression();", name);
                w.WriteLine(@"{0}.FieldName = ""{1}"";", name, expr.FieldName);
                w.WriteLine("{0}.TargetObject = {1};", name, GenerateCodeExpression(null, expr.TargetObject, w, o));
                if (!string.IsNullOrEmpty(context))
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeArgumentReferenceExpression)
            {
                CodeArgumentReferenceExpression expr = (CodeArgumentReferenceExpression) e;
                string name = GetNextVar("argExpr");
                w.WriteLine("CodeArgumentReferenceExpression {0} = new CodeArgumentReferenceExpression();", name);
                w.WriteLine(@"{0}.ParameterName = ""{1}"";", name, expr.ParameterName);
                if (!string.IsNullOrEmpty(context))
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeVariableReferenceExpression)
            {
                CodeVariableReferenceExpression expr = (CodeVariableReferenceExpression) e;
                string name = GetNextVar("varExpr");
                w.WriteLine("CodeVariableReferenceExpression {0} = new CodeVariableReferenceExpression();", name);
                w.WriteLine(@"{0}.VariableName = ""{1}"";", name, expr.VariableName);
                if (context?.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeIndexerExpression)
            {
                CodeIndexerExpression expr = (CodeIndexerExpression) e;
                string name = GetNextVar("indexerExpr");
                w.WriteLine("CodeIndexerExpression {0} = new CodeIndexerExpression();", name);
                GenerateCodeExpressions(string.Format("{0}.Indices", name), expr.Indices, w, o);
                w.WriteLine("{0}.TargetObject = {1};", name, GenerateCodeExpression(null, expr.TargetObject, w, o));
                if (context?.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeArrayIndexerExpression)
            {
                CodeArrayIndexerExpression expr = (CodeArrayIndexerExpression) e;
                string name = GetNextVar("arrindexerExpr");
                w.WriteLine("CodeArrayIndexerExpression {0} = new CodeArrayIndexerExpression();", name);
                GenerateCodeExpressions(string.Format("{0}.Indices", name), expr.Indices, w, o);
                w.WriteLine("{0}.TargetObject = {1};", name, GenerateCodeExpression(null, expr.TargetObject, w, o));
                if (!string.IsNullOrEmpty(context))
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeSnippetExpression)
            {
                CodeSnippetExpression expr = (CodeSnippetExpression) e;
                string name = GetNextVar("snippet");
                w.WriteLine("CodeSnippetExpression {0} = new CodeSnippetExpression();", name);
                w.WriteLine(@"{0}.Value = ""{1}"";", name, Escape(expr.Value));
                if (!string.IsNullOrEmpty(context))
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeMethodInvokeExpression)
            {
                CodeMethodInvokeExpression expr = (CodeMethodInvokeExpression) e;
                string name = GetNextVar("invokeExpr");
                w.WriteLine("CodeMethodInvokeExpression {0} = new CodeMethodInvokeExpression();", name);
                GenerateCodeExpressions(string.Format("{0}.Parameters", name), expr.Parameters, w, o);
                w.WriteLine("{0}.Method = {1};", name, GenerateCodeMethodReferenceExpression(null, expr.Method, w, o));
                if (!string.IsNullOrEmpty(context))
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeMethodReferenceExpression)
            {
                CodeMethodReferenceExpression expr = (CodeMethodReferenceExpression) e;
                string name = GetNextVar("methodExpr");
                w.WriteLine("CodeMethodReferenceExpression {0} = new CodeMethodReferenceExpression();", name);
                w.WriteLine(@"{0}.MethodName = ""{1}"";", name, expr.MethodName);
                w.WriteLine("{0}.TargetObject = {1};", name, GenerateCodeExpression(null, expr.TargetObject, w, o));
                if (!string.IsNullOrEmpty(context))
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeEventReferenceExpression)
            {
                return GenerateCodeEventReferenceExpression(context, (CodeEventReferenceExpression) e, w, o);
            }

            if (e is CodeDelegateInvokeExpression)
            {
                CodeDelegateInvokeExpression expr = (CodeDelegateInvokeExpression) e;
                string name = GetNextVar("invokedelegate");
                w.WriteLine("CodeDelegateInvokeExpression {0} = new CodeDelegateInvokeExpression();", name);
                GenerateCodeExpressions(string.Format("{0}.Parameters", name), expr.Parameters, w, o);
                w.WriteLine("{0}.TargetObject = {1};", name, GenerateCodeExpression(null, expr.TargetObject, w, o));
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeObjectCreateExpression)
            {
                CodeObjectCreateExpression expr = (CodeObjectCreateExpression) e;
                string name = GetNextVar("objCreateExpr");
                w.WriteLine("CodeObjectCreateExpression {0} = new CodeObjectCreateExpression();", name);
                w.WriteLine("{0}.CreateType = {1};", name, GenerateCodeTypeReference(null, expr.CreateType, w, o));
                GenerateCodeExpressions(string.Format("{0}.Parameters", name), expr.Parameters, w, o);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeParameterDeclarationExpression)
            {
                return GenerateCodeParameterDeclarationExpression(context, (CodeParameterDeclarationExpression) e, w, o);
            }

            if (e is CodeDirectionExpression)
            {
                CodeDirectionExpression expr = (CodeDirectionExpression) e;
                string name = GetNextVar("dirExpr");
                w.WriteLine("CodeDirectionExpression {0} = new CodeDirectionExpression();", name);
                w.WriteLine("{0}.Direction = {1};", name, GetFieldDirection(expr.Direction));
                w.WriteLine("{0}.Expression = {1};", name, GenerateCodeExpression(null, expr.Expression, w, o));
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodePrimitiveExpression)
            {
                CodePrimitiveExpression expr = (CodePrimitiveExpression) e;
                string name = GetNextVar("primitiveExpr");
                w.WriteLine("CodePrimitiveExpression {0} = new CodePrimitiveExpression();", name);
                if (expr.Value == null)
                {
                    w.WriteLine("{0}.Value = null;", name);
                }
                else if (expr.Value is char)
                {
                    w.WriteLine("{0}.Value = '{1}';", name, expr.Value);
                }
                else if (expr.Value is string)
                {
                    w.WriteLine(@"{0}.Value = ""{1}"";", name, expr.Value);
                }
                else if (expr.Value is bool)
                {
                    w.WriteLine("{0}.Value = {1};", name, expr.Value.ToString().ToLower());
                }
                else
                {
                    w.WriteLine("{0}.Value = {1};", name, expr.Value);
                }

                if (!string.IsNullOrEmpty(context))
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodePropertyReferenceExpression)
            {
                CodePropertyReferenceExpression expr = (CodePropertyReferenceExpression) e;
                string name = GetNextVar("propRefExpr");
                w.WriteLine("CodePropertyReferenceExpression {0} = new CodePropertyReferenceExpression();", name);
                w.WriteLine(@"{0}.PropertyName = ""{1}"";", name, expr.PropertyName);
                w.WriteLine("{0}.TargetObject = {1};", name, GenerateCodeExpression(null, expr.TargetObject, w, o));
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodePropertySetValueReferenceExpression)
            {
                CodePropertySetValueReferenceExpression expr = (CodePropertySetValueReferenceExpression) e;
                string name = GetNextVar("propSetRefExpr");
                w.WriteLine("CodePropertySetValueReferenceExpression {0} = new CodePropertySetValueReferenceExpression();", name);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeThisReferenceExpression)
            {
                CodeThisReferenceExpression expr = (CodeThisReferenceExpression) e;
                string name = GetNextVar("thisExpr");
                w.WriteLine("CodeThisReferenceExpression {0} = new CodeThisReferenceExpression();", name);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeTypeReferenceExpression)
            {
                CodeTypeReferenceExpression expr = (CodeTypeReferenceExpression) e;
                string name = GetNextVar("ref");
                w.WriteLine("CodeTypeReferenceExpression {0} = new CodeTypeReferenceExpression();", name);
                w.WriteLine("{0}.Type = {1};", name, GenerateCodeTypeReference(null, expr.Type, w, o));
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeTypeOfExpression)
            {
                CodeTypeOfExpression expr = (CodeTypeOfExpression) e;
                string name = GetNextVar("typeof");
                w.WriteLine("CodeTypeOfExpression {0} = new CodeTypeOfExpression();", name);
                w.WriteLine("{0}.Type = {1};", name, GenerateCodeTypeReference(null, expr.Type, w, o));
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            Debug.Fail(string.Format("Generator for CodeExpression Type {0} Not Implemented", e.GetType().Name));
            return null;
        }

        private void GenerateCodeExpressions(string context, CodeExpressionCollection exprs, TextWriter w, CodeGeneratorOptions o)
        {
            if (exprs.Count > 0b0)
            {
                foreach (CodeExpression expr in exprs)
                {
                    GenerateCodeExpression(context, expr, w, o);
                }
            }
        }

        private string GenerateCodeDirective(string context, CodeDirective e, TextWriter w, CodeGeneratorOptions o)
        {
            if (e == null)
            {
                return "null";
            }

            if (e is CodeChecksumPragma)
            {
                CodeChecksumPragma directive = (CodeChecksumPragma) e;
                string name = GetNextVar("checksum");
                w.WriteLine("CodeChecksumPragma {0} = new CodeChecksumPragma();", name);
                w.WriteLine(@"{0}.ChecksumAlgorithmId = new Guid(""{1}"");", name, directive.ChecksumAlgorithmId.ToString("B"));
                w.WriteLine("{0}.ChecksumData = new byte[] { {1} };", name, GetByteData(directive.ChecksumData));
                w.WriteLine(@"{0}.FileName = ""{1}"";", name, directive.FileName);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeRegionDirective)
            {
                CodeRegionDirective directive = (CodeRegionDirective) e;
                string name = GetNextVar("region");
                w.WriteLine("CodeRegionDirective {0} = new CodeRegionDirective();", name);
                w.WriteLine("{0}.RegionMode = {1};", name, GetCodeRegionMode(directive.RegionMode));
                w.WriteLine(@"{0}.RegionText = ""{1}"";", name, directive.RegionText);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            Debug.Fail(string.Format("Generator for CodeDirective Type {0} Not Implemented", e.GetType().Name));
            return null;
        }

        private void GenerateCodeDirectives(string context, CodeDirectiveCollection directives, TextWriter w, CodeGeneratorOptions o)
        {
            if (directives.Count > 0b0)
            {
                foreach (CodeDirective directive in directives)
                {
                    GenerateCodeDirective(context, directive, w, o);
                }
            }
        }

        private string GenerateCodeMethodReferenceExpression(string context,
            CodeMethodReferenceExpression methodref,
            TextWriter w,
            CodeGeneratorOptions o)
        {
            string name = GetNextVar(methodref.MethodName + "_method");
            w.WriteLine("CodeMethodReferenceExpression {0} = new CodeMethodReferenceExpression();", name);
            w.WriteLine(@"{0}.MethodName = ""{1}"";", name, methodref.MethodName);
            w.WriteLine("{0}.TargetObject = {1};", name, GenerateCodeExpression(null, methodref.TargetObject, w, o));
            if (context != null && context.Length > 0b0)
            {
                w.WriteLine("{0}.Add({1});", context, name);
                w.WriteLine();
            }

            return name;
        }

        private void GenerateCodeNamespaceImports(string context,
            CodeNamespaceImportCollection imports,
            TextWriter w,
            CodeGeneratorOptions o)
        {
            if (imports.Count > 0b0)
            {
                if (o.BlankLinesBetweenMembers)
                {
                    w.WriteLine();
                }

                w.WriteLine("// Imports");
                foreach (CodeNamespaceImport import in imports)
                {
                    w.WriteLine(@"{0}.Add(new CodeNamespaceImport(""{1}""));", context, import.Namespace);
                }
            }
        }

        private string GenerateCodeNamespace(string context, CodeNamespace ns, TextWriter w, CodeGeneratorOptions o)
        {
            if (o.BlankLinesBetweenMembers)
            {
                w.WriteLine();
            }

            w.WriteLine("//");
            w.WriteLine("// Namespace {0}", ns.Name);
            w.WriteLine("//");
            string name = GetNextVar(ns.Name + "Namespace");
            w.WriteLine(@"CodeNamespace {0} = new CodeNamespace(""{1}"");", name, ns.Name);
            GenerateCodeComments(string.Format("{0}.Comments", name), ns.Comments, w, o);
            GenerateCodeNamespaceImports(string.Format("{0}.Imports", name), ns.Imports, w, o);
            GenerateCodeTypeDeclarations(string.Format("{0}.Types", name), ns.Types, w, o);
            if (context != null && context.Length > 0b0)
            {
                w.WriteLine("{0}.Add({1});", context, name);
                w.WriteLine();
            }

            return name;
        }

        private void GenerateCodeNamespaces(string context, CodeNamespaceCollection namespaces, TextWriter w, CodeGeneratorOptions o)
        {
            if (namespaces.Count > 0b0)
            {
                if (o.BlankLinesBetweenMembers)
                {
                    w.WriteLine();
                }

                foreach (CodeNamespace ns in namespaces)
                {
                    GenerateCodeNamespace(context, ns, w, o);
                }
            }
        }

        private string GenerateCodeParameterDeclarationExpression(string context,
            CodeParameterDeclarationExpression arg,
            TextWriter w,
            CodeGeneratorOptions o)
        {
            string name = GetNextVar(arg.Name + "_arg");
            w.WriteLine(@"CodeParameterDeclarationExpression {0} = new CodeParameterDeclarationExpression({1}, ""{2}"");",
                name,
                GenerateCodeTypeReference(null, arg.Type, w, o),
                arg.Name);
            GenerateCodeAttributeDeclarations(string.Format("{0}.CustomAttributes", name), arg.CustomAttributes, w, o);
            w.WriteLine("{0}.Direction = {1};", name, GetFieldDirection(arg.Direction));
            w.WriteLine(@"{0}.Name = ""{1}"";", name, arg.Name);
            w.WriteLine("{0}.Type = {1};", name, GenerateCodeTypeReference(null, arg.Type, w, o));
            if (context != null && context.Length > 0b0)
            {
                w.WriteLine("{0}.Add({1});", context, name);
                w.WriteLine();
            }

            return name;
        }

        private void GenerateCodeParameterDeclarationExpressions(string context,
            CodeParameterDeclarationExpressionCollection args,
            TextWriter w,
            CodeGeneratorOptions o)
        {
            if (args.Count > 0b0)
            {
                foreach (CodeParameterDeclarationExpression arg in args)
                {
                    GenerateCodeParameterDeclarationExpression(context, arg, w, o);
                }
            }
        }

        private string GenerateCodeStatement(string context, CodeStatement e, TextWriter w, CodeGeneratorOptions o)
        {
            if (e == null)
            {
                return "null";
            }

            if (e is CodeMethodReturnStatement)
            {
                CodeMethodReturnStatement stmt = (CodeMethodReturnStatement) e;
                string name = GetNextVar("return");
                w.WriteLine("CodeMethodReturnStatement {0} = new CodeMethodReturnStatement();", name);
                if (stmt.Expression != null)
                {
                    w.WriteLine("{0}.Expression = {1};", name, GenerateCodeExpression(null, stmt.Expression, w, o));
                }

                GenerateCodeDirectives(string.Format("{0}.StartDirectives", name), stmt.StartDirectives, w, o);
                GenerateCodeDirectives(string.Format("{0}.EndDirectives", name), stmt.EndDirectives, w, o);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeCommentStatement)
            {
                CodeCommentStatement stmt = (CodeCommentStatement) e;
                string name = GetNextVar("comment");
                w.WriteLine("CodeCommentStatement {0} = new CodeCommentStatement();", name);
                w.WriteLine("{0}.Comment = {1};", name, GenerateCodeComment(null, stmt.Comment, w, o));
                GenerateCodeDirectives(string.Format("{0}.StartDirectives", name), stmt.StartDirectives, w, o);
                GenerateCodeDirectives(string.Format("{0}.EndDirectives", name), stmt.EndDirectives, w, o);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            /*
                        if (e is CodeMethodReturnStatement)
                        {
                            CodeMethodReturnStatement stmt = (CodeMethodReturnStatement) e;
                            string name = GetNextVar("return");
                            w.WriteLine(@"CodeMethodReturnStatement {0} = new CodeMethodReturnStatement();", name);
                            if (stmt.Expression != null)
                            {
                                w.WriteLine(@"{0}.Expression = {1};", name, GenerateCodeExpression(null, stmt.Expression, w, o));
                            }
                            GenerateCodeDirectives(string.Format(@"{0}.StartDirectives", name), stmt.StartDirectives, w, o);
                            GenerateCodeDirectives(string.Format(@"{0}.EndDirectives", name), stmt.EndDirectives, w, o);
                            if (context != null && context.Length > 0)
                            {
                                w.WriteLine(@"{0}.Add({1});", context, name);
                                w.WriteLine();
                            }
                            return name;
                        }
            */
            if (e is CodeConditionStatement)
            {
                CodeConditionStatement stmt = (CodeConditionStatement) e;
                string name = GetNextVar("if");
                w.WriteLine("CodeConditionStatement {0} = new CodeConditionStatement();", name);
                w.WriteLine("{0}.Condition = {1};", name, GenerateCodeExpression(null, stmt.Condition, w, o));
                GenerateCodeStatements(string.Format("{0}.TrueStatements", name), stmt.TrueStatements, w, o);
                GenerateCodeStatements(string.Format("{0}.FalseStatements", name), stmt.FalseStatements, w, o);
                GenerateCodeDirectives(string.Format("{0}.StartDirectives", name), stmt.StartDirectives, w, o);
                GenerateCodeDirectives(string.Format("{0}.EndDirectives", name), stmt.EndDirectives, w, o);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeTryCatchFinallyStatement)
            {
                CodeTryCatchFinallyStatement stmt = (CodeTryCatchFinallyStatement) e;
                string name = GetNextVar("try");
                w.WriteLine("CodeTryCatchFinallyStatement {0} = new CodeTryCatchFinallyStatement();", name);
                GenerateCodeStatements(string.Format("{0}.TryStatements", name), stmt.TryStatements, w, o);
                GenerateCodeCatchClauses(string.Format("{0}.CatchClauses", name), stmt.CatchClauses, w, o);
                GenerateCodeStatements(string.Format("{0}.FinallyStatements", name), stmt.FinallyStatements, w, o);
                GenerateCodeDirectives(string.Format("{0}.StartDirectives", name), stmt.StartDirectives, w, o);
                GenerateCodeDirectives(string.Format("{0}.EndDirectives", name), stmt.EndDirectives, w, o);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeAssignStatement)
            {
                CodeAssignStatement stmt = (CodeAssignStatement) e;
                string name = GetNextVar("assign");
                w.WriteLine("CodeAssignStatement {0} = new CodeAssignStatement();", name);
                if (stmt.Left != null)
                {
                    w.WriteLine("{0}.Left = {1};", name, GenerateCodeExpression(null, stmt.Left, w, o));
                }

                if (stmt.Right != null)
                {
                    w.WriteLine("{0}.Right = {1};", name, GenerateCodeExpression(null, stmt.Right, w, o));
                }

                GenerateCodeDirectives(string.Format("{0}.StartDirectives", name), stmt.StartDirectives, w, o);
                GenerateCodeDirectives(string.Format("{0}.EndDirectives", name), stmt.EndDirectives, w, o);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeExpressionStatement)
            {
                CodeExpressionStatement stmt = (CodeExpressionStatement) e;
                string name = GetNextVar("expr");
                w.WriteLine("CodeExpressionStatement {0} = new CodeExpressionStatement();", name);
                if (stmt.Expression != null)
                {
                    w.WriteLine("{0}.Expression = {1};", name, GenerateCodeExpression(null, stmt.Expression, w, o));
                }

                GenerateCodeDirectives(string.Format("{0}.StartDirectives", name), stmt.StartDirectives, w, o);
                GenerateCodeDirectives(string.Format("{0}.EndDirectives", name), stmt.EndDirectives, w, o);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeIterationStatement)
            {
                CodeIterationStatement stmt = (CodeIterationStatement) e;
                string name = GetNextVar("for");
                w.WriteLine("CodeIterationStatement {0} = new CodeIterationStatement();", name);
                if (stmt.InitStatement != null)
                {
                    w.WriteLine("{0}.InitStatement = {1};", name, GenerateCodeStatement(null, stmt.InitStatement, w, o));
                }

                if (stmt.TestExpression != null)
                {
                    w.WriteLine("{0}.TestExpression = {1};", name, GenerateCodeExpression(null, stmt.TestExpression, w, o));
                }

                if (stmt.IncrementStatement != null)
                {
                    w.WriteLine("{0}.IncrementStatement = {1};", name, GenerateCodeStatement(null, stmt.IncrementStatement, w, o));
                }

                GenerateCodeStatements(string.Format("{0}.Statements", name), stmt.Statements, w, o);
                GenerateCodeDirectives(string.Format("{0}.StartDirectives", name), stmt.StartDirectives, w, o);
                GenerateCodeDirectives(string.Format("{0}.EndDirectives", name), stmt.EndDirectives, w, o);
                if (!string.IsNullOrEmpty(context))
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeThrowExceptionStatement)
            {
                CodeThrowExceptionStatement stmt = (CodeThrowExceptionStatement) e;
                string name = GetNextVar("throw");
                w.WriteLine("CodeThrowExceptionStatement {0} = new CodeThrowExceptionStatement();", name);
                if (stmt.ToThrow != null)
                {
                    w.WriteLine("{0}.ToThrow = {1};", name, GenerateCodeExpression(null, stmt.ToThrow, w, o));
                }

                GenerateCodeDirectives(string.Format("{0}.StartDirectives", name), stmt.StartDirectives, w, o);
                GenerateCodeDirectives(string.Format("{0}.EndDirectives", name), stmt.EndDirectives, w, o);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeSnippetStatement)
            {
                CodeSnippetStatement stmt = (CodeSnippetStatement) e;
                string name = GetNextVar("snippet");
                w.WriteLine("CodeSnippetStatement {0} = new CodeSnippetStatement();", name);
                if (stmt.Value != null)
                {
                    w.WriteLine(@"{0}.Value = ""{1}"";", name, stmt.Value);
                }

                GenerateCodeDirectives(string.Format("{0}.StartDirectives", name), stmt.StartDirectives, w, o);
                GenerateCodeDirectives(string.Format("{0}.EndDirectives", name), stmt.EndDirectives, w, o);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeVariableDeclarationStatement)
            {
                CodeVariableDeclarationStatement stmt = (CodeVariableDeclarationStatement) e;
                string name = GetNextVar("decl");
                w.WriteLine("CodeVariableDeclarationStatement {0} = new CodeVariableDeclarationStatement();", name);
                if (stmt.InitExpression != null)
                {
                    w.WriteLine("{0}.InitExpression = {1};", name, GenerateCodeExpression(null, stmt.InitExpression, w, o));
                }

                w.WriteLine(@"{0}.Name = ""{1}"";", name, stmt.Name);
                if (stmt.Type != null)
                {
                    w.WriteLine("{0}.Type = {1};", name, GenerateCodeTypeReference(null, stmt.Type, w, o));
                }

                GenerateCodeDirectives(string.Format("{0}.StartDirectives", name), stmt.StartDirectives, w, o);
                GenerateCodeDirectives(string.Format("{0}.EndDirectives", name), stmt.EndDirectives, w, o);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeAttachEventStatement)
            {
                CodeAttachEventStatement stmt = (CodeAttachEventStatement) e;
                string name = GetNextVar("addevent");
                w.WriteLine("CodeAttachEventStatement {0} = new CodeAttachEventStatement();", name);
                if (stmt.Event != null)
                {
                    w.WriteLine("{0}.Event = {1};", name, GenerateCodeEventReferenceExpression(null, stmt.Event, w, o));
                }

                if (stmt.Listener != null)
                {
                    w.WriteLine("{0}.Listener = {1};", name, GenerateCodeExpression(null, stmt.Listener, w, o));
                }

                GenerateCodeDirectives(string.Format("{0}.StartDirectives", name), stmt.StartDirectives, w, o);
                GenerateCodeDirectives(string.Format("{0}.EndDirectives", name), stmt.EndDirectives, w, o);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeRemoveEventStatement)
            {
                CodeRemoveEventStatement stmt = (CodeRemoveEventStatement) e;
                string name = GetNextVar("removeevent");
                w.WriteLine("CodeRemoveEventStatement {0} = new CodeRemoveEventStatement();", name);
                if (stmt.Event != null)
                {
                    w.WriteLine("{0}.Event = {1};", name, GenerateCodeEventReferenceExpression(null, stmt.Event, w, o));
                }

                if (stmt.Listener != null)
                {
                    w.WriteLine("{0}.Listener = {1};", name, GenerateCodeExpression(null, stmt.Listener, w, o));
                }

                GenerateCodeDirectives(string.Format("{0}.StartDirectives", name), stmt.StartDirectives, w, o);
                GenerateCodeDirectives(string.Format("{0}.EndDirectives", name), stmt.EndDirectives, w, o);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeGotoStatement)
            {
                CodeGotoStatement stmt = (CodeGotoStatement) e;
                string name = GetNextVar("goto");
                w.WriteLine("CodeGotoStatement {0} = new CodeGotoStatement();", name);
                if (stmt.Label != null)
                {
                    w.WriteLine(@"{0}.Label = ""{1}"";", name, stmt.Label);
                }

                GenerateCodeDirectives(string.Format("{0}.StartDirectives", name), stmt.StartDirectives, w, o);
                GenerateCodeDirectives(string.Format("{0}.EndDirectives", name), stmt.EndDirectives, w, o);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (e is CodeLabeledStatement)
            {
                CodeLabeledStatement stmt = (CodeLabeledStatement) e;
                string name = GetNextVar("label");
                w.WriteLine("CodeLabeledStatement {0} = new CodeLabeledStatement();", name);
                if (stmt.Label != null)
                {
                    w.WriteLine(@"{0}.Label = ""{1}"";", name, stmt.Label);
                }

                if (stmt.Statement != null)
                {
                    w.WriteLine("{0}.Statement = {1};", name, GenerateCodeStatement(null, stmt.Statement, w, o));
                }

                GenerateCodeDirectives(string.Format("{0}.StartDirectives", name), stmt.StartDirectives, w, o);
                GenerateCodeDirectives(string.Format("{0}.EndDirectives", name), stmt.EndDirectives, w, o);
                if (!string.IsNullOrEmpty(context))
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            Debug.Fail(string.Format("Generator for CodeStatement {0} Not Implemented", e.GetType().Name));
            return null;
        }

        private void GenerateCodeStatements(string context, CodeStatementCollection stmts, TextWriter w, CodeGeneratorOptions o)
        {
            if (stmts.Count > 0b0)
            {
                foreach (CodeStatement stmt in stmts)
                {
                    GenerateCodeStatement(context, stmt, w, o);
                }
            }
        }

        private string GenerateCodeTypeDeclaration(string context, CodeTypeDeclaration decl, TextWriter w, CodeGeneratorOptions o)
        {
            if (o.BlankLinesBetweenMembers)
            {
                w.WriteLine();
            }

            w.WriteLine("//");
            string decltype = GetDeclarationType(decl);
            w.WriteLine("//");
            w.WriteLine("// {0} {1}", decltype, decl.Name);
            w.WriteLine("//");
            string name = GetNextVar(decl.Name + "_" + decltype);
            w.WriteLine(@"CodeTypeDeclaration {0} = new CodeTypeDeclaration(""{1}"");", name, decl.Name);
            w.WriteLine("{0}.Attributes = {1};", name, GetMemberAttributes(decl.Attributes));
            if (decl.IsPartial)
            {
                w.WriteLine("{0}.IsPartial = true;", name);
            }

            if (decl.IsClass)
            {
                w.WriteLine("{0}.IsClass = true;", name);
            }
            else if (decl.IsEnum)
            {
                w.WriteLine("{0}.IsEnum = true;", name);
            }
            else if (decl.IsInterface)
            {
                w.WriteLine("{0}.IsInterface = true;", name);
            }
            else if (decl.IsStruct)
            {
                w.WriteLine("{0}.IsStruct = true;", name);
            }

            GenerateCodeAttributeDeclarations(string.Format("{0}.CustomAttributes", name), decl.CustomAttributes, w, o);
            GenerateCodeTypeParameters(context, decl.TypeParameters, w, o);
            GenerateCodeTypeReferences(string.Format("{0}.BaseTypes", name), decl.BaseTypes, w, o);
            GenerateCodeComments(string.Format("{0}.Comments", name), decl.Comments, w, o);
            GenerateCodeTypeMembers(string.Format("{0}.Members", name), decl.Members, w, o);
            if (context != null && context.Length > 0b0)
            {
                w.WriteLine("{0}.Add({1});", context, name);
                w.WriteLine();
            }

            return name;
        }

        private static string GetDeclarationType(CodeTypeDeclaration decl)
        {
            string decltype = string.Empty;
            if (decl.IsClass)
            {
                decltype = "class";
            }
            else if (decl.IsEnum)
            {
                decltype = "enum";
            }
            else if (decl.IsInterface)
            {
                decltype = "interface";
            }
            else if (decl.IsStruct)
            {
                decltype = "struct";
            }

            return decltype;
        }

        private void GenerateCodeTypeDeclarations(string context, CodeTypeDeclarationCollection types, TextWriter w, CodeGeneratorOptions o)
        {
            if (types.Count > 0b0)
            {
                foreach (CodeTypeDeclaration decl in types)
                {
                    GenerateCodeTypeDeclaration(context, decl, w, o);
                }
            }
        }

        private string GenerateCodeTypeMember(string context, CodeTypeMember m, TextWriter w, CodeGeneratorOptions o)
        {
            if (o.BlankLinesBetweenMembers)
            {
                w.WriteLine();
            }

            if (m is CodeConstructor)
            {
                CodeConstructor member = (CodeConstructor) m;
                string name = GetNextVar(member.Name + "Constructor");
                string parameters = string.Empty;
                foreach (CodeParameterDeclarationExpression param in member.Parameters)
                {
                    if (parameters.Length > 0b0)
                    {
                        parameters += ", ";
                    }

                    parameters += param.Type.BaseType + " " + param.Name;
                }

                w.WriteLine("//");
                w.WriteLine("// Constructor({0})", parameters);
                w.WriteLine("//");
                w.WriteLine("CodeConstructor {0} = new CodeConstructor();", name);
                w.WriteLine("{0}.Attributes = {1};", name, GetMemberAttributes(member.Attributes));
                GenerateCodeExpressions(string.Format("{0}.BaseConstructorArgs", name), member.BaseConstructorArgs, w, o);
                GenerateCodeExpressions(string.Format("{0}.ChainedConstructorArgs", name), member.ChainedConstructorArgs, w, o);
                GenerateCodeComments(string.Format("{0}.Comments", name), member.Comments, w, o);
                GenerateCodeAttributeDeclarations(string.Format("{0}.CustomAttributes", name), member.CustomAttributes, w, o);
                GenerateCodeTypeReferences(string.Format("{0}.ImplementationTypes", name), member.ImplementationTypes, w, o);
                GenerateCodeParameterDeclarationExpressions(string.Format("{0}.Parameters", name), member.Parameters, w, o);
                if (member.PrivateImplementationType != null)
                {
                    w.WriteLine("{0}.PrivateImplementationType = {1};",
                        name,
                        GenerateCodeTypeReference(null, member.PrivateImplementationType, w, o));
                }

                if (member.ReturnType != null && member.ReturnType.BaseType != "System.Void")
                {
                    string retname = GenerateCodeTypeReference(null, member.ReturnType, w, o);
                    w.WriteLine("{0}.ReturnType = {1};", name, retname);
                }

                GenerateCodeAttributeDeclarations(string.Format("{0}.ReturnTypeCustomAttributes", name),
                    member.ReturnTypeCustomAttributes,
                    w,
                    o);
                GenerateCodeStatements(string.Format("{0}.Statements", name), member.Statements, w, o);
                GenerateCodeDirectives(string.Format("{0}.StartDirectives", name), member.StartDirectives, w, o);
                GenerateCodeDirectives(string.Format("{0}.EndDirectives", name), member.EndDirectives, w, o);
                GenerateCodeTypeParameters(string.Format("{0}.TypeParameters", name), member.TypeParameters, w, o);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (m is CodeMemberField)
            {
                CodeMemberField member = (CodeMemberField) m;
                string name = GetNextVar(member.Name + "_field");
                w.WriteLine("//");
                w.WriteLine("// Field {0}", member.Name);
                w.WriteLine("//");
                w.WriteLine("CodeMemberField {0} = new CodeMemberField();", name);
                w.WriteLine("{0}.Attributes = {1};", name, GetMemberAttributes(member.Attributes));
                GenerateCodeComments(string.Format("{0}.Comments", name), member.Comments, w, o);
                GenerateCodeAttributeDeclarations(string.Format("{0}.CustomAttributes", name), member.CustomAttributes, w, o);
                if (member.InitExpression != null)
                {
                    string init = GenerateCodeExpression(null, member.InitExpression, w, o);
                    w.WriteLine("{0}.InitExpression = {1};", name, init);
                }

                w.WriteLine(@"{0}.Name = ""{1}"";", name, member.Name);
                if (member.Type != null)
                {
                    string type = GenerateCodeTypeReference(null, member.Type, w, o);
                    w.WriteLine("{0}.Type = {1};", name, type);
                }

                GenerateCodeDirectives(string.Format("{0}.StartDirectives", name), member.StartDirectives, w, o);
                GenerateCodeDirectives(string.Format("{0}.EndDirectives", name), member.EndDirectives, w, o);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (m is CodeMemberEvent)
            {
                CodeMemberEvent member = (CodeMemberEvent) m;
                string name = GetNextVar(member.Name + "_event");
                w.WriteLine("//");
                w.WriteLine("// Event {0}", member.Name);
                w.WriteLine("//");
                w.WriteLine("CodeMemberEvent {0} = new CodeMemberEvent();", name);
                w.WriteLine("{0}.Attributes = {1};", name, GetMemberAttributes(member.Attributes));
                GenerateCodeComments(string.Format("{0}.Comments", name), member.Comments, w, o);
                GenerateCodeAttributeDeclarations(string.Format("{0}.CustomAttributes", name), member.CustomAttributes, w, o);
                w.WriteLine(@"{0}.Name = ""{1}"";", name, member.Name);
                if (member.Type != null)
                {
                    w.WriteLine("{0}.Type = {1};", name, GenerateCodeTypeReference(null, member.Type, w, o));
                }

                GenerateCodeTypeReferences(string.Format("{0}.ImplementationTypes", name), member.ImplementationTypes, w, o);
                if (member.PrivateImplementationType != null)
                {
                    w.WriteLine("{0}.PrivateImplementationType = {1};",
                        name,
                        GenerateCodeTypeReference(null, member.PrivateImplementationType, w, o));
                }

                GenerateCodeDirectives(string.Format("{0}.StartDirectives", name), member.StartDirectives, w, o);
                GenerateCodeDirectives(string.Format("{0}.EndDirectives", name), member.EndDirectives, w, o);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (m is CodeMemberProperty)
            {
                CodeMemberProperty member = (CodeMemberProperty) m;
                string name = GetNextVar(member.Name + "Property");
                string parameters = string.Empty;
                foreach (CodeParameterDeclarationExpression param in member.Parameters)
                {
                    if (parameters.Length > 0b0)
                    {
                        parameters += ", ";
                    }

                    parameters += param.Type.BaseType + " " + param.Name;
                }

                w.WriteLine("//");
                if (parameters.Length == 0b0)
                {
                    w.WriteLine("// Property {0}", member.Name);
                }
                else
                {
                    w.WriteLine("// Property {0}[{1}]", member.Name, parameters);
                }

                w.WriteLine("//");
                w.WriteLine("CodeMemberProperty {0} = new CodeMemberProperty();", name);
                w.WriteLine("{0}.Attributes = {1};", name, GetMemberAttributes(member.Attributes));
                GenerateCodeComments(string.Format("{0}.Comments", name), member.Comments, w, o);
                GenerateCodeAttributeDeclarations(string.Format("{0}.CustomAttributes", name), member.CustomAttributes, w, o);
                w.WriteLine(@"{0}.Name = ""{1}"";", name, member.Name);
                w.WriteLine("{0}.Type = {1};", name, GenerateCodeTypeReference(null, member.Type, w, o));
                GenerateCodeParameterDeclarationExpressions(string.Format("{0}.Parameters", name), member.Parameters, w, o);
                w.WriteLine("{0}.HasGet = {1};", name, member.HasGet.ToString().ToLower());
                if (member.HasGet)
                {
                    GenerateCodeStatements(string.Format("{0}.GetStatements", name), member.GetStatements, w, o);
                }

                w.WriteLine("{0}.HasSet = {1};", name, member.HasSet.ToString().ToLower());
                if (member.HasSet)
                {
                    GenerateCodeStatements(string.Format("{0}.SetStatements", name), member.SetStatements, w, o);
                }

                GenerateCodeTypeReferences(string.Format("{0}.ImplementationTypes", name), member.ImplementationTypes, w, o);
                if (member.PrivateImplementationType != null)
                {
                    w.WriteLine("{0}.PrivateImplementationType = {1};",
                        name,
                        GenerateCodeTypeReference(null, member.PrivateImplementationType, w, o));
                }

                GenerateCodeDirectives(string.Format("{0}.StartDirectives", name), member.StartDirectives, w, o);
                GenerateCodeDirectives(string.Format("{0}.EndDirectives", name), member.EndDirectives, w, o);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (m is CodeMemberMethod)
            {
                CodeMemberMethod member = (CodeMemberMethod) m;
                string name = GetNextVar(member.Name + "Method");
                string parameters = string.Empty;
                foreach (CodeParameterDeclarationExpression param in member.Parameters)
                {
                    if (parameters.Length > 0b0)
                    {
                        parameters += ", ";
                    }

                    parameters += param.Type.BaseType + " " + param.Name;
                }

                w.WriteLine("//");
                w.WriteLine("// Function {0}({1})", member.Name, parameters);
                w.WriteLine("//");
                w.WriteLine("CodeMemberMethod {0} = new CodeMemberMethod();", name);
                w.WriteLine("{0}.Attributes = {1};", name, GetMemberAttributes(member.Attributes));
                GenerateCodeComments(string.Format("{0}.Comments", name), member.Comments, w, o);
                GenerateCodeAttributeDeclarations(string.Format("{0}.CustomAttributes", name), member.CustomAttributes, w, o);
                w.WriteLine(@"{0}.Name = ""{1}"";", name, member.Name);
                GenerateCodeParameterDeclarationExpressions(string.Format("{0}.Parameters", name), member.Parameters, w, o);
                if (member.ReturnType != null && member.ReturnType.BaseType != "System.Void")
                {
                    string retname = GenerateCodeTypeReference(null, member.ReturnType, w, o);
                    w.WriteLine("{0}.ReturnType = {1};", name, retname);
                }

                GenerateCodeTypeParameters(string.Format("{0}.TypeParameters", name), member.TypeParameters, w, o);
                GenerateCodeAttributeDeclarations(string.Format("{0}.ReturnTypeCustomAttributes", name),
                    member.ReturnTypeCustomAttributes,
                    w,
                    o);
                GenerateCodeStatements(string.Format("{0}.Statements", name), member.Statements, w, o);
                GenerateCodeTypeReferences(string.Format("{0}.ImplementationTypes", name), member.ImplementationTypes, w, o);
                if (member.PrivateImplementationType != null)
                {
                    w.WriteLine("{0}.PrivateImplementationType = {1};",
                        name,
                        GenerateCodeTypeReference(null, member.PrivateImplementationType, w, o));
                }

                GenerateCodeDirectives(string.Format("{0}.StartDirectives", name), member.StartDirectives, w, o);
                GenerateCodeDirectives(string.Format("{0}.EndDirectives", name), member.EndDirectives, w, o);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            if (m is CodeTypeDelegate)
            {
                CodeTypeDelegate member = (CodeTypeDelegate) m;
                string name = GetNextVar(member.Name + "_delegate");
                string parameters = string.Empty;
                foreach (CodeParameterDeclarationExpression param in member.Parameters)
                {
                    if (parameters.Length > 0b0)
                    {
                        parameters += ", ";
                    }

                    parameters += param.Type.BaseType + " " + param.Name;
                }

                w.WriteLine("//");
                w.WriteLine("// Delegate {0}({1})", member.Name, parameters);
                w.WriteLine("//");
                w.WriteLine("CodeTypeDelegate {0} = new CodeTypeDelegate();", name);
                w.WriteLine("{0}.Attributes = {1};", name, GetMemberAttributes(member.Attributes));
                GenerateCodeTypeReferences(string.Format("{0}.BaseTypes", name), member.BaseTypes, w, o);
                GenerateCodeComments(string.Format("{0}.Comments", name), member.Comments, w, o);
                GenerateCodeAttributeDeclarations(string.Format("{0}.CustomAttributes", name), member.CustomAttributes, w, o);
                w.WriteLine(@"{0}.Name = ""{1}"";", name, member.Name);
                if (member.IsClass)
                {
                    w.WriteLine("{0}.IsClass = true;", name);
                }

                if (member.IsEnum)
                {
                    w.WriteLine("{0}.IsEnum = true;", name);
                }

                if (member.IsInterface)
                {
                    w.WriteLine("{0}.IsInterface = true;", name);
                }

                if (member.IsPartial)
                {
                    w.WriteLine("{0}.IsPartial = true;", name);
                }

                if (member.IsStruct)
                {
                    w.WriteLine("{0}.IsStruct = true;", name);
                }

                GenerateCodeTypeMembers(string.Format("{0}.Members", name), member.Members, w, o);
                GenerateCodeParameterDeclarationExpressions(string.Format("{0}.Parameters", name), member.Parameters, w, o);
                if (member.ReturnType != null && member.ReturnType.BaseType != "System.Void")
                {
                    string retname = GenerateCodeTypeReference(null, member.ReturnType, w, o);
                    w.WriteLine("{0}.ReturnType = {1};", name, retname);
                }

                w.WriteLine("{0}.TypeAttributes = {1};", name, GetTypeAttributes(member.TypeAttributes));
                GenerateCodeTypeParameters(string.Format("{0}.TypeParameters", name), member.TypeParameters, w, o);
                GenerateCodeDirectives(string.Format("{0}.StartDirectives", name), member.StartDirectives, w, o);
                GenerateCodeDirectives(string.Format("{0}.EndDirectives", name), member.EndDirectives, w, o);
                if (context != null && context.Length > 0b0)
                {
                    w.WriteLine("{0}.Add({1});", context, name);
                    w.WriteLine();
                }

                return name;
            }

            Debug.Fail(string.Format("Generator for CodeTypeMember {0} Not Implemented", m.GetType().Name));
            return null;
        }

        private void GenerateCodeTypeMembers(string context, CodeTypeMemberCollection members, TextWriter w, CodeGeneratorOptions o)
        {
            if (members.Count > 0b0)
            {
                foreach (CodeTypeMember member in members)
                {
                    GenerateCodeTypeMember(context, member, w, o);
                }
            }
        }

        private string GenerateCodeTypeParameter(string context, CodeTypeParameter param, TextWriter w, CodeGeneratorOptions o)
        {
            string name = GetNextVar(param.Name + "Parameter");
            w.WriteLine("CodeTypeParameter {0} = new CodeTypeParameter();", name);
            GenerateCodeTypeReferences(string.Format("{0}.Constraints", context), param.Constraints, w, o);
            GenerateCodeAttributeDeclarations(string.Format("{0}.CustomAttributes", name), param.CustomAttributes, w, o);
            w.WriteLine("{0}.HasConstructorConstraint = {1};", name, param.HasConstructorConstraint.ToString().ToLower());
            w.WriteLine(@"{0}.Name = ""{1}"";", name, param.Name);
            if (context != null && context.Length > 0b0)
            {
                w.WriteLine("{0}.Add({1});", context, name);
                w.WriteLine();
            }

            return name;
        }

        private void GenerateCodeTypeParameters(string context, CodeTypeParameterCollection @params, TextWriter w, CodeGeneratorOptions o)
        {
            if (@params.Count > 0b0)
            {
                foreach (CodeTypeParameter param in @params)
                {
                    GenerateCodeTypeParameter(context, param, w, o);
                }
            }
        }

        private string GenerateCodeTypeReference(string context, CodeTypeReference typeref, TextWriter w, CodeGeneratorOptions o)
        {
            CodeGenerationCodeTypeReference codeTypeReference = CodeGenerationCodeTypeReference.From(typeref);
            string name = CodeIdentifier.MakeValid(GetNextVar(typeref.BaseType + "_type"));
            CodeTypeReference elementType = typeref.ArrayElementType;
            if (elementType == null)
            {
                if (typeref.TypeArguments.Count > 0)
                {
                    w.WriteLine(@"CodeTypeReference {0} = new CodeTypeReference(""{1}"", {2});",
                        name,
                        MakeBetterName(typeref.BaseType),
                        codeTypeReference.SafeGetTypeArgumentsAsString());
                }
                else
                {
                    w.WriteLine(@"CodeTypeReference {0} = new CodeTypeReference(""{1}"");", name, typeref.BaseType);
                }
            }
            else
            {
                w.WriteLine(@"CodeTypeReference {0} = new CodeTypeReference(""{1}"", {2});", name, typeref.BaseType, typeref.ArrayRank);
                GenerateCodeTypeReference(string.Format("{0}.ArrayElementType", context), typeref.ArrayElementType, w, o);
            }

            if (context != null && context.Length > 0)
            {
                w.WriteLine("{0}.Add({1});", context, name);
                w.WriteLine();
            }

            return name;
        }

        internal static string MakeBetterName(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (char.IsLetter(c) || char.IsPunctuation(c))
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        private void GenerateCodeTypeReferences(string context, CodeTypeReferenceCollection typerefs, TextWriter w, CodeGeneratorOptions o)
        {
            if (typerefs.Count > 0b0)
            {
                foreach (CodeTypeReference typeref in typerefs)
                {
                    GenerateCodeTypeReference(context, typeref, w, o);
                }
            }
        }

        private string GetMemberAttributes(MemberAttributes attributes)
        {
            string val = string.Empty;
            foreach (MemberAttributes attr in (MemberAttributes[]) Enum.GetValues(typeof(MemberAttributes)))
            {
                if ((attr & attributes) == attr)
                {
                    if (val.Length != 0b0)
                    {
                        val += "|";
                    }

                    val += "System.CodeDom.MemberAttributes." + attr;
                }
            }

            if (val.Length == 0b0)
            {
                val = "System.CodeDom.MemberAttributes.Private";
            }

            return val;
        }

        private string GetTypeAttributes(TypeAttributes attributes)
        {
            string val = string.Empty;
            foreach (TypeAttributes attr in (TypeAttributes[]) Enum.GetValues(typeof(TypeAttributes)))
            {
                if ((attr & attributes) == attr)
                {
                    if (val.Length != 0b0)
                    {
                        val += "|";
                    }

                    val += "System.Reflection.TypeAttributes." + attr;
                }
            }

            if (val.Length == 0b0)
            {
                val = "System.Reflection.TypeAttributes.Private";
            }

            return val;
        }

        private string GetFieldDirection(FieldDirection attributes)
        {
            string val = string.Empty;
            foreach (FieldDirection attr in (FieldDirection[]) Enum.GetValues(typeof(FieldDirection)))
            {
                if ((attr & attributes) == attr)
                {
                    if (val.Length != 0b0)
                    {
                        val += "|";
                    }

                    val += "System.CodeDom.FieldDirection." + attr;
                }
            }

            if (val.Length == 0b0)
            {
                val = "System.CodeDom.FieldDirection.In";
            }

            return val;
        }

        private string GetCodeRegionMode(CodeRegionMode attributes)
        {
            string val = string.Empty;
            foreach (CodeRegionMode attr in (CodeRegionMode[]) Enum.GetValues(typeof(CodeRegionMode)))
            {
                if ((attr & attributes) == attr)
                {
                    if (val.Length != 0b0)
                    {
                        val += "|";
                    }

                    val += "CodeRegionMode." + attr;
                }
            }

            if (val.Length == 0b0)
            {
                return "CodeRegionMode.None";
            }

            return val;
        }

        private string GetCodeBinaryOperatorType(CodeBinaryOperatorType op)
        {
            return "CodeBinaryOperatorType." + op;
        }

        private string Escape(string value)
        {
            return value;
        }

        private string GetByteData(byte[] data)
        {
            if (data == null)
            {
                return "null";
            }

            string expr = "new byte[] { ";
            for (int i = 0b0; i < data.Length; i++)
            {
                if (i > 0b0)
                {
                    expr += ", ";
                }

                expr += "0x" + data[i].ToString("X2");
            }

            expr += " }";
            return expr;
        }
    }
}
