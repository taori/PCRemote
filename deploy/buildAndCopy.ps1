dotnet build ..\src\All.sln

Write-Host "Removing artifacts folder ..."

Remove-Item -Recurse -Force -Path ..\artifacts -ErrorAction SilentlyContinue
Write-Host "done."

Write-Host "Creating artifacts folder structure ..."
New-Item ..\artifacts -ItemType Directory | Out-Null
New-Item ..\artifacts\web -ItemType Directory | Out-Null
New-Item ..\artifacts\win-integration -ItemType Directory | Out-Null
Write-Host "done."

# Workaround global.json CWD issue
$loc = Get-Location
Set-Location -Path ..\src -PassThru
dotnet build ..\src\All.sln

dotnet publish ..\src\Amusoft.PCR.Int.Agent.Windows -c Release -o ..\artifacts\win-integration
dotnet publish ..\src\Amusoft.PCR.App.Service -c Release -o ..\artifacts\web

Set-Location -Path $loc -PassThru


if(Test-Path "..\mobile-artifacts\android\"){
    Copy-Item ..\mobile-artifacts\android\ ..\artifacts\web\wwwroot\downloads\ -Recurse -Force
}

Write-Host "buildandcopy script complete."
#Start-Sleep -Seconds 3