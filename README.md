# The TameMyCerts certificate Auto-Enrollment proxy

> **Commercial support**, **consulting services** and **maintenance agreements** are available on demand. [Contact me](https://www.gradenegger.eu/en/imprint/) for details if you are interested.

A server implementation of the [MS-WSTEP](https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-wstep/4766a85d-0d18-4fa1-a51f-e5cb98b752ea) protocol, mimicing the [Certificate Enrollment Web Service (CES)](https://learn.microsoft.com/en-us/previous-versions/windows/it-pro/windows-server-2012-r2-and-2012/hh831822(v=ws.11)) Windows Feature, that allows certificate Auto-Enrollment across Active Directory forest boundaries, without the requirement for a trust relationship. It is written in C#.

## Introduction

Microsoft [Active Directory Certificate Services (AD CS)](https://docs.microsoft.com/en-us/windows/win32/seccrypto/certificate-services) traditionally resides within an Active Directory forest and is able to automatically identify certificate enrollees and issue certificates to them, as long as they reside in the same forest boundaries. If an enterprise uses more than one Active Directory forest that have no trust relationship between each other, it would be necessary to deploy one or more certification authorities into each of these forests, increasing administrative overhead, cost and potentionally endangering security due to the increased complexity.

With the Windows 2008 R2 and Windows 7 operating system family, Microsoft introduced two new role services to AD CS:

- The [Certificate Enrollment Policy Web Service (CEP)](https://learn.microsoft.com/en-us/previous-versions/windows/it-pro/windows-server-2012-r2-and-2012/hh831625(v=ws.11)), which abstracts the LDAP queries against Domain Controllers to identify certificate templates and certification authorities in the environment.
- The Certificate Enrollment Web Service (CES), which abstracts the RPC/DCOM calls to the identified certification authorities.

These two services used in combination allow certificate Auto-Enrollment over HTTPS via [SOAP](https://en.wikipedia.org/wiki/SOAP) messages, but the Microsoft implementations are still limited to local Active Directory forest boundaries.

## How it works

The TameMyCerts Auto-Enrollment proxy implements the functionality of the Certificate Enrollment Web Service using the MS-WSTEP protocol. Instead of forwarding certificate requests to a certification authority in the same Active Directory forest, it uses Representational State Transfer (REST) messages which are sent to the [TameMyCerts REST API](https://github.com/Sleepw4lker/TameMyCerts.REST) to submit incoming certificate requests to AD CS certification authorities that reside in another Active Directory forest. Therefore, it is not necessary to deploy a certification authority hierarchy in a forest that uses the TameMyCerts Auto-Enrollment proxy.

To ensure the integrity of the incoming certificate requests, the TameMyCerts Auto-Enrollment proxy identifies the authenticated enrollee, ensures enroll permissions, then retrieves its identity from the local Active Directory forest and submits all the required information via a CMS message signed with a Registration Authority (RA) certificate.

## Limitations

- Currently the Auto-Enrollment proxy only supports certificates to be issued immediately by the certification authority. Certificate requests put into pending state and their retrieval is not supported.
- Private Key Archival is not supported.
- As Auto-Enrollment requests do not contain any identity by nature, the identity is verified by the Auto-Enrollment proxy based on the identified Active Directory identity of the enrollee. This process is limited to Subject Alternative Name (SAN) and Security Identifier (SID) certificate extensions. The Subject Distinguished Name of issued certificates will be empty. You may however populate it with static values or with the content of the requested SAN with the [TameMyCerts policy module for AD CS](https://github.com/Sleepw4lker/TameMyCerts).

## Getting started

Find the most recent version as a ready-to-use, digitally signed binary package on the [releases page](https://github.com/Sleepw4lker/TameMyCerts.WSTEP/releases).

### Supported Operating Systems and Prerequisites

The TameMyCerts Auto-Enrollment proxy was successfully tested with the following operating systems:

- Windows Server 2022

It *should* work as well with the following ones but this is yet to be tested.

- Windows Server 2019
- Windows Server 2016

Older Microsoft Windows operating systems are not supported.

For Windows Server 2016 and below, [.NET Framework 4.7.2](https://support.microsoft.com/en-us/topic/microsoft-net-framework-4-7-2-offline-installer-for-windows-05a72734-2127-a15d-50cf-daf56d5faec2) must be installed.

### Terminology

Though the server implementation is quite simple, the entire setup of the environment includes a lot of moving parts. Therefore it is necessary to clarify some terms that are used in the following.

- With **online certificate template** we mean a certificate template which is configured to build the identity of the certificate from the properties of the associated Active Directory object.
- With **offline certificate template** we mean a certificate template which is configured to build the identity of the certificate from what the enrollee provided in the associated certificate request.
- With **source forest** we mean the Active Directory forest that contains the certification authorities we want to communicate with.
- With **destination forest** we mean the Active Directory forest where our Auto-Enrollment proxy will be deployed to, and where the users and computers reside that we will provide certificates to.

### Security and Implementation considerations

- Be aware that this service acts as a registration authority in the destination forest and therefore posesses the power to get certificates issued for any identity in the forest. You should treat the host as a Tier-0 asset therefore, and protect the registration authority certificate with appropriate measures.
- Your certification authorities should publish CRL Distribution Points (CDP) and Authority Information Access (AIA) to a HTTP address that is reachable from the destination forest, as any LDAP path will not be useable in a cross-forest setup without a trust relationship.
- If you plan to use Extended Key Usage (EKU) constraints on your certification authories, ensure that the allowed EKUs contain the Object Identifier (OID) of the EKU you plan to use for the Registration Authority certificate. By default, this is "Certificate Request Agent" (_1.3.6.1.4.1.311.20.2.1_) but this can be customized.
- You may want to further restrict certificate requests on your certification authorities with the [TameMyCerts policy module for AD CS](https://github.com/Sleepw4lker/TameMyCerts).
- For the sake of Auto-Enrollment with the proxy, the certification authorities are not required to be populated to [NTAuthCertificates](https://www.gradenegger.eu/en/cleaning-up-the-ntauthcertificates-object/) in either forest.
- For the sake of simplicity, the below process uses the default Application Pool identity for the Web Services. Depending on your setup, you might want to use other measure which may complicate the matter in terms of kerberos authentication, name resolution and the like.
- Currently, only HTTP basic authentication is implemented for the Proxy to API connection.

### Deploying the TameMyCerts REST API in the source forest

Communications between the Auto-Enrollment proxy and the certification authorities is handled via the TameMyCerts REST API. Therefore you need to deploy it in your source forest, and make it reachable from the Auto-Enrollment proxy in the destination forest.

Find guidance on how to set up the REST API here [here](https://github.com/Sleepw4lker/TameMyCerts.REST/blob/main/README.md).

### Configuring Group Policy in the destination forest

To reduce network load, the Auto-Enrollment proxy uses the _CertificateTemplateCache_ registry key of the web server as source of information about certificate templates, which requires that Auto-Enrollment is enabled on the machine level (option "update certificates that use certificate templates"). Please ensure this is configured via group policy for the web server that will host the Auto-Enrollment proxy.

### Installing IIS

For the Auto-Enrollment proxy, you will need a Windows Server in the destination forest that will host the IIS web server role and the Auto-Enrollment proxy service.

Deploy IIS and the required featured with the below PowerShell command:

```powershell
Install-WindowsFeature -Name Web-Server,Web-Asp-Net45,Web-Windows-Auth,NET-WCF-HTTP-Activation45,Web-Filtering,Web-IP-Security -IncludeManagementTools
```

Then ensure you have a SSL certificate installed and require SSL on the web site you plan to install the Auto-Enrollment proxy onto.

### Installing Microsoft CEP

You will only be able to use the Auto-Enrollment proxy if you also have an instance of the Microsoft Certificate Enrollment Policy Web Service (CEP) that can point your clients to it.

You can deploy it on the same machine as the Auto-Enrollment proxy, or to a distinct host. Use below PowerShell command:

```powershell
Install-WindowsFeature -Name ADCS-Enroll-Web-Pol -IncludeManagementTools
```

Now identify the thumbprint of the SSL certificate you deployed in the previous step.

```powershell
$thumbprint = Get-ChildItem -Path Cert:\LocalMachine\My\ | Where-Object { $_.EnhancedKeyUsageList.ObjectId -match "1.3.6.1.5.5.7.3.1" } | Sort-Object -Property NotAfter -Descending | Select-Object -First 1 | Select-Object -ExpandProperty Thumbprint
```

Finally, enable the CEP with the following command:

```powershell
Install-AdcsEnrollmentPolicyWebService -AuthenticationType Kerberos -SSLCertThumbprint $thumbprint -Force
```

### Configuring IIS for the Auto-Enrollment proxy

Create a virtual application. Using the Default Application Pool is sufficient.

> It is advised to include the target CA name in the application name (like "TEST-CA_WSTEP"), as you will need one WSTEP endpoint per certification authority.

### Installing the Auto-Enrollment proxy service

Register the Event Source with the below PowerShell command (as Administrator):

```powershell
[System.Diagnostics.EventLog]::CreateEventSource('TameMyCerts.WSTEP', 'Application')
```

- Now save the files from the _wwwroot_ folder of the downloaded package to the root folder of the virtual application.
- Enable Windows Authentication in the virtual application settings.
- Under the Windows Authentication settings, set extended Protection Mode to "always".
  
> Note that you do not need to configure Kerberos Delegation as it would be the case with the original Microsoft product.

> It is advised to implement additional server hardening.

### Enrolling for and installing the signer certificate

> You may want to protect this certificate using a Hardware Security Module.

Create a certificate request in the machine context of the Auto-Enrollment proxy server. The [PSCertificateEnrollment](https://github.com/Sleepw4lker/PSCertificateEnrollment) PowerShell Module might be helpful here.

Example for PSCertificateEnrollment:

```powershell
New-CertificateRequest -Subject "CN=WSTEP signing certificate" -MachineContext
```

Submit the certificate request to the certification authority in the source forest. The resulting certificate must have the "Certificate Request Agent" (_1.3.6.1.4.1.311.20.2.1_) Extended Key Usage.

Afterwards, install the certificate on the Auto-Enrollment proxy server.

    certreq -accept %FILE_NAME%

Grant _Read_ permissions to "IIS_IUSRS" security group for the certificate's private key. You can do this via Microsoft Management Console (certlm.msc).

### Configuring the Auto-Enrollment proxy

Edit the Web.config file in the application root of the Auto-Enrollment proxy.

- Configure _CAName_ to the Common Name of the target certification authority (example: "TEST-CA").
- Configure the _ApiAddress_ to the base URI of the REST API server. Note that the path **must** end with a slash (example: "https://myapiendpoint.mydomain.local/tmcrest/").
- Configure the _ApiUser_ to the Domain User that will be granted enroll permissions on the certificate template in the source forest (example: "Service_WSTEP")
- Under _secureAppSettings_, enter the password for the API user into _ApiPassword_. It will be encrypted with the Server's DPAPI key in the next step.

Now run the following command against the root folder of the virtual application:

    %SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis.exe -pef "secureAppSettings" "path-to-Web.config" -prov "DataProtectionConfigurationProvider"

> Omit the file name of Web.config and the trailing backslash of the path. Apply the command against the folder, not the file.

### Setting up Auto-Enrollment

> Ensure the destination forest knows and trusts the PKI of the source forest, e.g. by propagating the Root CA certificate via Group Policy.

For Auto-Enrollment to work, you must export two LDAP objects from the source forest to the destination forest:

- The _pKIEnrollmentService_ objects for each certification authority that is to be used with the Auto-Enrollment proxy.
- The _pKICertificatTemplate_ objects for each certificate template that is to be used with the Auto-Enrollment proxy.

> As Microsoft AD CS determines the certificate template to be used for issuance based on it's object identifier (OID), it will **not** work creating a template with the same name in the destination forest. It must use the exact same OID as in the source forest.

Export the _pKIEnrollmentService_ objects **from the source forest** with the following script that it provided with the downloadable package.

```powershell
.\Export-CertificateTemplate.ps1 -CertificationAuthority "TEST-CA" 
```

Export _pKICertificateTemplate_ objects object **from the source forest** with the following script that it provided with the downloadable package.

```powershell
.\Export-CertificateTemplate.ps1 -CertificateTemplate "User_Online"
```

Edit the exported files and replace the name and LDAP path of the source forest with name and LDAP path of destination forest, except for CA server host name.

Import the _pKIEnrollmentService_ and the _pKICertificateTemplate_ objects into the destination forest. For each CA object and each certificate tempplate, run the following:

    ldifde -i -f %FILE_NAME%

Identify the OID of the _pKICertificateTemplate_ objects (from the exported file). Restore the OID LDAP object for each imported certificate template.

    certutil -f -oid %TEMPLATE_OID% %TEMPLATE_NAME%

Now publish the imported certificate templates to the certification authority. You can achieve this by entering the certificate template names into the _certificateTemplates_ attribute of the _pKIEnrollmentService_ object.

Now you can point your imported certification authority to your WSTEP endpoint with the below command.

    certutil -config "%CA_DNS_NAME%\%CA_COMMON_NAME%" -enrollmentserverurl "https://%WSTEP_SERVER_DNS_NAME%/%APPLICATION_NAME%/Service.svc/CES" kerberos 1

> The configuration string describes the combination of host name and CA Common Name of the imported object (which will point to a DNS name in the source forest).

### Installing AD CS management tools in the destination forest

You may want to install AD CS management tools to get the certificate template management MMC (certtmpl.msc) in the destination forest. You can install it on the WSTEP web server or any other administrative host in the forest.

```powershell
Install-WindowsFeature -Name RSAT-ADCS-Mgmt
```

### Configuring certificate templates

Each certificate template now exists once in the source and once in the destination forest.

- The version in the **source forest** must be configured as an **offline** certificate template. The service account of the WSTEP server must get _enroll_ permission.
    - In addition, you should configure the certificate template to require an authorized signature with the "Certificate Request Agent" (_1.3.6.1.4.1.311.20.2.1_) Extended Key Usage.
    - Publish the certificate template on the certification authority.
- The version in the **destination forest** must be configured as an **online** template. You grant _enroll_ and _autoenroll_ permissions to the entities that shall request certificates from the WSTEP server.

> Note that if you edit the template in any of the two forests, its version number is increased. Ensure that the major versions of both versions are always the same, and that the minor versions are either identical or that the minor version of the copy in the destination forest is lower as in the source forest. Otherwise, certificate requests will get denied by the certification authority.

### Configuring Group Policy

Now you can create a group policy that instructs clients to point to the CEP server (which will point them to the Auto-Enrollment proxy).

The Enrollment Policy is configured in the following place:

- User Configuration \ Windows Settings \ Security Settings \ Public Key Policies \ Certificate Services Client - Certificate Enrollment Policy
- Computer Configuration \ Windows Settings \ Security Settings \ Public Key Policies \ Certificate Services Client - Certificate Enrollment Policy

Auto-Enrollment is configured in the following place:

- User Configuration \ Windows Settings \ Security Settings \ Public Key Policies \ Certificate Services Client - Auto-Enrollment
- Computer Configuration \ Windows Settings \ Security Settings \ Public Key Policies \ Certificate Services Client - Auto-Enrollment