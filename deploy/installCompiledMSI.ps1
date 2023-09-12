[cmdletbinding()]
param(
    [Parameter(Mandatory=$True)]
    [ValidateSet('Release','Debug')]
    [string]$Configuration,
    [Parameter(Mandatory=$False)]    
    [bool]$LoggingEnabled=$True
)

function Get-ResolvedPath($path)
{
    [System.Uri]$uri = New-Object System.Uri($path);    
    return $uri.LocalPath;
}

# get directory path of powershell script
$currentDirectory = $PSScriptRoot
$msiName = "Amusoft PC Remote 2.msi"
$msiPath = Get-ResolvedPath ($currentDirectory + "\..\artifacts\msi-compiled\$Configuration\$msiName")
$logPath = Get-ResolvedPath ($currentDirectory + "\..\artifacts\msi-compiled\$Configuration\install.log")

if(!(Test-Path $msiPath))
{
    Write-Error "MSI not found in $msiPath"
    Exit    
}

Write-Host "Running MSI @ $msiPath"
Write-Host "Logging to $logPath"

& msiexec /i $msiPath /L*v $logPath