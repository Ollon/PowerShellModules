// -----------------------------------------------------------------------
// <copyright file="EnumGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.CodeDom;
using System.Reflection;

namespace PowerShell.Infrastructure.Utilities
{
    internal struct EnumContext
    {
        public EnumContext(string @namespace, bool @internal, string name, string verb, string innerEnumName, string[] enumMembers) : this()
        {
            Namespace = @namespace;
            Internal = @internal;
            Name = name;
            Verb = verb;
            InnerEnumName = innerEnumName;
            EnumMembers = enumMembers;
        }
        public string Namespace { get; set; }
        public bool Internal { get; set; }
        public string Name { get; set; }
        public string Verb { get; set; }
        public string InnerEnumName { get; set; }
        public string[] EnumMembers { get; set; }
    }
    internal static class EnumGenerator
    {
        public static CodeCompileUnit GenerateEnumStructure(EnumContext context)
        {
            string parameterName = CamelCase(context.InnerEnumName);
            string identifierName = $"_{parameterName}";
            CodeCompileUnit unit = new CodeCompileUnit();
            CodeNamespace ns = new CodeNamespace(context.Namespace);
            unit.Namespaces.Add(ns);
            ns.Imports.Add(new CodeNamespaceImport("System"));
            ns.Imports.Add(new CodeNamespaceImport("System.Collections"));
            ns.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));

            CodeTypeDeclaration ct = new CodeTypeDeclaration(context.Name);
            ns.Types.Add(ct);
            ct.TypeAttributes = context.Internal ? TypeAttributes.NotPublic : TypeAttributes.Public;

            ct.BaseTypes.Add(new CodeTypeReference($"IEquatable<{context.Name}>"));
            ct.IsStruct = true;
            ct.Comments.Add(new CodeCommentStatement("<summary>", true));
            ct.Comments.Add(new CodeCommentStatement($" Enum Structure <see cref=\"{context.Name}\"/>.", true));
            ct.Comments.Add(new CodeCommentStatement("</summary>", true));

            ct.Members.Add(new CodeMemberField(new CodeTypeReference(context.InnerEnumName), identifierName));

