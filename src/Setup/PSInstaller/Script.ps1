$sb = [System.Text.StringBuilder]::new()
$sw = [System.IO.StringWriter]::new($sb)
$w  = [System.CodeDom.Compiler.IndentedTextWriter]::new($sw)
$w.Indent = 1
Get-ChildItem -File -Path 'C:\Stage\git\PowerShellModules\bin\Debug\Infrastructure\PowerShell.Infrastructure' | ForEach-Object -Process {
    $File = $_
    $FileName = "FILE_$($File.Name.Replace('.',''))"
    $w.WriteLine("        <File Id=`"$FileName`"")
    $w.WriteLine("              Source=`"`$(var.Infrastructure.TargetDir)\PowerShell.Infrastructure\$($File.Name)`" />")
}
$sb.ToString() | Set-Clipboard
$sb.ToString()