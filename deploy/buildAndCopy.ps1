dotnet build ..\src\All.sln

Write-Host "Removing artifacts folder ..."

Remove-Item -Recurse -Force -Path ..\artifacts -ErrorAction SilentlyContinue
Write-Host "done."

Write-Host "Creating artifacts folder structure ..."
New-Item ..\artifacts -ItemType Directory | Out-Null
New-Item ..\artifacts\web -ItemType Directory | Out-Null
New-Item ..\artifacts\win-integration -ItemType Directory | Out-Null
Write-Host "done."

dotnet publish ..\src\Amusoft.PCR.Integration.WindowsDesktop -c Release -o ..\artifacts\win-integration
dotnet publish ..\src\Amusoft.PCR.Server -c Release -o ..\artifacts\web
if(Test-Path "..\mobile-artifacts\android\"){
    Copy-Item ..\mobile-artifacts\android\ ..\artifacts\web\wwwroot\downloads\ -Recurse -Force
}

Write-Host "Script complete"
#Start-Sleep -Seconds 3