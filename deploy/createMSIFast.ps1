param (
    [Parameter(HelpMessage="Type of creation to apply")]
    [ValidateSet('Fresh','RecycleBoth','JustHarvest')]
    [string]$Configuration,

    [Parameter(HelpMessage="Version of product")]
    [string]$Version="3.0.0",
    
    [Parameter(HelpMessage="Whether or not to open browser at first msi match or not")]
    [bool]$OpenExplorer=$true
)

switch($Configuration){
    "Fresh" { & .\createMSI.ps1 -DeleteArtifacts $true -SkipPublish $false -SkipHarvesting $false -OpenExplorer $OpenExplorer -ProductVersion "$Version"}
    "RecycleBoth" { & .\createMSI.ps1 -DeleteArtifacts $false -SkipPublish $true -SkipHarvesting $true -OpenExplorer $OpenExplorer -ProductVersion "$Version"}
    "JustHarvest" { & .\createMSI.ps1 -DeleteArtifacts $false -SkipPublish $true -SkipHarvesting $false -OpenExplorer $OpenExplorer -ProductVersion "$Version" }
}