
if ($Host.Name -eq 'Windows PowerShell ISE Host')
{
    $ISEExtensionsFolder = [System.IO.Path]::Combine($env:ProgramFiles,"WindowsPowerShell","Modules","PowerShell.EditorFeatures.UI")

    $ISEExtensionFile = [System.IO.Path]::Combine($ISEExtensionsFolder, "PowerShell.EditorFeatures.UI.dll")


    if (Test-Path $ISEExtensionFile)
    {
        Add-Type -Path "$ISEExtensionFile"

        $psISE.CurrentPowerShellTab.VerticalAddOnTools.Add("Editor Features", [PowerShell.EditorFeatures.UI.Controls.EditorFeaturesHostObject], $true)

    }

    try
    {
        Import-Module -Name PsISEProjectExplorer
    }
    catch
    {

    }
}

