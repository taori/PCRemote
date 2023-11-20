param (
    [Parameter(Mandatory=$true, HelpMessage="Signing password for publish process")]
    [string]$SignPassword,
    [Parameter(Mandatory=$true, HelpMessage="Destination folder for APK publish")]
    [string]$PublishDst
)

Import-Module ".\functions.psm1"

$keyStore = [System.Uri]::new([System.Uri]::new("$PSScriptRoot\.", [System.UriKind]::Absolute), [System.Uri]::new("PCR3.keystore", [System.UriKind]::Relative)).LocalPath
$apkProject = [System.Uri]::new([System.Uri]::new("$PSScriptRoot\.", [System.UriKind]::Absolute), [System.Uri]::new("..\src\Amusoft.PCR.App.UI\Amusoft.PCR.App.UI.csproj", [System.UriKind]::Relative)).LocalPath

$apkPath = Build-Android -KeyStorePath $keyStore -SignPassword $SignPassword -ProjectPath $apkProject -PublishDirectory $PublishDst