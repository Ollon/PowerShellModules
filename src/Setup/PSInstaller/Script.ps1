$sb = [System.Text.StringBuilder]::new()
$sw = [System.IO.StringWriter]::new($sb)
$w  = [System.CodeDom.Compiler.IndentedTextWriter]::new($sw)
$w.Indent = 1
Get-ChildItem -File -Path 'D:\hold\stage\src\github\PowerShellModules\bin\Debug\PowerShell.EditorFeatures.UI' | ForEach-Object -Process {
    $File = $_
    $FileName = "FILE_$($File.Name.Replace('.',''))"
    $w.WriteLine("        <File Id=`"$FileName`"")
    $w.WriteLine("              Source=`"`$(var.PowerShell.EditorFeatures.UI.TargetDir)\PowerShell.EditorFeatures.UI\$($File.Name)`" />")
}
$sb.ToString() | Set-Clipboard
$sb.ToString()