<#
    .SYNOPSIS
    Exports all certificate templates bound to our test certification authority to LDIF files.
#>

#Requires -Modules ActiveDirectory

[cmdletbinding()]
param (
    [Parameter(Mandatory=$False)]
    [ValidateNotNullOrEmpty()]
    [String[]]
    $CertificateTemplate,

    [Parameter(Mandatory=$False)]
    [ValidateNotNullOrEmpty()]
    [String[]]
    $CertificationAuthority
)

New-Variable -Option Constant -Name BUILD_NUMBER_WINDOWS_2016 -Value 14393

If ([int](Get-WmiObject -Class Win32_OperatingSystem).BuildNumber -lt $BUILD_NUMBER_WINDOWS_2016) {
    Write-Error -Message "This must be run on Windows Server 2016 or newer! Aborting."
    Return 
}

If (-not ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Error -Message "This must be run as Administrator! Aborting."
    Return
}

If (-not (Get-WmiObject -Class Win32_ComputerSystem).PartOfDomain) {
    Write-Error "You must install the domain first!"
    Return
}

Function Remove-InvalidFileNameChars {

    [cmdletbinding()]
    param(
        [Parameter(Mandatory=$True)]
        [ValidateNotNullOrEmpty()]
        [String]$Name
    )

    process {

        $invalidChars = [IO.Path]::GetInvalidFileNameChars() -join ''
        $re = "[{0}]" -f [RegEx]::Escape($invalidChars)
        return ($Name -replace $re)

    }
}

$DomainController = Get-ADDomainController -DomainName $TargetDomain -Discover
$Forest = Get-ADForest -Identity $DomainController.Forest -Server ($DomainController.HostName | Select-Object -First 1)
$ForestRootDomain = $($Forest | Select-Object -ExpandProperty RootDomain | Get-ADDomain).DistinguishedName
$ConfigNC = "CN=Configuration,$ForestRootDomain"

foreach ($Item in $CertificateTemplate) {

    if (-not (Test-Path -Path "AD:CN=$($Item),CN=Certificate Templates,CN=Public Key Services,CN=Services,$ConfigNC")) {
        Write-Warning -Message "Certificate template ""$Item"" was not found!"
    }

    $FilePath = "$(Remove-InvalidFileNameChars -Name $Item).ldf"

    Remove-Item -Path $FilePath -ErrorAction SilentlyContinue

    $Arguments = @(
        "-f"
        "$FilePath"
        "-d"
        "CN=$($Item),CN=Certificate Templates,CN=Public Key Services,CN=Services,$ConfigNC"
        "-p"
        "Base"
        "-o"
        "dSCorePropagationData,whenChanged,whenCreated,uSNCreated,uSNChanged,objectGuid"
    )
    [void](& ldifde $Arguments)
}


foreach ($Item in $CertificationAuthority) {

    if (-not (Test-Path -Path "AD:CN=$($Item),CN=Enrollment Services,CN=Public Key Services,CN=Services,$ConfigNC")) {
        Write-Warning -Message "Certification Authority ""$Item"" was not found!"
    }

    $FilePath = "$(Remove-InvalidFileNameChars -Name $Item).ldf"

    Remove-Item -Path $FilePath -ErrorAction SilentlyContinue

    $Arguments = @(
        "-f"
        "$FilePath"
        "-d"
        "CN=$($Item),CN=Enrollment Services,CN=Public Key Services,CN=Services,$ConfigNC"
        "-p"
        "Base"
        "-o"
        "dSCorePropagationData,whenChanged,whenCreated,uSNCreated,uSNChanged,objectGuid,certificateTemplates"
    )
    [void](& ldifde $Arguments)
}