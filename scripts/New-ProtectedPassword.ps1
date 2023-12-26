[cmdletbinding()]
param (
    [Switch]
    $User
)

Add-Type -AssemblyName System.Security

$SecurePassword = Read-Host "Enter Password" -AsSecureString

if ($SecurePassword.Length -lt 8) {
    Write-Error -Message "Invalid Password"
    return
}

$Cleartext = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($SecurePassword))

$Bytes = ([System.Text.Encoding]::UTF8).GetBytes($Cleartext)
$EncryptedBytes = [Security.Cryptography.ProtectedData]::Protect($Bytes, $null, [int](-not $User.IsPresent))

[Convert]::ToBase64String($EncryptedBytes)