
Clear-Host

Import-Module .\PowerShell.Infrastructure -Force -Verbose

"PASCAL CASE" | ConvertTo-PascalCase -Full

"CAMEL CASE" | ConvertTo-CamelCase -Full

Test-PendingReboot

