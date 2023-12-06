param (
    [Parameter(HelpMessage="Type of creation to apply")]
    [ValidateSet('Fresh','RecycleBoth','JustHarvest')]
    [string]$Configuration,

    [Parameter(HelpMessage="Version of product")]
    [string]$Version="3.0.0",
    
    [Parameter(HelpMessage="Whether or not to open browser at first msi match or not")]
    [bool]$OpenExplorer=$true,
    
    [Parameter(HelpMessage="Start MSI file")]
    [bool]$OpenMsi=$false
)

$scriptPath = Resolve-Path "$PSScriptRoot\createMsi.ps1"

if($OpenMsi -eq $true){
    switch($Configuration){
        "Fresh" { $r = & "$scriptPath" -DeleteArtifacts $true -SkipPublish $false -SkipHarvesting $false -OpenExplorer $OpenExplorer -ProductVersion "$Version" }
        "RecycleBoth" { $r = & "$scriptPath" -DeleteArtifacts $false -SkipPublish $true -SkipHarvesting $true -OpenExplorer $OpenExplorer -ProductVersion "$Version" }
        "JustHarvest" { $r = & "$scriptPath" -DeleteArtifacts $false -SkipPublish $true -SkipHarvesting $false -OpenExplorer $OpenExplorer -ProductVersion "$Version" }
    }

    & $r.MsiPath    
}
else
{
    switch($Configuration){
        "Fresh" { & "$scriptPath" -DeleteArtifacts $true -SkipPublish $false -SkipHarvesting $false -OpenExplorer $OpenExplorer -ProductVersion "$Version" }
        "RecycleBoth" { & "$scriptPath" -DeleteArtifacts $false -SkipPublish $true -SkipHarvesting $true -OpenExplorer $OpenExplorer -ProductVersion "$Version" }
        "JustHarvest" { & "$scriptPath" -DeleteArtifacts $false -SkipPublish $true -SkipHarvesting $false -OpenExplorer $OpenExplorer -ProductVersion "$Version" }
    }    
}
