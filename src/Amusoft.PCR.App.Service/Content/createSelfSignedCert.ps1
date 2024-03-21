    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$ExportPath,
        [Parameter(Mandatory = $true)]
        [string]$CaPassword,
        [Parameter(Mandatory = $true)]
        [string]$SslPassword,
        [Parameter(Mandatory = $false)]
        [Int32]$ValidForYears = 10
    )

    $SslPassword = $SslPassword.Trim("'")
    $CaPassword = $CaPassword.Trim("'")

    $existingCert = Get-ChildItem -Path Cert:\LocalMachine\My `
| where { $_.subject -eq 'CN=PC Remote 3 Server Certificate' } `
| Select-Object -First 1

    if ($existingCert)
    {
        Write-Output "Certificate already exists. Aborting creation"
        exit;
    }

    $rootCA = New-SelfSignedCertificate -Subject "CN=Self Signed Authority,O=Self Signed Authority,OU=localhost" `
-CertStoreLocation "Cert:\LocalMachine\My" `
-FriendlyName "Self Signed Authority" `
-KeyExportPolicy Exportable `
-KeyUsage CertSign, CRLSign, DigitalSignature `
-KeyLength 4096 `
-KeyUsageProperty All `
-KeyAlgorithm "RSA" `
-HashAlgorithm "SHA256" `
-Provider 'Microsoft Enhanced RSA and AES Cryptographic Provider' `
-NotAfter (Get-Date).AddYears($ValidForYears)

    $siteCert = New-SelfSignedCertificate -Signer $rootCA `
-FriendlyName "PC Remote 3 Server Certificate" `
-Subject "PC Remote 3 Server Certificate" `
-KeyLength 2048 `
-CertStoreLocation Cert:\LocalMachine\My `
-KeyExportPolicy Exportable `
-KeyUsage DigitalSignature, KeyEncipherment `
-DnsName localhost `
-KeyAlgorithm "RSA" `
-HashAlgorithm "SHA256" `
-Provider 'Microsoft Enhanced RSA and AES Cryptographic Provider' `
-NotAfter (Get-Date).AddYears($ValidForYears)

    $rootcaPwd = ConvertTo-SecureString -String $CaPassword -Force -AsPlainText
    Export-PfxCertificate -Cert $rootCA -FilePath "$ExportPath\rootca.pfx" -Password $rootcaPwd
    Export-Certificate -Cert $rootCA -FilePath "$ExportPath\rootca.cer"

    $sitePassword = ConvertTo-SecureString -String $SslPassword -Force -AsPlainText
    Export-PfxCertificate -Cert $siteCert -FilePath "$ExportPath\server.pfx" -Password $sitePassword
    Export-Certificate -Cert $siteCert -FilePath "$ExportPath\server.cer"

    Import-Certificate -CertStoreLocation Cert:\LocalMachine\AuthRoot -FilePath "$ExportPath\rootca.cer"

    Test-Certificate -Cert $rootCA        