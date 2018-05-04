namespace PowerShell.EditorFeatures.UI.Windows
{
    public static class CodeGenerationFactory
    {
        public static CodeGenerationInfrastructure Create()
        {
            CodeGenerationModel model = new CodeGenerationModel();
            CodeGenerationViewModel viewModel = new CodeGenerationViewModel(model);
            CodeGenerationWindow view = new CodeGenerationWindow(viewModel);
            return new CodeGenerationInfrastructure(model, viewModel, view);
        }
    }
}
