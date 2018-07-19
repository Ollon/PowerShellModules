
// -----------------------------------------------------------------------
// <copyright file="CodeGenerationModel.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using ICSharpCode.AvalonEdit;

namespace PowerShell.EditorFeatures.UI.Windows
{
    public class CodeGenerationModel
    {

        public string InputText { get; set; }


        public string OutputText { get; set; }



        public int IndentSpaces { get; set; }

        public bool BlankLineBetweenMembers { get; set; }

        public string BracingStyle { get; set; }

        public bool ElseOnClosing { get; set; }




        public bool OpenParenthesisOnNewLine { get; set; }

        public bool ClosingParenthesisOnNewLine { get; set; }

        public bool UseDefaultFormatting { get; set; }

        public bool RemoveRedundantModifyingCalls { get; set; }

        public bool ShortenCodeWithUsingStatic { get; set; }
    }


    public class TabItemModel
    {
        public string Header { get; set; }

        public string InputText { get; set; }

        public string OutputText { get; set; }


    }
}
