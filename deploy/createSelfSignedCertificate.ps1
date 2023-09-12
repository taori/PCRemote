[CmdletBinding()]
param(
    [Parameter(Mandatory=$true, Position=0)]
    [string]$exportPath,
    [Parameter(Mandatory=$false, Position=1)]
    [string]$pfxPassword
    )
    
function getExportPath {
    if([string]::IsNullOrEmpty($exportPath) -eq $false){
        return $exportPath
    }
      
    Add-Type -AssemblyName System.Windows.Forms
                
    $browser = New-Object System.Windows.Forms.SaveFileDialog
    $browser.AddExtension = $true
    $browser.Filter = "Certificate file (*.pfx)|*.pfx"
    if($browser.ShowDialog() -eq "OK"){

        #$certPassword = ConvertTo-SecureString -String “YourPassword” -Force –AsPlainText
    
        #New-Item -Path $browser.FileName
        return $browser.FileName
     
        #$tempCertPath = [System.IO.Path]::GetTempFileName();
        #$caCertPassword = Read-Host -AsSecureString -Prompt "Select a password for the CA certificate"
        #Export-PfxCertificate -Cert $rootCert -FilePath $tempCertPath -Password $caCertPassword
     
        #Import-PfxCertificate -CertStoreLocation Cert:\LocalMachine\AuthRoot -FilePath $tempCertPath 
    } else {
        exit -1;
    }
}

$rootCert = New-SelfSignedCertificate -NotAfter (Get-Date).AddYears(10) -KeyAlgorithm "RSA" -KeyLength 2048 -HashAlgorithm "SHA256" -KeyExportPolicy Exportable -CertStoreLocation Cert:\LocalMachine\My\ -FriendlyName "Amusoft Root Certificate Authority" -Subject 'CN=AmusoftRootCA,O=AmusoftRootCA,OU=AmusoftRootCA'  -KeyUsage CertSign,CRLSign,DigitalSignature -KeyUsageProperty All -Provider 'Microsoft Enhanced RSA and AES Cryptographic Provider'

$siteCert = New-SelfSignedCertificate -NotAfter (Get-Date).AddYears(10) -KeyAlgorithm "RSA" -KeyLength 2048 -HashAlgorithm "SHA256" -KeyExportPolicy Exportable -CertStoreLocation Cert:\LocalMachine\My\ -FriendlyName "Amusoft PC Remote 2 Certificate" -Subject "localhost" -Signer $rootCert -KeyUsage KeyEncipherment,DigitalSignature -KeyUsageProperty All

$exportPath = getExportPath

if ([string]::IsNullOrEmpty($pfxPassword) -eq $true){

    $guid = [System.Guid]::NewGuid().ToString()
    $encryptedPfxPassword = ConvertTo-SecureString $guid -AsPlainText -Force
    #$encryptedPfxPassword = Read-Host -AsSecureString -Prompt "Select a password for the site certificate"
} else {
    $encryptedPfxPassword = ConvertTo-SecureString $pfxPassword -AsPlainText -Force
}


if(Test-Path $exportPath){
    Export-PfxCertificate -Cert $siteCert -FilePath $exportPath -Password $encryptedPfxPassword
} else {
    New-Item $exportPath -Force
    Export-PfxCertificate -Cert $siteCert -FilePath $exportPath -Password $encryptedPfxPassword
}

Write-Host "Script complete"
Start-Sleep -Seconds 3