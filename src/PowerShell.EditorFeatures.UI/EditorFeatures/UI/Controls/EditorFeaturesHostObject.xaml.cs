using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

using Microsoft.PowerShell.Host.ISE;
using PowerShell.EditorFeatures.Core.Features.Restart;
using PowerShell.EditorFeatures.Core.Features.SmartPaste;
using PowerShell.EditorFeatures.Core.Host;
using PowerShell.EditorFeatures.UI.Windows;

namespace PowerShell.EditorFeatures.UI.Controls
{
    public partial class EditorFeaturesHostObject : UserControl, IEditorFeaturesHostObject
    {
        public EditorFeaturesHostObject()
        {
            InitializeComponent();

        }

        public ObjectModelRoot HostObject { get; set; }


        public PowerShellTab CurrentTab
        {
            get
            {
                return HostObject.CurrentPowerShellTab;
            }
        }

        public ISEEditor CurrentEditor
        {
            get
            {
                return CurrentFile.Editor;
            }
        }

        public ISEFile CurrentFile
        {
            get
            {
                return HostObject
                    .CurrentPowerShellTab
                    .Files
                    .SelectedFile;
            }
        }

        public ISEFile CreateFile(string text)
        {
            ISEFile newFile = CurrentTab.Files.Add();
            newFile.Editor.Text = text;
            newFile.Editor.SetCaretPosition(1, 1);
            return newFile;
        }

        public string GetSelectedText()
        {
            return CurrentEditor.SelectedText;
        }

        public void SetToFirstPosition()
        {
            SetCaretPosition(1, 1);
        }

        public void SetCaretPosition(int x, int y)
        {
            CurrentFile.Editor.SetCaretPosition(x, y);
        }

        public void SetCurrentFileText(string text)
        {
            CurrentFile.Editor.Text = text;
        }

        public void ShiftTextLeft(int i)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                IEnumerable<string> lines = GetLines(CurrentFile.Editor.SelectedText);
                foreach (string line in lines)
                {
                    if (line.Length >= i)
                    {
                        sb.Append(line.Substring(i));
                    }
                    else
                    {
                        sb.AppendLine();
                    }
                }

                CurrentEditor.InsertText(sb.ToString().TrimEnd());
            }
            catch
            {
            }
        }

        public void ShiftTextRight(int i)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                IEnumerable<string> lines = GetLines(GetSelectedText());
                foreach (string line in lines)
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        sb.AppendLine();
                    }
                    else
                    {
                        for (int j = 0; j < i; j++)
                        {
                            sb.Append(" ");
                        }
                        sb.Append(line);
                    }
                }

                CurrentEditor.InsertText(sb.ToString().TrimEnd());
            }
            catch
            {
            }
        }

        public void CloseAllTabs(bool forceSave)
        {
            foreach (PowerShellTab powerShellTab in HostObject.PowerShellTabs)
            {
                var random = new Random(1);
                powerShellTab.ShowCommands = true;
                int count = powerShellTab.Files.Count;

                if (forceSave)
                {
                    foreach (ISEFile file in powerShellTab.Files)
                    {
                        string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        string psISEFolder = Path.Combine(myDocuments, "WindowsPowerShell", "Scripts", "Backup");
                        if (!Directory.Exists(psISEFolder))
                        {
                            Directory.CreateDirectory(psISEFolder);
                        }

                    
                        file.SaveAs(Path.Combine(psISEFolder, $"PowerShellScript{random.Next()}_{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Year}.ps1"));
                    }
                }

                powerShellTab.Files.Clear();
            }
        }

        private IEnumerable<string> GetLines(string text)
        {
            List<string> lines = new List<string>();
            if (string.IsNullOrEmpty(text))
            {
                return new string[] { };
            }

            string[] splits = text.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            foreach (string split in splits)
            {
                lines.Add(split + Environment.NewLine);
            }

            return lines.ToArray();
        }

        private void ShiftLeftButton_Click(object sender, RoutedEventArgs e)
        {
            ShiftTextLeft(1);
        }

        private void ShiftRightButton_Click(object sender, RoutedEventArgs e)
        {
            ShiftTextRight(1);

        }

        private void OnPasteAsStringBuilderButton_Click(object sender, RoutedEventArgs e)
        {
            SmartPaster.PasteAsStringBuilder(this);
        }



        private void OnRemoveEmptyLinesButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentFile.Editor.Text = Regex.Replace(CurrentFile.Editor.Text, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);

        }

        private void OnGenerateCodeButton_Click(object sender, RoutedEventArgs e)
        {
            var infra = CodeGenerationFactory.Create();

            var view = infra.View;

            if (view.ShowDialog() == true)
            {
                SetCurrentFileText(view.OutputText);
            }
        }

        private void OnCloseAllTabsButton_Click(object sender, RoutedEventArgs e)
        {
            CloseAllTabs(true);
        }

        private void OnRestartPowerShellButton_Click(object sender, RoutedEventArgs e)
        {
            Restarter.Restart(this);
        }

        private void EncloseWithXmlTextWriterButton_Click(object sender, RoutedEventArgs e)
        {
            SmartPaster.EncloseWithXmlTextWriter(this);
        }

        private void EncloseWithStringBuilderButton_Click(object sender, RoutedEventArgs e)
        {
            SmartPaster.EncloseWithStringBuilder(this);
        }

        private void EncloseWithTextWriterButton_Click(object sender, RoutedEventArgs e)
        {
            SmartPaster.EncloseWithTextWriter(this);
        }

        private void OnPasteAsTextWriterButton_Click(object sender, RoutedEventArgs e)
        {


            SmartPaster.PasteAsTextWriter(this);
        }
    }
}
