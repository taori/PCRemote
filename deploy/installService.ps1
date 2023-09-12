Add-Type -AssemblyName System.Windows.Forms

if (!([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {

    Write-Host "Script directory: $($PSScriptRoot)"
    Write-Host "Script path: $($MyInvocation.MyCommand.Path)"
    Write-Host "Restarting script as admin"
    Start-Process PowerShell -Verb RunAs "-NoProfile -ExecutionPolicy Bypass -Command `"cd '$pwd'; & '$PSCommandPath';`"";
    exit;
}    

$serviceName = "PCR2"
$programFolder = [System.Environment+SpecialFolder]::ProgramFiles
$translatedFolder = [System.Environment]::GetFolderPath($programFolder)

Write-Host "Waiting for folder selection" -ForegroundColor Yellow
$browser = New-Object System.Windows.Forms.FolderBrowserDialog
$browser.ShowNewFolderButton = $true
if(Test-Path "$translatedFolder\Amusoft\PCR2"){
    $translatedFolder = "$translatedFolder\Amusoft\PCR2"}

$browser.SelectedPath = "$translatedFolder"
if($browser.ShowDialog() -eq "OK"){
    $folder = $browser.SelectedPath    

    Write-Host "Copying files to $folder from ..\artifacts\"
    Remove-Item "$folder\*" -Recurse -ErrorAction Stop -Force
    Copy-Item ..\artifacts\* -Recurse -ErrorAction Stop -Destination $folder
    Write-Host "Copy done." -ForegroundColor Green

    New-Item -ItemType Directory -Path "$folder\logs" -ErrorAction Stop | Out-Null
    
    $allUser = New-Object System.Security.Principal.SecurityIdentifier([System.Security.Principal.WellKnownSidType]::WorldSid, $null)
    $acl = Get-Acl -Path "$folder\logs" 
    $rule = New-Object System.Security.AccessControl.FileSystemAccessRule($allUser, "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
    $acl.SetAccessRule($rule)
    Set-Acl "$folder\logs" $acl

    #powershell -Command "New-LocalUser -Name $serviceName"
    Write-Host "Creating service ..."
    New-Service -Name "$serviceName" -BinaryPathName "$folder\web\Amusoft.PCR.Server.exe" -Description "PC Remote 2 Website" -DisplayName "Amusoft PC Remote 2 Service" -StartupType Automatic | Out-Null
    Write-Host "Launching service ..."
    Start-Service -Name "$serviceName" | Out-Null
    try{    
        Write-Host "Waiting 10 seconds for service to start ..."
        $service = Get-Service "$serviceName"
        $service.WaitForStatus([System.ServiceProcess.ServiceControllerStatus]::Running, [System.TimeSpan]::FromSeconds(10))
        Write-Host "Service launched successfully" -ForegroundColor Green
    } catch {
        Write-Host "Service failed to start" -ForegroundColor Red
    }
}

Write-Host "Script complete"
#Start-Sleep -Seconds 3