
Import-Module ".\functions.psm1"

#ütf8

Remove-Item -Recurse -Force -Path $(Get-ResolvedPath "$PSScriptRoot\..\artifacts\") -ErrorAction Stop

$installerProject = Get-ResolvedPath "$PSScriptRoot\..\installer\Amusoft.PCR.Installer\Amusoft.PCR.Installer.wixproj"
$installerOutput = Get-ResolvedPath "$PSScriptRoot\..\artifacts\msi\compiled"
$apkFile = Get-ResolvedPath "$PSScriptRoot\..\artifacts\msi\apk\app.apk"
$apkDirectory = [System.IO.Path]::GetDirectoryName($apkFile)
$solutionDir = Get-ResolvedPath "$PSScriptRoot\..\src\"

& ".\publishapk.ps1" -PublishFilePath "$apkFile"

&dotnet build "$installerProject" -c Release -o "$installerOutput" -p:SolutionDir=$solutionDir -p:ApkSource=$apkDirectory