using Microsoft.PowerShell.Host.ISE;

namespace PowerShell.EditorFeatures.Core.Host
{
    public interface IEditorFeaturesHostObject : IAddOnToolHostObject
    {
        PowerShellTab CurrentTab { get; }

        ISEEditor CurrentEditor { get; }

        ISEFile CurrentFile { get; }


        ISEFile CreateFile(string text);

        string GetSelectedText();

        void SetCaretPosition(int x, int y);

        void SetCurrentFileText(string text);

        void ShiftTextLeft(int i);

        void ShiftTextRight(int i);

        void CloseAllTabs(bool forceSave);
    }
}
