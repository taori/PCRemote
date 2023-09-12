$msbuildPath = &"${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe
#/p:AndroidKeyStore=true /p:AndroidSigningKeyAlias="$env:ANDROID_KEY_ALIAS" /p:AndroidSigningKeyPass="$env:ANDROID_KEY_PASS" /p:AndroidSigningKeyStore="$env:WS\android.keystore" /p:AndroidSigningStorePass="$env:ANDROID_KEYSTORE_PASS"
$scriptRoot = $PSScriptRoot

#$extensionRoot = "$msbuildPath\..\..\..\..\..\Community\MSBuild"

Write-Host $scriptRoot
Write-Host $msbuildPath
if(Test-Path "$scriptRoot\..\artifacts\android\"){
    Write-Host "Deleting old artifacts" -ForegroundColor Green
    Remove-Item $scriptRoot\..\artifacts\android\ -Recurse -Force
}

#/p:MSBuildExtensionsPath="$extensionRoot"
&$msbuildPath "$scriptRoot\..\src\Amusoft.PCR.Mobile.Droid\Amusoft.PCR.Mobile.Droid.csproj" /verbosity:minimal /restore /t:"SignAndroidPackage" /bl /p:Configuration=Release /p:OutputPath="$scriptRoot\..\artifacts\android" 
New-Item -ItemType Directory -Force -Path "$scriptRoot\..\mobile-artifacts\android" | Out-Null

Write-Host "APK built" -ForegroundColor Green

Get-ChildItem "$scriptRoot\..\artifacts\android\" -Filter "*.apk" | Foreach { Move-Item $_.FullName "$scriptRoot\..\mobile-artifacts\android\$($_.Name)" -Force }
Get-ChildItem "$scriptRoot\..\mobile-artifacts\android" | Foreach { Move-Item $_.FullName $_.FullName.Replace("-Signed","") -Force }

Write-Host "APK moved to server copy source" -ForegroundColor Green