namespace PowerShell.EditorFeatures.UI.Windows
{
    public class CodeGenerationInfrastructure
    {
        public CodeGenerationInfrastructure(
            CodeGenerationModel model,
            CodeGenerationViewModel viewModel,
            CodeGenerationWindow view)
        {
            Model = model;
            ViewModel = viewModel;
            View = view;
        }

        public CodeGenerationModel Model { get; }
        public CodeGenerationViewModel ViewModel { get; }
        public CodeGenerationWindow View { get; }
    }
}
