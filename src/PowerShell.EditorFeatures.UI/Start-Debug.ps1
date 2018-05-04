
Set-Location $PSScriptRoot

Add-Type -Path "PowerShell.EditorFeatures.UI.dll"

$psISE.CurrentPowerShellTab.VerticalAddOnTools.Add("Editor Features", [PowerShell.EditorFeatures.UI.Controls.EditorFeaturesHostObject], $true)

