using PowerShell.EditorFeatures.Core.Host;

namespace PowerShell.EditorFeatures.UI.Windows
{
    public static class CodeGenerationFactory
    {
        public static CodeGenerationInfrastructure Create(IEditorFeaturesHostObject hostObject)
        {
            CodeGenerationModel model = new CodeGenerationModel();
            CodeGenerationViewModel viewModel = new CodeGenerationViewModel(model);
            CodeGenerationWindow view = new CodeGenerationWindow(viewModel, hostObject);
            return new CodeGenerationInfrastructure(model, viewModel, view);
        }
    }
}
