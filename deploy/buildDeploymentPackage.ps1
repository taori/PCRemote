param (
    [Parameter(HelpMessage="Signing password for publish process")]
    [string]$SignPassword = $null,
    [Parameter(Mandatory=$true, HelpMessage="Build configuration")]
    [ValidateSet('Release','Debug')]
    [string]$Configuration = "Release",
    [string]$TargetFolder = "D:\tmp\PCR3SA"
)

# ü

Import-Module (Resolve-Path "$PSScriptRoot\functions.psm1")

$keyStore = Resolve-Path "$PSScriptRoot\PCR3.keystore"
$apkProject = Resolve-Path "$PSScriptRoot\..\src\Amusoft.PCR.App.UI\Amusoft.PCR.App.UI.csproj"
$webProj = Resolve-Path "$PSScriptRoot\..\src\Amusoft.PCR.App.Service\Amusoft.PCR.App.Service.csproj"
$intWinProj = Resolve-Path "$PSScriptRoot\..\src\Amusoft.PCR.Int.Agent.Windows\Amusoft.PCR.Int.Agent.Windows.csproj"

$artifactsRoot = $TargetFolder
Write-Host "Removing artifacts folder ..."
Remove-Item -Recurse -Force -Path $artifactsRoot -ErrorAction SilentlyContinue
Write-Host "done."

&dotnet publish $webProj -c $Configuration -o "$artifactsRoot\web"
&dotnet publish $intWinProj -c $Configuration -o "$artifactsRoot\win-integration"

$apkPath = Build-Android -KeyStorePath $keyStore -SignPassword $SignPassword -ProjectPath $apkProject -PublishFilePath "$artifactsRoot\web\wwwroot\app.apk" -Configuration $Configuration
Move-Item $apkPath "$artifactsRoot\web\wwwroot\app.apk" -Force
Remove-Item -Recurse -Force -Path "$artifactsRoot\tmp" -ErrorAction SilentlyContinue

&explorer "$artifactsRoot"