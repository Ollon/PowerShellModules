using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PowerShell.EditorFeatures.Core.Parsing
{
    /// <summary>
    /// Full abstract implementation of <see cref="AbstractCodeGenerator"/>
    /// </summary>
    public abstract class AbstractCSharpCodeGenerator : CodeGeneratorBase
    {
        protected AbstractCSharpCodeGenerator() : base()
        {

        }
        protected AbstractCSharpCodeGenerator(TextWriter writer) : base(writer)
        {

        }

        protected override string NullToken { get; } = "null";

        public void OutputVTableModifier(MemberAttributes attributes)
        {
            switch (attributes & MemberAttributes.VTableMask)
            {
                case MemberAttributes.New:
                    Output.Write("new ");
                    break;
            }
        }

        protected override void OutputFieldScopeModifier(MemberAttributes attributes)
        {
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

        protected override void OutputMemberAccessModifier(MemberAttributes attributes)
        {
            switch (attributes & MemberAttributes.AccessMask)
            {
                case MemberAttributes.Assembly:
                    Output.Write("internal ");
                    break;
                case MemberAttributes.FamilyAndAssembly:
                    Output.Write("internal ");  /*FamANDAssem*/
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

        protected override void OutputMemberScopeModifier(MemberAttributes attributes)
        {
            switch (attributes & MemberAttributes.ScopeMask)
            {
                case MemberAttributes.Abstract:
                    Output.Write("abstract ");
                    break;
                case MemberAttributes.Final:
                    Output.Write("sealed ");
                    break;
                case MemberAttributes.Static:
                    Output.Write("static ");
                    break;
                case MemberAttributes.Override:
                    Output.Write("override ");
                    break;
                    //default:
                    //    switch (attributes & MemberAttributes.AccessMask)
                    //    {
                    //        case MemberAttributes.Family:
                    //        case MemberAttributes.Public:
                    //        case MemberAttributes.Assembly:
                    //            Output.Write("virtual ");
                    //            break;
                    //    }
                    //    break;
            }
        }

        protected override void OutputType(CodeTypeReference typeRef)
        {
            Output.Write(GetTypeOutput(typeRef));
        }

        protected override void OutputOperator(CodeBinaryOperatorType op)
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

        protected override void GenerateArrayCreateExpression(CodeArrayCreateExpression e)
        {
            Output.Write("new ");

            CodeExpressionCollection init = e.Initializers;
            if (init.Count > 0)
            {
                OutputType(e.CreateType);
                if (e.CreateType.ArrayRank == 0)
                {
                    // Unfortunately, many clients are already calling this without array
                    // types. This will allow new clients to correctly use the array type and
                    // not break existing clients. For VNext, stop doing this.
                    Output.Write("[]");
                }
                Output.WriteLine(" {");
                Indent++;
                OutputExpressionList(init, true /*newlineBetweenItems*/);
                Indent--;
                Output.Write("}");
            }
            else
            {
                Output.Write(GetBaseTypeOutput(e.CreateType));

                Output.Write("[");
                if (e.SizeExpression != null)
                {
                    GenerateExpression(e.SizeExpression);
                }
                else
                {
                    Output.Write(e.Size);
                }
                Output.Write("]");

                int nestedArrayDepth = e.CreateType.ArrayRank;
                for (int i = 0; i < nestedArrayDepth - 1; i++)
                {
                    Output.Write("[]");
                }
            }
        }

        protected override void GenerateBaseReferenceExpression(CodeBaseReferenceExpression e)
        {
            Output.Write("base");
        }

        protected override void GenerateBinaryOperatorExpression(CodeBinaryOperatorExpression e)
        {
            bool indentedExpression = false;
            Output.Write("(");

            GenerateExpression(e.Left);
            Output.Write(" ");

            if (e.Left is CodeBinaryOperatorExpression || e.Right is CodeBinaryOperatorExpression)
            {
                // In case the line gets too long with nested binary operators, we need to output them on
                // different lines. However we want to indent them to maintain readability, but this needs
                // to be done only once;
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

        protected override void GenerateCastExpression(CodeCastExpression e)
        {
            Output.Write("((");
            OutputType(e.TargetType);
            Output.Write(")(");
            GenerateExpression(e.Expression);
            Output.Write("))");
        }

        protected override void GenerateDelegateCreateExpression(CodeDelegateCreateExpression e)
        {
            Output.Write("new ");
            OutputType(e.DelegateType);
            Output.Write("(");
            GenerateExpression(e.TargetObject);
            Output.Write(".");
            OutputIdentifier(e.MethodName);
            Output.Write(")");
        }

        protected override void GenerateDecimalValue(decimal d)
        {
            Output.Write(d.ToString(CultureInfo.InvariantCulture));
            Output.Write('m');
        }

        protected override void GenerateDefaultValueExpression(CodeDefaultValueExpression e)
        {
            Output.Write("default(");
            OutputType(e.Type);
            Output.Write(")");
        }

        protected override void GenerateFieldReferenceExpression(CodeFieldReferenceExpression e)
        {
            if (e.TargetObject != null)
            {
                GenerateExpression(e.TargetObject);
                Output.Write(".");
            }
            OutputIdentifier(e.FieldName);
        }

        protected override void GenerateArgumentReferenceExpression(CodeArgumentReferenceExpression e)
        {
            OutputIdentifier(e.ParameterName);
        }

        protected override void GenerateVariableReferenceExpression(CodeVariableReferenceExpression e)
        {
            OutputIdentifier(e.VariableName);
        }

        protected override void GenerateIndexerExpression(CodeIndexerExpression e)
        {
            GenerateExpression(e.TargetObject);
            Output.Write("[");
            bool first = true;
            foreach (CodeExpression exp in e.Indices)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    Output.Write(", ");
                }
                GenerateExpression(exp);
            }
            Output.Write("]");
        }

        protected override void GenerateArrayIndexerExpression(CodeArrayIndexerExpression e)
        {
            GenerateExpression(e.TargetObject);
            Output.Write("[");
            bool first = true;
            foreach (CodeExpression exp in e.Indices)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    Output.Write(", ");
                }
                GenerateExpression(exp);
            }
            Output.Write("]");
        }

        protected override void GenerateSnippetExpression(CodeSnippetExpression e)
        {
            Output.Write(e.Value);
        }

        protected override void GenerateMethodInvokeExpression(CodeMethodInvokeExpression e)
        {
            GenerateMethodReferenceExpression(e.Method);
            Output.Write("(");
            OutputExpressionList(e.Parameters);
            Output.Write(")");
        }

        protected override void GenerateMethodReferenceExpression(CodeMethodReferenceExpression e)
        {
            if (e.TargetObject != null)
            {
                if (e.TargetObject is CodeBinaryOperatorExpression)
                {
                    Output.Write("(");
                    GenerateExpression(e.TargetObject);
                    Output.Write(")");
                }
                else
                {
                    GenerateExpression(e.TargetObject);
                }
                Output.Write(".");
            }
            OutputIdentifier(e.MethodName);

            if (e.TypeArguments.Count > 0)
            {
                Output.Write(GetTypeArgumentsOutput(e.TypeArguments));
            }
        }

        public string GetTypeArgumentsOutput(CodeTypeReferenceCollection typeArguments)
        {
            StringBuilder sb = new StringBuilder(128);
            GetTypeArgumentsOutput(typeArguments, 0, typeArguments.Count, sb);
            return sb.ToString();
        }

        protected override void GenerateEventReferenceExpression(CodeEventReferenceExpression e)
        {
            if (e.TargetObject != null)
            {
                GenerateExpression(e.TargetObject);
                Output.Write(".");
            }
            OutputIdentifier(e.EventName);
        }

        protected override void GenerateDelegateInvokeExpression(CodeDelegateInvokeExpression e)
        {
            if (e.TargetObject != null)
            {
                GenerateExpression(e.TargetObject);
            }
            Output.Write("(");
            OutputExpressionList(e.Parameters);
            Output.Write(")");
        }

        protected override void GenerateObjectCreateExpression(CodeObjectCreateExpression e)
        {
            Output.Write("new ");
            OutputType(e.CreateType);
            Output.Write("(");
            OutputExpressionList(e.Parameters);
            Output.Write(")");
        }

        public void GeneratePrimitiveChar(char c)
        {
            Output.Write('\'');
            switch (c)
            {
                case '\r':
                    Output.Write("\\r");
                    break;
                case '\t':
                    Output.Write("\\t");
                    break;
                case '\"':
                    Output.Write("\\\"");
                    break;
                case '\'':
                    Output.Write("\\\'");
                    break;
                case '\\':
                    Output.Write("\\\\");
                    break;
                case '\0':
                    Output.Write("\\0");
                    break;
                case '\n':
                    Output.Write("\\n");
                    break;
                case '\u2028':
                case '\u2029':
                case '\u0084':
                case '\u0085':
                    AppendEscapedChar(null, c);
                    break;

                default:
                    if (char.IsSurrogate(c))
                    {
                        AppendEscapedChar(null, c);
                    }
                    else
                    {
                        Output.Write(c);
                    }
                    break;
            }
            Output.Write('\'');
        }

        public void AppendEscapedChar(StringBuilder b, char value)
        {
            if (b == null)
            {
                Output.Write("\\u");
                Output.Write(((int)value).ToString("X4", CultureInfo.InvariantCulture));
            }
            else
            {
                b.Append("\\u");
                b.Append(((int)value).ToString("X4", CultureInfo.InvariantCulture));
            }
        }

        protected override void GeneratePrimitiveExpression(CodePrimitiveExpression e)
        {
            if (e.Value is char)
            {
                GeneratePrimitiveChar((char)e.Value);
            }
            else if (e.Value is sbyte)
            {
                // C# has no literal marker for types smaller than Int32                
                Output.Write(((sbyte)e.Value).ToString(CultureInfo.InvariantCulture));
            }
            else if (e.Value is ushort)
            {
                // C# has no literal marker for types smaller than Int32, and you will
                // get a conversion error if you use "u" here.
                Output.Write(((ushort)e.Value).ToString(CultureInfo.InvariantCulture));
            }
            else if (e.Value is uint)
            {
                Output.Write(((uint)e.Value).ToString(CultureInfo.InvariantCulture));
                Output.Write("u");
            }
            else if (e.Value is ulong)
            {
                Output.Write(((ulong)e.Value).ToString(CultureInfo.InvariantCulture));
                Output.Write("ul");
            }
            else
            {
                GeneratePrimitiveExpressionBase(e);
            }
        }

        public void GeneratePrimitiveExpressionBase(CodePrimitiveExpression e)
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
                Output.Write("'" + e.Value.ToString() + "'");
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

        protected override void GenerateSingleFloatValue(float s)
        {
            if (float.IsNaN(s))
            {
                Output.Write("float.NaN");
            }
            else if (float.IsNegativeInfinity(s))
            {
                Output.Write("float.NegativeInfinity");
            }
            else if (float.IsPositiveInfinity(s))
            {
                Output.Write("float.PositiveInfinity");
            }
            else
            {
                Output.Write(s.ToString(CultureInfo.InvariantCulture));
                Output.Write('F');
            }
        }

        protected override void GenerateDoubleValue(double d)
        {
            if (double.IsNaN(d))
            {
                Output.Write("double.NaN");
            }
            else if (double.IsNegativeInfinity(d))
            {
                Output.Write("double.NegativeInfinity");
            }
            else if (double.IsPositiveInfinity(d))
            {
                Output.Write("double.PositiveInfinity");
            }
            else
            {
                Output.Write(d.ToString("R", CultureInfo.InvariantCulture));
                // always mark a double as being a double in case we have no decimal portion (e.g write 1D instead of 1 which is an int)
                Output.Write("D");
            }
        }

        protected override void GeneratePropertyReferenceExpression(CodePropertyReferenceExpression e)
        {
            if (e.TargetObject != null)
            {
                GenerateExpression(e.TargetObject);
                Output.Write(".");
            }
            OutputIdentifier(e.PropertyName);
        }

        protected override void GeneratePropertySetValueReferenceExpression(CodePropertySetValueReferenceExpression e)
        {
            Output.Write("value");
        }

        protected override void GenerateThisReferenceExpression(CodeThisReferenceExpression e)
        {
            Output.Write("this");
        }

        protected override void GenerateTypeReferenceExpression(CodeTypeReferenceExpression e)
        {
            OutputType(e.Type);
        }

        protected override void GenerateTypeOfExpression(CodeTypeOfExpression e)
        {
            Output.Write("typeof(");
            OutputType(e.Type);
            Output.Write(")");
        }

        protected override void GenerateExpressionStatement(CodeExpressionStatement e)
        {
            GenerateExpression(e.Expression);
            if (!generatingForLoop)
            {
                Output.WriteLine(";");
            }
        }

        protected override void GenerateIterationStatement(CodeIterationStatement e)
        {
            generatingForLoop = true;
            Output.Write("for (");
            GenerateStatement(e.InitStatement);
            Output.Write("; ");
            GenerateExpression(e.TestExpression);
            Output.Write("; ");
            GenerateStatement(e.IncrementStatement);
            Output.Write(")");
            OutputStartingBrace();
            generatingForLoop = false;
            Indent++;
            GenerateStatements(e.Statements);
            Indent--;
            Output.WriteLine("}");
        }

        protected override void GenerateThrowExceptionStatement(CodeThrowExceptionStatement e)
        {
            Output.Write("throw");
            if (e.ToThrow != null)
            {
                Output.Write(" ");
                GenerateExpression(e.ToThrow);
            }
            Output.WriteLine(";");
        }

        protected override void GenerateCommentStatement(CodeCommentStatement e)
        {
            if (e.Comment == null)
                throw new ArgumentNullException();
            GenerateComment(e.Comment);
        }

        protected override void GenerateCommentStatements(CodeCommentStatementCollection e)
        {
            foreach (CodeCommentStatement comment in e)
            {
                GenerateCommentStatement(comment);
            }
        }

        protected override void GenerateComment(CodeComment e)
        {
            string commentLineStart = e.DocComment ? "///" : "//";
            Output.Write(commentLineStart);
            Output.Write(" ");

            string value = e.Text;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '\u0000')
                {
                    continue;
                }
                Output.Write(value[i]);

                if (value[i] == '\r')
                {
                    if (i < value.Length - 1 && value[i + 1] == '\n')
                    { // if next char is '\n', skip it
                        Output.Write('\n');
                        i++;
                    }
                    for (int index = 0; index < Indent; ++index)
                    {
                        Output.Write("    ");
                    }
                    Output.Write(commentLineStart);
                }
                else if (value[i] == '\n')
                {
                    for (int index = 0; index < Indent; ++index)
                    {
                        Output.Write("    ");
                    }
                    Output.Write(commentLineStart);
                }
                else if (value[i] == '\u2028' || value[i] == '\u2029' || value[i] == '\u0085')
                {
                    Output.Write(commentLineStart);
                }
            }
            Output.WriteLine();
        }

        protected override void GenerateMethodReturnStatement(CodeMethodReturnStatement e)
        {
            Output.Write("return");
            if (e.Expression != null)
            {
                Output.Write(" ");
                GenerateExpression(e.Expression);
            }
            Output.WriteLine(";");
        }

        protected override void GenerateConditionStatement(CodeConditionStatement e)
        {
            Output.Write("if (");
            GenerateExpression(e.Condition);
            Output.Write(")");
            OutputStartingBrace();
            Indent++;
            GenerateStatements(e.TrueStatements);
            Indent--;

            CodeStatementCollection falseStatemetns = e.FalseStatements;
            if (falseStatemetns.Count > 0)
            {
                Output.Write("}");
                if (Options.ElseOnClosing)
                {
                    Output.Write(" ");
                }
                else
                {
                    Output.WriteLine("");
                }
                Output.Write("else");
                OutputStartingBrace();
                Indent++;
                GenerateStatements(e.FalseStatements);
                Indent--;
            }
            Output.WriteLine("}");
        }

        protected override void GenerateTryCatchFinallyStatement(CodeTryCatchFinallyStatement e)
        {
            Output.Write("try");
            OutputStartingBrace();
            Indent++;
            GenerateStatements(e.TryStatements);
            Indent--;
            CodeCatchClauseCollection catches = e.CatchClauses;
            if (catches.Count > 0)
            {
                IEnumerator en = catches.GetEnumerator();
                while (en.MoveNext())
                {
                    Output.Write("}");
                    if (Options.ElseOnClosing)
                    {
                        Output.Write(" ");
                    }
                    else
                    {
                        Output.WriteLine("");
                    }
                    CodeCatchClause current = (CodeCatchClause)en.Current;
                    Output.Write("catch (");
                    OutputType(current.CatchExceptionType);
                    Output.Write(" ");
                    OutputIdentifier(current.LocalName);
                    Output.Write(")");
                    OutputStartingBrace();
                    Indent++;
                    GenerateStatements(current.Statements);
                    Indent--;
                }
            }

            CodeStatementCollection finallyStatements = e.FinallyStatements;
            if (finallyStatements.Count > 0)
            {
                Output.Write("}");
                if (Options.ElseOnClosing)
                {
                    Output.Write(" ");
                }
                else
                {
                    Output.WriteLine("");
                }
                Output.Write("finally");
                OutputStartingBrace();
                Indent++;
                GenerateStatements(finallyStatements);
                Indent--;
            }
            Output.WriteLine("}");
        }

        private bool generatingForLoop = false;
        


        protected override void GenerateAssignStatement(CodeAssignStatement e)
        {
            GenerateExpression(e.Left);
            Output.Write(" = ");
            GenerateExpression(e.Right);
            if (!generatingForLoop)
            {
                Output.WriteLine(";");
            }
        }

        protected override void GenerateAttachEventStatement(CodeAttachEventStatement e)
        {
            GenerateEventReferenceExpression(e.Event);
            Output.Write(" += ");
            GenerateExpression(e.Listener);
            Output.WriteLine(";");
        }

        protected override void GenerateRemoveEventStatement(CodeRemoveEventStatement e)
        {
            GenerateEventReferenceExpression(e.Event);
            Output.Write(" -= ");
            GenerateExpression(e.Listener);
            Output.WriteLine(";");
        }

        protected override void GenerateGotoStatement(CodeGotoStatement e)
        {
            Output.Write("goto ");
            Output.Write(e.Label);
            Output.WriteLine(";");
        }

        protected override void GenerateLabeledStatement(CodeLabeledStatement e)
        {
            Indent--;
            Output.Write(e.Label);
            Output.WriteLine(":");
            Indent++;
            if (e.Statement != null)
            {
                GenerateStatement(e.Statement);
            }
        }

        protected override void GenerateSnippetStatement(CodeSnippetStatement e)
        {
            Output.WriteLine(e.Value);
        }

        protected override void GenerateVariableDeclarationStatement(CodeVariableDeclarationStatement e)
        {
            OutputTypeNamePair(e.Type, e.Name);
            if (e.InitExpression != null)
            {
                Output.Write(" = ");
                GenerateExpression(e.InitExpression);
            }
            if (!generatingForLoop)
            {
                Output.WriteLine(";");
            }
        }

        protected override void GenerateLinePragmaStart(CodeLinePragma e)
        {
            Output.WriteLine("");
            Output.Write("#line ");
            Output.Write(e.LineNumber);
            Output.Write(" \"");
            Output.Write(e.FileName);
            Output.Write("\"");
            Output.WriteLine("");
        }

        protected override void GenerateLinePragmaEnd(CodeLinePragma e)
        {
            Output.WriteLine();
            Output.WriteLine("#line default");
            Output.WriteLine("#line hidden");
        }

        protected override void GenerateEvent(CodeMemberEvent e, CodeTypeDeclaration c)
        {
            if (IsCurrentDelegate || IsCurrentEnum) return;

            if (e.CustomAttributes.Count > 0)
            {
                GenerateAttributes(e.CustomAttributes);
            }

            if (e.PrivateImplementationType == null)
            {
                OutputMemberAccessModifier(e.Attributes);
            }
            Output.Write("event ");
            string name = e.Name;
            if (e.PrivateImplementationType != null)
            {
                name = GetBaseTypeOutput(e.PrivateImplementationType) + "." + name;
            }
            OutputTypeNamePair(e.Type, name);
            Output.WriteLine(";");
        }

        protected override void GenerateField(CodeMemberField e)
        {
            if (IsCurrentDelegate || IsCurrentInterface) return;

            if (IsCurrentEnum)
            {
                if (e.CustomAttributes.Count > 0)
                {
                    GenerateAttributes(e.CustomAttributes);
                }
                OutputIdentifier(e.Name);
                if (e.InitExpression != null)
                {
                    Output.Write(" = ");
                    GenerateExpression(e.InitExpression);
                }
                Output.WriteLine(",");
            }
            else
            {
                if (e.CustomAttributes.Count > 0)
                {
                    GenerateAttributes(e.CustomAttributes);
                }

                OutputMemberAccessModifier(e.Attributes);

                if (e.UserData.Contains("IsReadOnly") && (bool)e.UserData["IsReadOnly"])
                {
                    Output.Write("readonly ");
                }
                OutputVTableModifier(e.Attributes);
                OutputFieldScopeModifier(e.Attributes);
                OutputTypeNamePair(e.Type, e.Name);
                if (e.InitExpression != null)
                {
                    Output.Write(" = ");
                    GenerateExpression(e.InitExpression);
                }
                Output.WriteLine(";");
            }
        }

        protected override void GenerateSnippetMember(CodeSnippetTypeMember e)
        {
            Output.Write(e.Text);
        }

        protected override void GenerateEntryPointMethod(CodeEntryPointMethod e, CodeTypeDeclaration c)
        {
            if (e.CustomAttributes.Count > 0)
            {
                GenerateAttributes(e.CustomAttributes);
            }
            Output.Write("public static ");
            OutputType(e.ReturnType);
            Output.Write(" Main(params string[] args)");
            OutputStartingBrace();
            Indent++;

            GenerateStatements(e.Statements);

            Indent--;
            Output.WriteLine("}");
        }

        protected override void GenerateMethod(CodeMemberMethod e, CodeTypeDeclaration c)
        {
            if (!(IsCurrentClass || IsCurrentStruct || IsCurrentInterface)) return;

            if (e.CustomAttributes.Count > 0)
            {
                GenerateAttributes(e.CustomAttributes);
            }
            if (e.ReturnTypeCustomAttributes.Count > 0)
            {
                GenerateAttributes(e.ReturnTypeCustomAttributes, "return: ");
            }

            if (!IsCurrentInterface)
            {
                if (e.PrivateImplementationType == null)
                {
                    OutputMemberAccessModifier(e.Attributes);
                    OutputVTableModifier(e.Attributes);
                    OutputMemberScopeModifier(e.Attributes);
                }
            }
            else
            {
                // interfaces still need "new"
                OutputVTableModifier(e.Attributes);
            }
            OutputType(e.ReturnType);
            Output.Write(" ");
            if (e.PrivateImplementationType != null)
            {
                Output.Write(GetBaseTypeOutput(e.PrivateImplementationType));
                Output.Write(".");
            }
            OutputIdentifier(e.Name);

            OutputTypeParameters(e.TypeParameters);

            Output.Write("(");
            OutputParameters(e.Parameters);
            Output.Write(")");

            OutputTypeParameterConstraints(e.TypeParameters);

            if (!IsCurrentInterface
                && (e.Attributes & MemberAttributes.ScopeMask) != MemberAttributes.Abstract)
            {

                OutputStartingBrace();
                Indent++;

                GenerateStatements(e.Statements);

                Indent--;
                Output.WriteLine("}");
            }
            else
            {
                Output.WriteLine(";");
            }
        }

        protected override void GenerateProperty(CodeMemberProperty e, CodeTypeDeclaration c)
        {
            if (!(IsCurrentClass || IsCurrentStruct || IsCurrentInterface)) return;

            if (e.CustomAttributes.Count > 0)
            {
                GenerateAttributes(e.CustomAttributes);
            }

            if (!IsCurrentInterface)
            {
                if (e.PrivateImplementationType == null)
                {
                    OutputMemberAccessModifier(e.Attributes);
                    OutputVTableModifier(e.Attributes);
                    OutputMemberScopeModifier(e.Attributes);
                }
            }
            else
            {
                OutputVTableModifier(e.Attributes);
            }
            OutputType(e.Type);
            Output.Write(" ");

            if (e.PrivateImplementationType != null && !IsCurrentInterface)
            {
                Output.Write(GetBaseTypeOutput(e.PrivateImplementationType));
                Output.Write(".");
            }

            if (e.Parameters.Count > 0 && string.Compare(e.Name, "Item", StringComparison.OrdinalIgnoreCase) == 0)
            {
                Output.Write("this[");
                OutputParameters(e.Parameters);
                Output.Write("]");
            }
            else
            {
                OutputIdentifier(e.Name);
            }

            OutputStartingBrace();
            Indent++;

            if (e.HasGet)
            {
                if (IsCurrentInterface || (e.Attributes & MemberAttributes.ScopeMask) == MemberAttributes.Abstract)
                {
                    Output.WriteLine("get;");
                }
                else
                {
                    Output.Write("get");
                    OutputStartingBrace();
                    Indent++;
                    GenerateStatements(e.GetStatements);
                    Indent--;
                    Output.WriteLine("}");
                }
            }
            if (e.HasSet)
            {
                if (IsCurrentInterface || (e.Attributes & MemberAttributes.ScopeMask) == MemberAttributes.Abstract)
                {
                    Output.WriteLine("set;");
                }
                else
                {
                    Output.Write("set");
                    OutputStartingBrace();
                    Indent++;
                    GenerateStatements(e.SetStatements);
                    Indent--;
                    Output.WriteLine("}");
                }
            }

            Indent--;
            Output.WriteLine("}");
        }

        protected override void GenerateConstructor(CodeConstructor e, CodeTypeDeclaration c)
        {
            if (!(IsCurrentClass || IsCurrentStruct)) return;

            if (e.CustomAttributes.Count > 0)
            {
                GenerateAttributes(e.CustomAttributes);
            }

            OutputMemberAccessModifier(e.Attributes);
            OutputIdentifier(CurrentTypeName);
            Output.Write("(");
            OutputParameters(e.Parameters);
            Output.Write(")");

            CodeExpressionCollection baseArgs = e.BaseConstructorArgs;
            CodeExpressionCollection thisArgs = e.ChainedConstructorArgs;

            if (baseArgs.Count > 0)
            {
                Output.WriteLine(" : ");
                Indent++;
                Indent++;
                Output.Write("base(");
                OutputExpressionList(baseArgs);
                Output.Write(")");
                Indent--;
                Indent--;
            }

            if (thisArgs.Count > 0)
            {
                Output.WriteLine(" : ");
                Indent++;
                Indent++;
                Output.Write("this(");
                OutputExpressionList(thisArgs);
                Output.Write(")");
                Indent--;
                Indent--;
            }

            OutputStartingBrace();
            Indent++;
            GenerateStatements(e.Statements);
            Indent--;
            Output.WriteLine("}");
        }

        protected override void GenerateTypeConstructor(CodeTypeConstructor e)
        {
            if (!(IsCurrentClass || IsCurrentStruct)) return;

            if (e.CustomAttributes.Count > 0)
            {
                GenerateAttributes(e.CustomAttributes);
            }
            Output.Write("static ");
            Output.Write(CurrentTypeName);
            Output.Write("()");
            OutputStartingBrace();
            Indent++;
            GenerateStatements(e.Statements);
            Indent--;
            Output.WriteLine("}");
        }

        public void GenerateAttributes(CodeAttributeDeclarationCollection attributes)
        {
            GenerateAttributes(attributes, null, false);
        }

        public void GenerateAttributes(CodeAttributeDeclarationCollection attributes, string prefix)
        {
            GenerateAttributes(attributes, prefix, false);
        }

        public void GenerateAttributes(CodeAttributeDeclarationCollection attributes, string prefix, bool inLine)
        {
            if (attributes.Count == 0) return;
            IEnumerator en = attributes.GetEnumerator();
            bool paramArray = false;

            while (en.MoveNext())
            {
                // we need to convert paramArrayAttribute to params keyword to 
                // make csharp compiler happy. In addition, params keyword needs to be after 
                // other attributes.

                CodeAttributeDeclaration current = (CodeAttributeDeclaration)en.Current;

                if (current.Name.Equals("system.paramarrayattribute", StringComparison.OrdinalIgnoreCase))
                {
                    paramArray = true;
                    continue;
                }

                GenerateAttributeDeclarationsStart(attributes);
                if (prefix != null)
                {
                    Output.Write(prefix);
                }

                if (current.AttributeType != null)
                {
                    Output.Write(GetTypeOutput(current.AttributeType));
                }
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
                GenerateAttributeDeclarationsEnd(attributes);
                if (inLine)
                {
                    Output.Write(" ");
                }
                else
                {
                    Output.WriteLine();
                }
            }

            if (paramArray)
            {
                if (prefix != null)
                {
                    Output.Write(prefix);
                }
                Output.Write("params");

                if (inLine)
                {
                    Output.Write(" ");
                }
                else
                {
                    Output.WriteLine();
                }
            }


        }

        protected override void GenerateTypeStart(CodeTypeDeclaration e)
        {
            if (e.CustomAttributes.Count > 0)
            {
                GenerateAttributes(e.CustomAttributes);
            }

            if (IsCurrentDelegate)
            {
                switch (e.TypeAttributes & TypeAttributes.VisibilityMask)
                {
                    case TypeAttributes.Public:
                        Output.Write("public ");
                        break;
                    case TypeAttributes.NotPublic:
                    default:
                        break;
                }

                CodeTypeDelegate del = (CodeTypeDelegate)e;
                Output.Write("delegate ");
                OutputType(del.ReturnType);
                Output.Write(" ");
                OutputIdentifier(e.Name);
                Output.Write("(");
                OutputParameters(del.Parameters);
                Output.WriteLine(");");
            }
            else
            {
                OutputTypeAttributes(e);
                OutputIdentifier(e.Name);

                OutputTypeParameters(e.TypeParameters);

                bool first = true;
                foreach (CodeTypeReference typeRef in e.BaseTypes)
                {
                    if (first)
                    {
                        Output.Write(" : ");
                        first = false;
                    }
                    else
                    {
                        Output.Write(", ");
                    }
                    OutputType(typeRef);
                }

                OutputTypeParameterConstraints(e.TypeParameters);

                OutputStartingBrace();
                Indent++;
            }
        }

        public void OutputTypeAttributes(CodeTypeDeclaration e)
        {
            if ((e.Attributes & MemberAttributes.New) != 0)
            {
                Output.Write("new ");
            }

            TypeAttributes attributes = e.TypeAttributes;
            switch (attributes & TypeAttributes.VisibilityMask)
            {
                case TypeAttributes.Public:
                case TypeAttributes.NestedPublic:
                    Output.Write("public ");
                    break;
                case TypeAttributes.NestedPrivate:
                    Output.Write("private ");
                    break;
                case TypeAttributes.NestedFamily:
                    Output.Write("protected ");
                    break;
                case TypeAttributes.NotPublic:
                case TypeAttributes.NestedAssembly:
                case TypeAttributes.NestedFamANDAssem:
                    Output.Write("internal ");
                    break;
                case TypeAttributes.NestedFamORAssem:
                    Output.Write("protected internal ");
                    break;
            }

            if (e.IsStruct)
            {
                if (e.IsPartial)
                {
                    Output.Write("partial ");
                }
                Output.Write("struct ");
            }
            else if (e.IsEnum)
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
                        if (e.IsPartial)
                        {
                            Output.Write("partial ");
                        }

                        Output.Write("class ");

                        break;
                    case TypeAttributes.Interface:
                        if (e.IsPartial)
                        {
                            Output.Write("partial ");
                        }
                        Output.Write("interface ");
                        break;
                }
            }
        }

        protected override void GenerateTypeEnd(CodeTypeDeclaration e)
        {
            if (!IsCurrentDelegate)
            {
                Indent--;
                Output.WriteLine("}");
            }
        }

        protected override void GenerateCompileUnit(CodeCompileUnit e)
        {
            GenerateCompileUnitStart(e);
            GenerateNamespaces(e);
            GenerateCompileUnitEnd(e);
        }

        protected override void GenerateCompileUnitStart(CodeCompileUnit e)
        {
            Output.WriteLine("//------------------------------------------------------------------------------");
            Output.Write("// <auto-generated>");
            Output.WriteLine("//     This code was generated by a tool.");
            Output.Write("//     Runtime Version: ");
            Output.WriteLine(Environment.Version.ToString());
            Output.WriteLine("//");
            Output.WriteLine("//     Changes to this file may cause incorrect behavior and will be lost if");
            Output.WriteLine("//     the code is reWrited.");
            Output.WriteLine("// </auto-generated>");
            Output.WriteLine("//------------------------------------------------------------------------------");
            Output.WriteLine("");

            SortedList importList;
            // CSharp needs to put assembly attributes after using statements.
            // Since we need to create a empty namespace even if we don't need it,
            // using will generated after assembly attributes.
            importList = new SortedList(StringComparer.Ordinal);
            foreach (CodeNamespace nspace in e.Namespaces)
            {
                if (String.IsNullOrEmpty(nspace.Name))
                {
                    // mark the namespace to stop it generating its own import list
                    nspace.UserData["WriteImports"] = false;

                    // Collect the unique list of imports
                    foreach (CodeNamespaceImport import in nspace.Imports)
                    {
                        if (!importList.Contains(import.Namespace))
                        {
                            importList.Add(import.Namespace, import.Namespace);
                        }
                    }
                }
            }

            // now output the imports
            foreach (string import in importList.Keys)
            {
                Output.Write("using ");
                OutputIdentifier(import);
                Output.WriteLine(";");
            }
            if (importList.Keys.Count > 0)
            {
                Output.WriteLine("");
            }

            // in C# the best place to put these is at the top level.
            if (e.AssemblyCustomAttributes.Count > 0)
            {
                GenerateAttributes(e.AssemblyCustomAttributes, "assembly: ");
                Output.WriteLine("");
            }

            foreach (CodeNamespace nspace in e.Namespaces)
            {
                nspace.Imports.Clear();
            }
        }

        protected override void GenerateCompileUnitEnd(CodeCompileUnit e)
        {
            if (e.EndDirectives.Count > 0)
            {
                GenerateDirectives(e.EndDirectives);
            }
        }

        protected override void GenerateNamespaceStart(CodeNamespace e)
        {
            if (!usingsAlreadyWritten)
            {
                GenerateNamespaceImports(e);
            }

            if (!string.IsNullOrEmpty(e.Name))
            {
                Output.Write("namespace ");
                string[] names = e.Name.Split('.');
                Debug.Assert(names.Length > 0);
                OutputIdentifier(names[0]);
                for (int i = 1; i < names.Length; i++)
                {
                    Output.Write(".");
                    OutputIdentifier(names[i]);
                }
                OutputStartingBrace();
                Indent++;
            }
        }

        protected override void GenerateNamespaceEnd(CodeNamespace e)
        {
            if (!string.IsNullOrEmpty(e.Name))
            {
                Indent--;
                Output.WriteLine("}");
            }
        }

        protected override void GenerateNamespaceImport(CodeNamespaceImport e)
        {
            Output.Write("using ");
            OutputIdentifier(e.Namespace);
            Output.WriteLine(";");
        }

        protected override void GenerateAttributeDeclarationsStart(CodeAttributeDeclarationCollection attributes)
        {
            Output.Write("[");
        }

        protected override void GenerateAttributeDeclarationsEnd(CodeAttributeDeclarationCollection attributes)
        {
            Output.Write("]");
        }

        public void GetTypeArgumentsOutput(CodeTypeReferenceCollection typeArguments, int start, int length, StringBuilder sb)
        {
            sb.Append('<');
            bool first = true;
            for (int i = start; i < start + length; i++)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(", ");
                }

                // it's possible that we call GetTypeArgumentsOutput with an empty typeArguments collection.  This is the case
                // for open types, so we want to just output the brackets and commas. 
                if (i < typeArguments.Count)
                    sb.Append(GetTypeOutput(typeArguments[i]));
            }
            sb.Append('>');
        }

        // returns the type name without any array declaration.
        public string GetBaseTypeOutput(CodeTypeReference typeRef)
        {
            string s = typeRef.BaseType;
            if (s.Length == 0)
            {
                s = "void";
                return s;
            }

            string lowerCaseString = s.ToLower(CultureInfo.InvariantCulture).Trim();

            switch (lowerCaseString)
            {
                case "system.int16":
                    s = "short";
                    break;
                case "system.int32":
                    s = "int";
                    break;
                case "system.int64":
                    s = "long";
                    break;
                case "system.string":
                    s = "string";
                    break;
                case "system.object":
                    s = "object";
                    break;
                case "system.boolean":
                    s = "bool";
                    break;
                case "system.void":
                    s = "void";
                    break;
                case "system.char":
                    s = "char";
                    break;
                case "system.byte":
                    s = "byte";
                    break;
                case "system.uint16":
                    s = "ushort";
                    break;
                case "system.uint32":
                    s = "uint";
                    break;
                case "system.uint64":
                    s = "ulong";
                    break;
                case "system.sbyte":
                    s = "sbyte";
                    break;
                case "system.single":
                    s = "float";
                    break;
                case "system.double":
                    s = "double";
                    break;
                case "system.decimal":
                    s = "decimal";
                    break;
                default:
                    // replace + with . for nested classes.
                    //
                    StringBuilder sb = new StringBuilder(s.Length + 10);
                    if ((typeRef.Options & CodeTypeReferenceOptions.GlobalReference) != 0)
                    {
                        sb.Append("global::");
                    }

                    string baseType = typeRef.BaseType;

                    int lastIndex = 0;
                    int currentTypeArgStart = 0;
                    for (int i = 0; i < baseType.Length; i++)
                    {
                        switch (baseType[i])
                        {
                            case '+':
                            case '.':
                                sb.Append(CreateEscapedIdentifier(baseType.Substring(lastIndex, i - lastIndex)));
                                sb.Append('.');
                                i++;
                                lastIndex = i;
                                break;

                            case '`':
                                sb.Append(CreateEscapedIdentifier(baseType.Substring(lastIndex, i - lastIndex)));
                                i++;    // skip the '
                                int numTypeArgs = 0;
                                while (i < baseType.Length && baseType[i] >= '0' && baseType[i] <= '9')
                                {
                                    numTypeArgs = numTypeArgs * 10 + (baseType[i] - '0');
                                    i++;
                                }

                                GetTypeArgumentsOutput(typeRef.TypeArguments, currentTypeArgStart, numTypeArgs, sb);
                                currentTypeArgStart += numTypeArgs;

                                // Arity can be in the middle of a nested type name, so we might have a . or + after it. 
                                // Skip it if so. 
                                if (i < baseType.Length && (baseType[i] == '+' || baseType[i] == '.'))
                                {
                                    sb.Append('.');
                                    i++;
                                }

                                lastIndex = i;
                                break;
                        }
                    }

                    if (lastIndex < baseType.Length)
                        sb.Append(CreateEscapedIdentifier(baseType.Substring(lastIndex)));

                    return sb.ToString();
            }
            return s;
        }

        public void OutputTypeParameters(CodeTypeParameterCollection typeParameters)
        {
            if (typeParameters.Count == 0)
            {
                return;
            }

            Output.Write('<');
            bool first = true;
            for (int i = 0; i < typeParameters.Count; i++)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    Output.Write(", ");
                }

                if (typeParameters[i].CustomAttributes.Count > 0)
                {
                    GenerateAttributes(typeParameters[i].CustomAttributes, null, true);
                    Output.Write(' ');
                }

                Output.Write(typeParameters[i].Name);
            }

            Output.Write('>');
        }

        public void OutputTypeParameterConstraints(CodeTypeParameterCollection typeParameters)
        {
            if (typeParameters.Count == 0)
            {
                return;
            }

            for (int i = 0; i < typeParameters.Count; i++)
            {
                // generating something like: "where KeyType: IComparable, IEnumerable"

                Output.WriteLine();
                Indent++;

                bool first = true;
                if (typeParameters[i].Constraints.Count > 0)
                {
                    foreach (CodeTypeReference typeRef in typeParameters[i].Constraints)
                    {
                        if (first)
                        {
                            Output.Write("where ");
                            Output.Write(typeParameters[i].Name);
                            Output.Write(" : ");
                            first = false;
                        }
                        else
                        {
                            Output.Write(", ");
                        }
                        OutputType(typeRef);
                    }
                }

                if (typeParameters[i].HasConstructorConstraint)
                {
                    if (first)
                    {
                        Output.Write("where ");
                        Output.Write(typeParameters[i].Name);
                        Output.Write(" : new()");
                    }
                    else
                    {
                        Output.Write(", new ()");
                    }
                }

                Indent--;
            }
        }

        protected override string GetTypeOutput(CodeTypeReference value)
        {
            string s = string.Empty;

            CodeTypeReference baseTypeRef = value;
            while (baseTypeRef.ArrayElementType != null)
            {
                baseTypeRef = baseTypeRef.ArrayElementType;
            }
            s += GetBaseTypeOutput(baseTypeRef);

            while (value != null && value.ArrayRank > 0)
            {
                char[] results = new char[value.ArrayRank + 1];
                results[0] = '[';
                results[value.ArrayRank] = ']';
                for (int i = 1; i < value.ArrayRank; i++)
                {
                    results[i] = ',';
                }
                s += new string(results);
                value = value.ArrayElementType;
            }

            return s;
        }

        protected override string QuoteSnippetString(string value)
        {
            return null;
        }

        protected override bool Supports(GeneratorSupport support)
        {
            return true;
        }

        protected override bool IsValidIdentifier(string value)
        {
            return true;
        }

        protected override string CreateEscapedIdentifier(string value)
        {
            if (IsKeyword(value) || IsPrefixTwoUnderscore(value))
            {
                return "@" + value;
            }
            return value;
        }

        protected override string CreateValidIdentifier(string value)
        {
            if (IsPrefixTwoUnderscore(value))
            {
                value = "_" + value;
            }

            while (IsKeyword(value))
            {
                value = "_" + value;
            }

            return value;
        }

        private static bool IsPrefixTwoUnderscore(string value)
        {
            if (value.Length < 3)
            {
                return false;
            }
            else
            {
                return ((value[0] == '_') && (value[1] == '_') && (value[2] != '_'));
            }
        }

        private static bool IsKeyword(string name)
        {
            switch (name)
            {
                case "bool":
                case "byte":
                case "sbyte":
                case "short":
                case "ushort":
                case "int":
                case "uint":
                case "long":
                case "ulong":
                case "double":
                case "float":
                case "decimal":
                case "string":
                case "char":
                case "object":
                case "typeof":
                case "sizeof":
                case "null":
                case "true":
                case "false":
                case "if":
                case "else":
                case "while":
                case "for":
                case "foreach":
                case "do":
                case "switch":
                case "case":
                case "default":
                case "lock":
                case "try":
                case "throw":
                case "catch":
                case "finally":
                case "goto":
                case "break":
                case "continue":
                case "return":
                case "public":
                case "private":
                case "internal":
                case "protected":
                case "static":
                case "readonly":
                case "sealed":
                case "const":
                case "new":
                case "override":
                case "abstract":
                case "virtual":
                case "partial":
                case "ref":
                case "out":
                case "in":
                case "where":
                case "params":
                case "this":
                case "base":
                case "namespace":
                case "using":
                case "class":
                case "struct":
                case "interface":
                case "delegate":
                case "checked":
                case "get":
                case "set":
                case "add":
                case "remove":
                case "operator":
                case "implicit":
                case "explicit":
                case "fixed":
                case "extern":
                case "event":
                case "enum":
                case "unsafe":
                    return true;
                default:
                    return false;
            }
        }

        public void OutputStartingBrace()
        {
            if (Options.BracingStyle == "C")
            {
                Output.WriteLine("");
                Output.WriteLine("{");
            }
            else
            {
                Output.WriteLine(" {");
            }
        }
    }
}
