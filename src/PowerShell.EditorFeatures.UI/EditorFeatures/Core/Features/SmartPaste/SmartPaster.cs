// -----------------------------------------------------------------------
// <copyright file="SmartPaster.cs" company="Ollon, LLC">
//     Copyright (c) 2018 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Windows;
using PowerShell.EditorFeatures.Core.Host;

namespace PowerShell.EditorFeatures.Core.Features.SmartPaste
{
    public static class SmartPaster
    {
        public const string Quote = "\"";
        public const string Tilde = "`";
        public const string DollarSign = "$";


        public static string ClipboardText
        {
            get
            {
                IDataObject iData = Clipboard.GetDataObject();
                if (iData == null)
                {
                    return string.Empty;
                }

                if (iData.GetDataPresent(DataFormats.UnicodeText))
                {
                    return Convert.ToString(iData.GetData(DataFormats.UnicodeText));
                }

                if (iData.GetDataPresent(DataFormats.Text))
                {
                    return Convert.ToString(iData.GetData(DataFormats.Text));
                }

                return string.Empty;
            }
        }

        public static string TextWriterizeInCSharp(string txt)
        {
            StringBuilder sb = new StringBuilder(txt);
            sb.Replace("\u0022", "\u0022\u0022");
            sb.Replace("\u005C", "\u005C\u005C");
            sb.Replace("\u007B", "\u007B\u007B");
            sb.Replace("\u007D", "\u007D\u007D");
            string fullString = sb.ToString();
            sb.Clear();
            sb.AppendLine("System.Text.StringBuilder sb = new System.Text.StringBuilder();");
            sb.AppendLine("System.CodeDom.Compiler.IndentedTextWriter writer = ");
            sb.AppendLine("    System.CodeDom.Compiler.IndentedTextWriter(new System.IO.StringWriter(sb))");
            using (StringReader reader = new StringReader(fullString))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    sb.Append("writer.WriteLine(@");
                    sb.Append(Quote);
                    sb.Append(line);
                    sb.AppendLine(Quote + ");");
                }
            }

