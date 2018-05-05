using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.PowerShell.Host.ISE;
using PowerShell.EditorFeatures.Core.Host;
using PowerShell.EditorFeatures.Core.Parsing;

namespace PowerShell.EditorFeatures.UI.Windows
{
    /// <summary>
    /// Interaction logic for CodeGenerationWindow.xaml
    /// </summary>
    public partial class CodeGenerationWindow : Window
    {
        private readonly CodeGenerationViewModel _viewModel;


        private IEditorFeaturesHostObject HostObject { get; set; }

        private TextDocument OutputDocument => OutputTextEditor.Document;

        private TextDocument InputDocument => InputTextEditor.Document;

        public string OutputText => OutputDocument.Text;

        public string InputText => InputDocument.Text;

        public string NewFileText { get; set; }


        public CodeGenerationWindow()
        {
            InitializeComponent();

            InputDocument.Text = Settings.Default.InputText;
            OutputDocument.Text = Settings.Default.OutputText;

        }

        public CodeGenerationWindow(CodeGenerationViewModel viewModel, IEditorFeaturesHostObject hostObject)
        {
            HostObject = hostObject;
            _viewModel = viewModel;
            InitializeComponent();
            DataContext = _viewModel;
            InputDocument.Text = Settings.Default.InputText;
            OutputDocument.Text = Settings.Default.OutputText;
            _viewModel.InputText = Settings.Default.InputText;
            _viewModel.OutputText = Settings.Default.OutputText;
            _viewModel.ClosingParenthesisOnNewLine = Settings.Default.ClosingParenthesisOnNewLine;
            _viewModel.OpenParenthesisOnNewLine = Settings.Default.OpenParenthesisOnNewLine;
            _viewModel.RemoveRedundantModifyingCalls = Settings.Default.RemoveRedundantModifyingCalls;
            _viewModel.UseDefaultFormatting = Settings.Default.UseDefaultFormatting;
            _viewModel.ShortenCodeWithUsingStatic = Settings.Default.ShortenCodeWithUsingStatic;

        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            NewFileText = OutputText;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OnCSharpEnumToPowerShellScript_Click(object sender, RoutedEventArgs e)
        {

            CSharpCodeParser parser = new CSharpCodeParser();

            CodeCompileUnit unit = parser.ParseCodeCompileUnit(InputText);

            string name = string.Empty;

            if (unit != null && unit.Namespaces.Count > 0 && unit.Namespaces[0].Types.Count > 0)
            {
                name = unit.Namespaces[0].Types[0].Name;
            }


            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"$code = @'");
            sb.AppendLine(InputText);
            sb.AppendLine("'@");
            sb.AppendLine();
            sb.AppendLine("Add-Type -TypeDefinition $code");
            sb.AppendLine();
            sb.AppendLine($"[System.Enum]::GetNames([{name}]) | ForEach-Object -Process {{");
            sb.AppendLine("        $Name = $_");
            sb.AppendLine("        $null = $sb.AppendLine($Name)");
            sb.AppendLine("}");

            OutputTabItem.SetValue(TabItem.IsSelectedProperty, true);

            Paragraph paragraph = new Paragraph();
            paragraph.Style = CreateParagraphStyle();
            paragraph.Inlines.Add(EncloseWithStringBuilder(sb.ToString()));



            OutputDocument.Text = string.Empty;


            OutputDocument.Text = EncloseWithStringBuilder(sb.ToString());


            OutputTextEditor.Focus();
        }

        private string EncloseWithStringBuilder(string text)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("$sb = [System.Text.StringBuilder]::new()");
            sb.AppendLine();
            sb.AppendLine();
            sb.Append(text);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("$sb.ToString() | Set-Clipboard");
            sb.AppendLine("");
            sb.AppendLine("$sb.ToString()");
            sb.AppendLine("");
            return sb.ToString();
        }

        private Style CreateParagraphStyle()
        {

            Style style = new Style { TargetType = typeof(Paragraph) };
            style.Setters.Add(CreateSetter(FontFamilyProperty, new System.Windows.Media.FontFamily("Consolas")));
            style.Setters.Add(CreateSetter(FontSizeProperty, (double)14));
            style.Setters.Add(CreateSetter(Block.LineHeightProperty, (double)11));
            style.Setters.Add(CreateSetter(Block.LineStackingStrategyProperty, LineStackingStrategy.BlockLineHeight));
            return style;
        }

        private Setter CreateSetter(DependencyProperty property, object value)
        {
            Setter setter = new Setter
            {
                Property = property,
                Value = value
            };

            return setter;
        }

        private ObservableCollection<ItemData> GetListViewItems(ListBox l)
        {
            ObservableCollection<ItemData> list = new ObservableCollection<ItemData>();
            foreach (ItemData item in l.Items)
            {
                list.Add(item);
            }
            return list;
        }


        private void OnCSharpCodeToCodeDom_Click(object sender, RoutedEventArgs e)
        {
            CodeDomCodeGenerator generator = new CodeDomCodeGenerator();
            CodeCompileUnit unit = generator.Parse(InputText);
            StringBuilder sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                generator.GenerateCodeFromCompileUnit(unit, writer, new CodeGeneratorOptions());
            }
            OutputTabItem.SetValue(TabItem.IsSelectedProperty, true);
            OutputDocument.Text = string.Empty;
            OutputDocument.Text = sb.ToString();
            OutputTextEditor.Focus();
        }

        private void OnCSharpCodeToRoslyn_Click(object sender, RoutedEventArgs e)
        {
            Quoter quoter = new Quoter
            {
                ClosingParenthesisOnNewLine = _viewModel.ClosingParenthesisOnNewLine,
                OpenParenthesisOnNewLine = _viewModel.OpenParenthesisOnNewLine,
                UseDefaultFormatting = _viewModel.UseDefaultFormatting,
                ShortenCodeWithUsingStatic = _viewModel.ShortenCodeWithUsingStatic,
                RemoveRedundantModifyingCalls = _viewModel.RemoveRedundantModifyingCalls
            };



        }
    }
}
