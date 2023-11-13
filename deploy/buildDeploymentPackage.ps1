param (
    [Parameter(Mandatory=$true, HelpMessage="Signing password for publish process")]
    [string]$SignPassword
)

Import-Module ".\functions.psm1"

$keyStore = [System.Uri]::new([System.Uri]::new("$PSScriptRoot\.", [System.UriKind]::Absolute), [System.Uri]::new("PCR3.keystore", [System.UriKind]::Relative)).LocalPath

$apkProject = [System.Uri]::new([System.Uri]::new("$PSScriptRoot\.", [System.UriKind]::Absolute), [System.Uri]::new("..\src\Amusoft.PCR.App.UI\Amusoft.PCR.App.UI.csproj", [System.UriKind]::Relative)).LocalPath
$webProj = [System.Uri]::new([System.Uri]::new("$PSScriptRoot\.", [System.UriKind]::Absolute), [System.Uri]::new("..\src\Amusoft.PCR.App.Service\Amusoft.PCR.App.Service.csproj", [System.UriKind]::Relative)).LocalPath
$intWinProj = [System.Uri]::new([System.Uri]::new("$PSScriptRoot\.", [System.UriKind]::Absolute), [System.Uri]::new("..\src\Amusoft.PCR.Int.Agent.Windows\Amusoft.PCR.Int.Agent.Windows.csproj", [System.UriKind]::Relative)).LocalPath

$artifactsRoot = "D:\tmp\PCR3SA"
Write-Host "Removing artifacts folder ..."
Remove-Item -Recurse -Force -Path $artifactsRoot -ErrorAction SilentlyContinue
Write-Host "done."

&dotnet publish $webProj -c Release -o "$artifactsRoot\web"
&dotnet publish $intWinProj -c Release -o "$artifactsRoot\win-integration"

$apkPath = Build-Android -KeyStorePath $keyStore -SignPassword $SignPassword -ProjectPath $apkProject -PublishDirectory "$artifactsRoot\tmp\android"
Move-Item $apkPath "$artifactsRoot\web\wwwroot\app.apk"
Remove-Item -Recurse -Force -Path "$artifactsRoot\tmp" -ErrorAction SilentlyContinue

&explorer "$artifactsRoot"