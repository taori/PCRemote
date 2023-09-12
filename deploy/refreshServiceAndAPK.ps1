& "$PSScriptRoot\uninstallService.ps1"
& "$PSScriptRoot\androidBuild.ps1"
& "$PSScriptRoot\buildAndCopy.ps1"
& "$PSScriptRoot\installService.ps1"

Write-Host "Script complete"
#Start-Sleep -Seconds 3