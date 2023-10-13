& "$PSScriptRoot\uninstallService.ps1"
& "$PSScriptRoot\buildAndCopy.ps1"
& "$PSScriptRoot\installService.ps1"

Write-Host "refreshservice script complete."
#Start-Sleep -Seconds 3