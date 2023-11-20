function Build-Android {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory,ValueFromPipeline)]
        [string] $KeyStorePath, 

        [Parameter(Mandatory,ValueFromPipeline)]
        [string] $SignPassword, 

        [Parameter(Mandatory,ValueFromPipeline)]
        [string] $ProjectPath, 

        [Parameter(Mandatory,ValueFromPipeline)]
        [string] $PublishDirectory,
		
		[Parameter(Mandatory=$true, HelpMessage="Build configuration")]
		[string]$Configuration = "Release"
    )

    [Environment]::SetEnvironmentVariable('PCR3PW', $SignPassword, 'Process')

    $keyAlias = "PCR3"
    $keyPass = "env:PCR3PW"
    $storePass = "env:PCR3PW"

    # https://learn.microsoft.com/de-de/dotnet/maui/android/deployment/publish-cli ü
    $publishCode = "dotnet publish $ProjectPath -f net7.0-android -c $Configuration -o `"$PublishDirectory`" -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=$KeyStorePath -p:AndroidSigningKeyAlias=$keyAlias -p:AndroidSigningKeyPass=$keyPass -p:AndroidSigningStorePass=$storePass"
    Write-Host "$publishCode" -ForegroundColor DarkGreen
        
    #Invoke-Expression -Command $publishCode -ErrorAction Stop
    &dotnet publish $ProjectPath -f net7.0-android -c $Configuration -o `"$PublishDirectory`" -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=$KeyStorePath -p:AndroidSigningKeyAlias=$keyAlias -p:AndroidSigningKeyPass=$keyPass -p:AndroidSigningStorePass=$storePass | Out-Host
    
    $apk = Get-ChildItem "$PublishDirectory" -Filter "*.apk" | %{$_.FullName}

    return $apk
}
