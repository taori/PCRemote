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
    [Parameter(HelpMessage="Product Version for MSI")]
    [string]$ProductVersion = "3.0.0"
)

Import-Module (Resolve-Path "$PSScriptRoot\functions.psm1")

# todo dotnet publish -p:PublishProfile=Package -f net7.0-windows10.0.19041.0 -c Release -o d:\tmp\mauagain -p:PublishDir=D:\tmp\mauiagain2

#Write-Host "Generating MSI with artifacts deletion: $DeleteArtifacts skipPublish: $SkipPublish skipHeat: $SkipHarvesting"

#ütf8

if($DeleteArtifacts -eq $true){
    $artifactsRoot = Resolve-Path "$PSScriptRoot\..\artifacts\"
    if(Test-Path $artifactsRoot -eq $true){
        Remove-Item -Recurse -Force -Path $artifactsRoot -ErrorAction Stop        
    }    
}

$apkFile = Resolve-Path "$PSScriptRoot\..\artifacts\msi\apk\app.apk"
$installerProject = Resolve-Path "$PSScriptRoot\..\installer\Amusoft.PCR.Installer\Amusoft.PCR.Installer.wixproj"
$installerOutput = Resolve-Path "$PSScriptRoot\..\artifacts\msi\compiled"
$apkDirectory = [System.IO.Path]::GetDirectoryName($apkFile)
$solutionDir = Resolve-Path "$PSScriptRoot\..\src\"

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

$buildCode = "dotnet build `"$installerProject`" -c Release -o `"$installerOutput`" -p:SolutionDir=`"$solutionDir`" -p:ApkSource=`"$apkDirectory`" -p:XRunPublish=$runPublish -p:XRunHeat=$runHeat -p:XProductVersion=$ProductVersion -p:XSelfContainedPublish=false"
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

$msiPath = Get-ChildItem -Path "." -Recurse -Filter "*.msi" `
            | Where-Object { $_.FullName -like "*en-US*" } `
            | % { $_.FullName }

if($OpenExplorer -eq $true){
    &explorer ([System.IO.Path]::GetDirectoryName($msiPath))
}

Write-Host "Script complete." -ForegroundColor Green

return @{
    ApkPath = $apkFile
    MsiPath = $msiPath
}