            CodeConstructor ctor1 = new CodeConstructor();
            ctor1.Attributes = (ctor1.Attributes & ~MemberAttributes.AccessMask) | MemberAttributes.Private;
            ctor1.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(context.InnerEnumName), parameterName));
            ctor1.Statements.Add(new CodeAssignStatement(new CodeArgumentReferenceExpression(identifierName), new CodeArgumentReferenceExpression(parameterName)));

            ct.Members.Add(ctor1);

            CodeTypeDeclaration innerEnum = GenerateNestedEnumeration(context);

            double i = 0;
            foreach (string enumMember in context.EnumMembers)
            {
                double y = Math.Pow(i, 2);
                innerEnum.Members.Add(GenerateEnumMemberField(enumMember, (int)y));
                i++;
            }
            ct.Members.Add(innerEnum);

            foreach (string enumMember in context.EnumMembers)
            {
                ct.Members.Add(GenerateVerbBoolProperty(context, identifierName, enumMember));
            }
            foreach (string enumMember in context.EnumMembers)
            {
                ct.Members.Add(GenerateWithVerbMethod(context, identifierName, enumMember));
            }

            ct.Members.Add(GenerateSetFlagMethod(context));

            foreach (string enumMember in context.EnumMembers)
            {
                ct.Members.Add(GenerateStaticEnumMemberProperty(context, enumMember));
            }

            string[] chrs = new[] { "|", "&", "+", "-" };
            foreach (string c in chrs)
            {
                string c2 = c + " ";
                if (c == "+")
                {
                    c2 = "| ";
                }
                if (c == "-")
                {
                    c2 = "&  ~";
                }

                ct.Members.Add(new CodeSnippetTypeMember($"        public static {context.Name} operator {c}({context.Name} left, {context.Name} right) =>" +
                    $" new {context.Name}(left.{identifierName} {c2}right.{identifierName});"));

            }

            ct.Members.Add(new CodeSnippetTypeMember($@"


        public bool Equals({context.Name} {parameterName})
        {{
            return {identifierName} == {parameterName}.{identifierName};
        }}
 
        public override bool Equals(object obj)
        {{
            return obj is {context.Name} && Equals(({context.Name})obj);
        }}
 
        public override int GetHashCode()
        {{
            return (int){identifierName};
        }}
 
        public static bool operator ==({context.Name} left, {context.Name} right)
        {{
            return left.{identifierName} == right.{identifierName};
        }}
 
        public static bool operator !=({context.Name} left, {context.Name} right)
        {{
            return left.{identifierName} != right.{identifierName};
        }}
 
        public override string ToString()
        {{
            return {identifierName}.ToString();
        }}
 
        public static bool TryParse(string value, out {context.Name} {parameterName})
        {{
            if (Enum.TryParse(value, out {context.InnerEnumName} mods))
            {{
                {parameterName} = new {context.Name}(mods);
                return true;
            }}
            else
            {{
                {parameterName} = default({context.Name});
                return false;
            }}
        }}
"));



            return unit;
        }

        private static CodeBinaryOperatorExpression GenerateOperatorExpression()
        {
            CodeBinaryOperatorExpression exp = new CodeBinaryOperatorExpression();

            return exp;
        }

        private static CodeTypeDeclaration GenerateNestedEnumeration(EnumContext context)
        {
            CodeTypeDeclaration innerEnum = new CodeTypeDeclaration(context.InnerEnumName)
            {
                TypeAttributes = TypeAttributes.NestedPrivate
            };
            innerEnum.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference("Flags")));
            innerEnum.IsEnum = true;
            return innerEnum;
        }

        private static CodeMemberProperty GenerateStaticEnumMemberProperty(EnumContext context, string enumMember)
        {
            CodeMemberProperty p = new CodeMemberProperty();
            p.Attributes = (p.Attributes & ~MemberAttributes.AccessMask) | MemberAttributes.Public | MemberAttributes.Static;
            p.Name = enumMember;
            p.Type = new CodeTypeReference(context.Name);
            p.GetStatements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression(new CodeTypeReference(context.Name), new CodeArgumentReferenceExpression($"{context.InnerEnumName}.{enumMember}"))));
            return p;
        }

        private static CodeMemberMethod GenerateWithVerbMethod(EnumContext context, string identifierName, string enumMember)
        {
            string verbPhrase = $"{context.Verb}{enumMember}";
            string camelVerbPhrase = CamelCase(verbPhrase);
            string withVerbPhrase = $"With{verbPhrase}";
            CodeMemberMethod m = new CodeMemberMethod();
            m.Attributes = (m.Attributes & ~MemberAttributes.AccessMask) | MemberAttributes.Public;
            m.Name = withVerbPhrase;
            m.ReturnType = new CodeTypeReference(context.Name);
            m.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(bool)), camelVerbPhrase));
            m.Statements.Add(GenerateWithMethodReturnStatement(context, identifierName, enumMember, camelVerbPhrase));
            return m;
        }

        private static CodeMemberProperty GenerateVerbBoolProperty(EnumContext context, string identifierName, string enumMember)
        {
            CodeMemberProperty p = new CodeMemberProperty();
            p.Attributes = (p.Attributes & ~MemberAttributes.AccessMask) | MemberAttributes.Public;
            p.Name = $"{context.Verb}{enumMember}";
            p.Type = new CodeTypeReference(typeof(bool));
            p.GetStatements.Add(
                GenerateBoolReturnStatement(context, identifierName, enumMember));
            return p;
        }

        private static string CamelCase(string str)
        {
            return str[0].ToString().ToLower() + str.Substring(1);
        }

        private static CodeMemberMethod GenerateSetFlagMethod(EnumContext context)
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "SetFlag";
            method.Attributes = (method.Attributes & ~MemberAttributes.AccessMask) | MemberAttributes.Private | MemberAttributes.Static;
            method.ReturnType = new CodeTypeReference(context.InnerEnumName);
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(context.InnerEnumName), "existing"));
            string parameterName = CamelCase(context.InnerEnumName);
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(context.InnerEnumName), parameterName));
            method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(bool)), "isSet"));
            method.Statements.Add(new CodeMethodReturnStatement(new CodeArgumentReferenceExpression($"isSet ? (existing | {parameterName}) : (existing & ~{parameterName})")));

            return method;
        }

        private static CodeMethodReturnStatement GenerateWithMethodReturnStatement(EnumContext context, string identifierName, string enumMember, string camelVerbPhrase)
        {
            return new CodeMethodReturnStatement(
                                new CodeObjectCreateExpression(new CodeTypeReference(context.Name),
                                new CodeArgumentReferenceExpression($"SetFlag({identifierName}, {context.InnerEnumName}.{enumMember}, {camelVerbPhrase})")));
        }

        private static CodeMethodReturnStatement GenerateBoolReturnStatement(EnumContext context, string identifierName, string enumMember)
        {
            return new CodeMethodReturnStatement(
                                    new CodeArgumentReferenceExpression($"({identifierName} & {context.InnerEnumName}.{enumMember}) != 0"));
        }

        private static CodeMemberField GenerateEnumMemberField(string name, object init)
        {
            CodeMemberField field = new CodeMemberField();
            field.Name = name;
            field.InitExpression = new CodePrimitiveExpression(init);
            return field;
        }
    }
}
