using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PowerShell.EditorFeatures.UI.Windows
{
    public class CodeGenerationViewModel : INotifyPropertyChanged
    {
        private readonly CodeGenerationModel _model;

        public CodeGenerationViewModel(CodeGenerationModel model)
        {
            _model = model;
        }




        public string InputText
        {
            get
            {
                return _model.InputText;
            }
            set
            {
                if (_model.InputText == value) return;

                _model.InputText = value;
                OnPropertyChanged();
            }
        }


        public string OutputText
        {
            get
            {
                return _model.OutputText;
            }
            set
            {
                if (_model.OutputText == value) return;

                _model.OutputText = value;
                OnPropertyChanged();
            }
        }

        public bool UseDefaultFormatting
        {
            get
            {
                return _model.UseDefaultFormatting;
            }
            set
            {
                if (_model.UseDefaultFormatting == value) return;

                _model.UseDefaultFormatting = value;
                OnPropertyChanged();
            }
        }
        public bool ClosingParenthesisOnNewLine
        {
            get
            {
                return _model.ClosingParenthesisOnNewLine;
            }
            set
            {
                if (_model.ClosingParenthesisOnNewLine == value) return;

                _model.ClosingParenthesisOnNewLine = value;
                OnPropertyChanged();
            }
        }

        public bool OpenParenthesisOnNewLine
        {
            get
            {
                return _model.OpenParenthesisOnNewLine;
            }
            set
            {
                if (_model.OpenParenthesisOnNewLine == value) return;

                _model.OpenParenthesisOnNewLine = value;
                OnPropertyChanged();
            }
        }

        public bool ShortenCodeWithUsingStatic
        {
            get
            {
                return _model.ShortenCodeWithUsingStatic;
            }
            set
            {
                if (_model.ShortenCodeWithUsingStatic == value) return;

                _model.ShortenCodeWithUsingStatic = value;
                OnPropertyChanged();
            }
        }

        public bool RemoveRedundantModifyingCalls
        {
            get
            {
                return _model.RemoveRedundantModifyingCalls;
            }
            set
            {
                if (_model.RemoveRedundantModifyingCalls == value) return;

                _model.RemoveRedundantModifyingCalls = value;
                OnPropertyChanged();
            }
        }

        public bool BlankLineBetweenMembers
        {
            get
            {
                return _model.BlankLineBetweenMembers;
            }
            set
            {
                if (_model.BlankLineBetweenMembers == value) return;

                _model.BlankLineBetweenMembers = value;
                OnPropertyChanged();
            }
        }

        public string BracingStyle
        {
            get
            {
                return _model.BracingStyle;
            }
            set
            {
                if (_model.BracingStyle == value) return;

                _model.BracingStyle = value;
                OnPropertyChanged();
            }
        }

        public bool ElseOnClosing
        {
            get
            {
                return _model.ElseOnClosing;
            }
            set
            {
                if (_model.ElseOnClosing == value) return;

                _model.ElseOnClosing = value;
                OnPropertyChanged();
            }
        }

        public int IndentSpaces
        {
            get
            {
                return _model.IndentSpaces;
            }
            set
            {
                if (_model.IndentSpaces == value) return;

                _model.IndentSpaces = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
