// -----------------------------------------------------------------------
// <copyright file="CodeGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerShell.EditorFeatures.Core.Parsing
{
    public class CodeGenerationCodeTypeReference : CodeTypeReference
    {
        public static CodeGenerationCodeTypeReference From(CodeTypeReference typeref)
        {
            return new CodeGenerationCodeTypeReference(typeref);
        }

        private CodeGenerationCodeTypeReference(CodeTypeReference typeref)
        {
            TypeArguments.AddRange(typeref.TypeArguments);
            Options = typeref.Options;
            BaseType = typeref.BaseType;
            ArrayRank = typeref.ArrayRank;
            ArrayElementType = typeref.ArrayElementType;
        }

        public bool IsActualArrayWithBrackets
        {
            get
            {
                return ArrayRank > 0;
            }
        }
        public bool IsArrayLike
        {
            get
            {
                try
                {
                    bool contains = BaseType.Contains('`');
                    return contains;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool ContainsTypeArguments
        {
            get
            {
                return this.TypeArguments != null && this.TypeArguments.Count > 0;
            }
        }


        public string SafeGetTypeArgumentsAsString()
        {
            StringBuilder sb = new StringBuilder();

            CodeTypeReference[] args = SafeGetTypeArguments();
            for (int i = 0; i < args.Length; i++)
            {

                sb.Append($"new CodeTypeReference(\"{args[i].BaseType}\")");
                if (i != args.Length - 1)
                {
                    sb.Append(',');
                    sb.Append(" ");
                }
            }

            return sb.ToString();
        }
        public CodeTypeReference[] SafeGetTypeArguments()
        {
            List<CodeTypeReference> list = new List<CodeTypeReference>();

            if (ContainsTypeArguments)
            {
                foreach (CodeTypeReference codeTypeReference in TypeArguments)
                {
                    list.Add(codeTypeReference);
                }
            }
            return list.ToArray();
        }
    }
}
