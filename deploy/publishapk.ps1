param (
    [Parameter(HelpMessage="Signing password for publish process")]
    [string]$SignPassword,
    [Parameter(Mandatory=$true, HelpMessage="Target filepath for the APK-file")]
    [string]$PublishFilePath
)

#ütf8

Import-Module ".\functions.psm1"

$SecretSignPassword = Get-SecretKeyValue "$env:APPDATA\microsoft\UserSecrets\fff9ecb8-0258-4f9f-8333-3da6f78ab9ce\secrets.json" "SignPassword"
$SignPassword = $SignPassword, $SecretSignPassword | Get-FirstNonNull

$keyStore = Get-ResolvedPath "$PSScriptRoot\PCR3.keystore"
$apkProject = Get-ResolvedPath "$PSScriptRoot\..\src\Amusoft.PCR.App.UI\Amusoft.PCR.App.UI.csproj"

Build-Android -KeyStorePath $keyStore -SignPassword $SignPassword -ProjectPath $apkProject -PublishFilePath $PublishFilePath