            return sb.ToString();
        }

        public static string StringbuilderizeInCSharp(string txt)
        {
            StringBuilder sb = new StringBuilder(txt);
            sb.Replace("\"", "\"\"");
            sb.Replace("\\", "\\\\");
            sb.Replace("{", "{{");
            sb.Replace("}", "}}");
            string fullString = sb.ToString();
            sb.Clear();
            sb.AppendLine("System.Text.StringBuilder sb = new System.Text.StringBuilder();");
            using (StringReader reader = new StringReader(fullString))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    sb.Append("sb.AppendLine(@");
                    sb.Append(Quote);
                    sb.Append(line);
                    sb.AppendLine(Quote + ");");
                }
            }

            return sb.ToString();
        }

        public static void EncloseWithXmlTextWriter(IEditorFeaturesHostObject hostObject)
        {
            string txt;
            if (string.IsNullOrEmpty(txt = hostObject.GetSelectedText()))
            {
                txt = hostObject.CurrentEditor.Text;
            }
            StringBuilder sb = new StringBuilder(txt);
            sb.Replace(Tilde, Tilde + Tilde);
            sb.Replace(Quote, Tilde + Quote);
            sb.Replace(DollarSign, Tilde + DollarSign);
            string fullString = sb.ToString();
            sb.Clear();
            sb.AppendLine("[System.Reflection.Assembly]::LoadWithPartialName('System.Xml')");
            sb.AppendLine();
            sb.AppendLine("$sb = [System.Text.StringBuilder]::new()");
            sb.AppendLine("$sw = [System.IO.StringWriter]::new($sb)");
            sb.AppendLine("$w  = [System.Xml.XmlTextWriter]::new($sw)");
            sb.AppendLine("$w.Formatting = [System.Xml.Formatting]::Indented;");
            sb.AppendLine("");
            using (StringReader reader = new StringReader(fullString))
            {
                sb.AppendLine("<#");
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }

                sb.AppendLine("#>");
            }

            sb.AppendLine("");
            sb.AppendLine("$sb.ToString() | Set-Clipboard");
            sb.AppendLine("");
            sb.AppendLine("$sb.ToString()");
            hostObject.SetCurrentFileText(sb.ToString());
        }

        public static void EncloseWithTextWriter(IEditorFeaturesHostObject hostObject)
        {
            string txt;
            if (string.IsNullOrEmpty(txt = hostObject.GetSelectedText()))
            {
                txt = hostObject.CurrentEditor.Text;
            }

            StringBuilder sb = new StringBuilder(txt);
            sb.Replace(Tilde, Tilde + Tilde);
            sb.Replace(Quote, Tilde + Quote);
            sb.Replace(DollarSign, Tilde + DollarSign);
            string fullString = sb.ToString();
            sb.Clear();
            sb.AppendLine("$sb = [System.Text.StringBuilder]::new()");
            sb.AppendLine("$sw = [System.IO.StringWriter]::new($sb)");
            sb.AppendLine("$w  = [System.CodeDom.Compiler.IndentedTextWriter]::new($sw)");
            sb.AppendLine("$w.Indent = 1");
            sb.AppendLine("");
            using (StringReader reader = new StringReader(fullString))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    sb.Append("$w.WriteLine");
                    sb.Append("(" + Quote);
                    sb.Append(line);
                    sb.AppendLine(Quote + ")");
                }
            }

            sb.AppendLine("");
            sb.AppendLine("$sb.ToString() | Set-Clipboard");
            sb.AppendLine("");
            sb.AppendLine("$sb.ToString()");
            hostObject.SetCurrentFileText(sb.ToString());
        }

        public static void EncloseWithStringBuilder(IEditorFeaturesHostObject hostObject)
        {
            string txt;
            if (string.IsNullOrEmpty(txt = hostObject.GetSelectedText()))
            {
                txt = hostObject.CurrentEditor.Text;
            }

            StringBuilder sb = new StringBuilder(txt);
            sb.Replace(Tilde, Tilde + Tilde);
            sb.Replace(Quote, Tilde + Quote);
            sb.Replace(DollarSign, Tilde + DollarSign);
            string fullString = sb.ToString();
            sb.Clear();
            sb.AppendLine("Clear-Host");
            sb.AppendLine();
            sb.AppendLine("$sb = [System.Text.StringBuilder]::new()");
            sb.AppendLine("");
            using (StringReader reader = new StringReader(fullString))
            {
                string line;
                //while ((line = reader.ReadLine()) != null)
                //{
                //    sb.AppendLine(line);
                //}
                while ((line = reader.ReadLine()) != null)
                {
                    sb.Append("$null = $sb.AppendLine");
                    sb.Append("(" + Quote);
                    sb.Append(line);
                    sb.AppendLine(Quote + ")");
                }
            }

            sb.AppendLine("");
            sb.AppendLine("$sb.ToString() | Set-Clipboard");
            sb.AppendLine("");
            sb.AppendLine("$sb.ToString()");
            sb.AppendLine("");


            hostObject.SetCurrentFileText(sb.ToString());
        }


        public static void PasteAsStringBuilder(IEditorFeaturesHostObject hostObject)
        {
            string txt;
            if (string.IsNullOrEmpty(txt = ClipboardText))
            {
                txt = hostObject.CurrentEditor.Text;
            }

            StringBuilder sb = new StringBuilder(txt);
            sb.Replace(Tilde, Tilde + Tilde);
            sb.Replace(Quote, Tilde + Quote);
            sb.Replace(DollarSign, Tilde + DollarSign);
            string fullString = sb.ToString();
            sb.Clear();
            using (StringReader reader = new StringReader(fullString))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    sb.Append("$null = $sb.AppendLine");
                    sb.Append("(" + Quote);
                    sb.Append(line);
                    sb.AppendLine(Quote + ")");
                }
            }

            int current_column = hostObject.CurrentEditor.CaretColumn;
            int current_line = hostObject.CurrentEditor.CaretLine;

            if (current_line == 1 && current_column == 1)
            {
                hostObject.SetCurrentFileText(sb.ToString());
            }
            else
            {
                hostObject.CurrentEditor.InsertText(sb.ToString());
            }
        }

        public static void PasteAsTextWriter(IEditorFeaturesHostObject hostObject)
        {
            string txt;
            if (string.IsNullOrEmpty(txt = ClipboardText))
            {
                txt = hostObject.CurrentEditor.Text;
            }

            StringBuilder sb = new StringBuilder(txt);
            sb.Replace(Tilde, Tilde + Tilde);
            sb.Replace(Quote, Tilde + Quote);
            sb.Replace(DollarSign, Tilde + DollarSign);
            string fullString = sb.ToString();
            sb.Clear();
            using (StringReader reader = new StringReader(fullString))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    sb.Append("$w.WriteLine");
                    sb.Append("(" + Quote);
                    sb.Append(line);
                    sb.AppendLine(Quote + ")");
                }
            }

            int current_column = hostObject.CurrentEditor.CaretColumn;
            int current_line = hostObject.CurrentEditor.CaretLine;

            if (current_line == 1 && current_column == 1)
            {
                hostObject.SetCurrentFileText(sb.ToString());
            }
            else
            {
                hostObject.CurrentEditor.InsertText(sb.ToString());
            }
            
        }

        public static void PasteAsXmlTextWriter(IEditorFeaturesHostObject hostObject)
        {

        }
    }
}


