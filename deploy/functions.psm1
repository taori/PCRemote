
#ütf8

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
        [string] $PublishFilePath,
		
		[Parameter(HelpMessage="Build configuration")]
        [ValidateSet('Release','Debug')]
		[string]$Configuration = "Release"
    )
    
    Write-Host "Build-Android: `"KeyStorePath`": $KeyStorePath "`
                                + " `"ProjectPath`": $ProjectPath "`
                                + " `"PublishFilePath`": $PublishFilePath "`
                                + " `"Configuration`": $Configuration "

    [Environment]::SetEnvironmentVariable('PCR3PW', $SignPassword, 'Process')

    $keyAlias = "PCR3"
    $keyPass = "env:PCR3PW"
    $storePass = "env:PCR3PW"
    
    $tempDest = New-TemporaryDirectory
    Write-Host "New Temp folder is: $tempDest"

    # https://learn.microsoft.com/de-de/dotnet/maui/android/deployment/publish-cli ü
    $publishCode = "dotnet publish $ProjectPath -f net7.0-android -c $Configuration -o `"$tempDest`" -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=$KeyStorePath -p:AndroidSigningKeyAlias=$keyAlias -p:AndroidSigningKeyPass=$keyPass -p:AndroidSigningStorePass=$storePass"
    Write-Host "$publishCode" -ForegroundColor DarkGreen
        
    #Invoke-Expression -Command $publishCode -ErrorAction Stop
    &dotnet publish $ProjectPath -f net7.0-android -c $Configuration -o `"$tempDest`" -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=$KeyStorePath -p:AndroidSigningKeyAlias=$keyAlias -p:AndroidSigningKeyPass=$keyPass -p:AndroidSigningStorePass=$storePass | Out-Host
        
    Write-Host "Looking for apk file in $tempDest" -ForegroundColor Cyan
    $apk = Get-ChildItem "$tempDest" -Filter "*.apk" | %{$_.FullName}

    Write-Host "Ensure publish directory exists for path $PublishFilePath" -ForegroundColor Cyan
    EnsureDirectoryExists ([System.IO.Path]::GetDirectoryName($PublishFilePath))

    Write-Host "Moving $apk to $PublishFilePath" -ForegroundColor Cyan
    Move-Item $apk $PublishFilePath -Force
    
    Write-Host "Removing Temp folder $tempDest" -ForegroundColor Cyan
    Remove-Item -Recurse -Force -Path "$tempDest" -ErrorAction Stop
}

function New-TemporaryDirectory {
    $parent = [System.IO.Path]::GetTempPath()
    [string] $name = [System.Guid]::NewGuid()
    $f = New-Item -ItemType Directory -Path (Join-Path "$parent" "$name")
    return $f.FullName
}

function EnsureDirectoryExists ([string]$path){
    $dirInfo = [System.IO.DirectoryInfo]::new($path)
    if($dirInfo.Exists -eq $false){
        $dirInfo.Create()
    }
}

function Get-SecretKeyValue ([string]$path, [string] $key){    
    $jsonObj = Get-Content $path | ConvertFrom-Json
    return $jsonObj."$key"
}

function Get-FirstNonNull {
    [Alias('Coalesce','Get-Coalesed')]
    param()

    end {
        $Things = @($input).ForEach{
            if ([string]::IsNullOrEmpty($_)) {
                $null
            } else { $_ }
        }
        $Things | Select-Object -First 1
    }
}

function Get-ResolvedPath($path)
{
    [System.Uri]$uri = New-Object System.Uri($path);
    return $uri.LocalPath;
}
