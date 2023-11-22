param (
    [Parameter(HelpMessage="Whether or not to purge the artifacts folder prior to creation")]
    [bool]$DeleteArtifacts = $true,
    [Parameter(HelpMessage="Whether or not to skip the publishing")]
    [string]$SkipPublish = $false,
    [Parameter(HelpMessage="Whether or not to skip the harvesting done by Heat(WiX)")]
    [string]$SkipHarvesting = $false
)

Import-Module ".\functions.psm1"

#Write-Host "Generating MSI with artifacts deletion: $DeleteArtifacts skipPublish: $SkipPublish skipHeat: $SkipHarvesting"

#ütf8

if($DeleteArtifacts -eq $true){
    Remove-Item -Recurse -Force -Path $(Get-ResolvedPath "$PSScriptRoot\..\artifacts\") -ErrorAction Stop    
}

$installerProject = Get-ResolvedPath "$PSScriptRoot\..\installer\Amusoft.PCR.Installer\Amusoft.PCR.Installer.wixproj"
$installerOutput = Get-ResolvedPath "$PSScriptRoot\..\artifacts\msi\compiled"
$apkFile = Get-ResolvedPath "$PSScriptRoot\..\artifacts\msi\apk\app.apk"
$apkDirectory = [System.IO.Path]::GetDirectoryName($apkFile)
$solutionDir = Get-ResolvedPath "$PSScriptRoot\..\src\"

$buildArgs = ""

if($SkipPublish -eq $false){
    & ".\publishapk.ps1" -PublishFilePath "$apkFile"    
}
else
{
    $buildArgs += " -p:XRunPublish=false"
}

if($SkipHarvesting -eq $true){
    $buildArgs += " -p:XRunHeat=false"
}

$buildArgs = $buildArgs.trimstart(" ")

Write-Host "dotnet build `"$installerProject`" -c Release -o `"$installerOutput`" -p:SolutionDir=`"$solutionDir`" -p:ApkSource=`"$apkDirectory`" $buildArgs" -ForegroundColor Green
&dotnet build "$installerProject" -c Release -o "$installerOutput" -p:SolutionDir=$solutionDir -p:ApkSource=$apkDirectory $buildArgs

Write-Host "Script complete." -ForegroundColor Green