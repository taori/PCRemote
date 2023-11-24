param (
    [Parameter(HelpMessage="Whether or not to purge the artifacts folder prior to creation")]
    [bool]$DeleteArtifacts = $true,
    [Parameter(HelpMessage="Whether or not to skip the publishing")]
    [string]$SkipPublish = $false,
    [Parameter(HelpMessage="Whether or not to skip the harvesting done by Heat(WiX)")]
    [string]$SkipHarvesting = $false,
    [Parameter(HelpMessage="Whether or not to skip the APK Build")]
    [string]$SkipAPK = $false,
    [Parameter(HelpMessage="Whether or not to open explorer afterwards")]
    [string]$OpenExplorer = $false,
    [Parameter(HelpMessage="Whether or not to open explorer afterwards")]
    [string]$ProductVersion = "3.0.0"
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

$runHeat = "false"
$runPublish = "false"
if($SkipPublish -eq $false){
    $runPublish = "true"
    if($SkipAPK -eq $false){
        & ".\publishapk.ps1" -PublishFilePath "$apkFile"        
    }
}

if($SkipHarvesting -eq $false){
    $runHeat = "true"
}

$buildCode = "dotnet build `"$installerProject`" -c Release -o `"$installerOutput`" -p:SolutionDir=`"$solutionDir`" -p:ApkSource=`"$apkDirectory`" -p:XRunPublish=$runPublish -p:XRunHeat=$runHeat -p:XProductVersion=$ProductVersion"
Write-Host $buildCode -ForegroundColor Green
Invoke-Expression $buildCode -ErrorAction Stop
#&dotnet build "$installerProject" -c Release -o "$installerOutput" -p:SolutionDir=$solutionDir -p:ApkSource=$apkDirectory -p:XRunPublish=`"$runPublish`" -p:XRunHeat=`"$runHeat`"

Get-ChildItem -Path $installerOutput -Recurse -Filter "*Installer.msi" `
    | %{
        $o = $_.FullName
        $n = "$($_.DirectoryName)\PCRemote $ProductVersion.msi"
        Write-Host "$o -> $n"
        Move-Item $o $n -Force 
    }   

if($OpenExplorer -eq $true){
    Get-ChildItem -Path $installerOutput -Recurse -Filter "*.msi" `
    | Select-Object -First 1 `
    | %{ &explorer ([System.IO.Path]::GetDirectoryName($_.FullName)) }    
}

Write-Host "Script complete." -ForegroundColor Green