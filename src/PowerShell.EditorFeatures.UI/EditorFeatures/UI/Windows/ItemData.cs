namespace PowerShell.EditorFeatures.UI.Windows
{
    public class ItemData
    {
        public string Name { get; set; }
        public bool InEditMode { get; set; }

        public ItemData()
        {
            InEditMode = true;
        }
    }
}
