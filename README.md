# The TameMyCerts certificate enrollment proxy

> **Commercial support**, **consulting services** and **maintenance agreements** are available on demand. [Contact me](https://www.gradenegger.eu/en/imprint/) for details if you are interested.

A server implementation of the [MS-WSTEP](https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-wstep/4766a85d-0d18-4fa1-a51f-e5cb98b752ea) protocol, mimicing the [Certificate Enrollment Web Service (CES)](https://learn.microsoft.com/en-us/previous-versions/windows/it-pro/windows-server-2012-r2-and-2012/hh831822(v=ws.11)) Windows Feature, that allows certificate Auto-Enrollment across Active Directory forest boundaries, without the requirement for a trust relationship. It is written in C# using .NET Core.

## Introduction

Microsoft [Active Directory Certificate Services (AD CS)](https://docs.microsoft.com/en-us/windows/win32/seccrypto/certificate-services) traditionally resides within an Active Directory forest and is able to automatically identify certificate enrollees and issue certificates to them, as long as they reside in the same forest boundaries. If an enterprise uses more than one Active Directory forest that have no trust relationship between each other, it would be necessary to deploy one or more certification authorities into each of these forests, increasing administrative overhead, cost and potentionally endangering security due to the increased complexity.

With the Windows 2008 R2 and Windows 7 operating system family, Microsoft introduced two new role services to AD CS:

- The [Certificate Enrollment Policy Web Service (CEP)](https://learn.microsoft.com/en-us/previous-versions/windows/it-pro/windows-server-2012-r2-and-2012/hh831625(v=ws.11)), which abstracts the LDAP queries against Domain Controllers to identify certificate templates and certification authorities in the environment.
- The Certificate Enrollment Web Service (CES), which abstracts the RPC/DCOM calls to the identified certification authorities.

These two services used in combination allow Windows certificate (Auto-)Enrollment over HTTPS via [SOAP](https://en.wikipedia.org/wiki/SOAP) messages, but the Microsoft implementations are still limited to local Active Directory forest boundaries.

## How it works

The TameMyCerts enrollment proxy ist to be installed on a Windows Server machine residing in the same Actice Directory forest as the clients and users you intend to distribute certificates to.

The enrollment proxy implements the functionality of the Certificate Enrollment Web Service using the MS-WSTEP protocol. Instead of forwarding certificate requests to a certification authority in the same Active Directory forest, it uses Representational State Transfer (REST) messages which are sent to an endpoint using the [TameMyCerts REST API](https://github.com/Sleepw4lker/TameMyCerts.REST) to submit incoming certificate requests to AD CS certification authorities that reside in another Active Directory forest. Therefore, it is not necessary to deploy a certification authority hierarchy in a forest that uses the TameMyCerts enrollment proxy, to enable Windows computers and users to get certificates issued.

To ensure the integrity of the incoming certificate requests, the TameMyCerts enrollment proxy identifies the authenticated enrollee, ensures enroll permissions, then retrieves its identity from the local Active Directory forest and submits all the required information via a CMS message signed with a Registration Authority (RA) certificate.

## Getting started

Find the most recent version as a ready-to-use binary package on the [releases page](https://github.com/Sleepw4lker/TameMyCerts.WSTEP/releases).

### Supported Operating Systems and Prerequisites

The TameMyCerts enrollment proxy was successfully tested with the following operating systems:

- Windows Server 2022

It *should* work as well with the following ones but this is yet to be tested.

- Windows Server 2019
- Windows Server 2016

Older Microsoft Windows operating systems are not supported.

### Limitations

- Currently the enrollment proxy only supports certificates to be issued immediately by the certification authority. Certificate requests put into pending state and their retrieval is not supported.
- Private Key Archival is not supported.
- As Auto-Enrollment requests do not contain any identity by nature, the enrollee's identity is verified by the enrollment proxy based on the enrollee's Kerberos authentication to the proxy. This process is limited to Subject Alternative Name (SAN) and Security Identifier (SID) certificate extensions. The Subject Distinguished Name of issued certificates will be empty. You may however populate it with static values or with the content of the requested SAN with the [TameMyCerts policy module for AD CS](https://github.com/Sleepw4lker/TameMyCerts).
- Currently, only HTTP basic authentication is implemented for the Proxy to API connection.

### Terminology

Though the server implementation is quite simple, the entire setup of the environment includes a lot of moving parts. Therefore it is necessary to clarify some terms that are used in the following.

- With **online certificate template** we mean a certificate template which is configured to build the identity of the certificate from the properties of the associated Active Directory object.
- With **offline certificate template** we mean a certificate template which is configured to build the identity of the certificate from what the enrollee provided in the associated certificate request.
- With **source forest** we mean the Active Directory forest that contains the certification authorities we want to communicate with.
- With **destination forest** we mean the Active Directory forest where our enrollment proxy will be deployed to, and where the users and computers reside that we will provide certificates to.

### Security and Implementation considerations

- Be aware that this service acts as a registration authority in the destination forest and therefore posesses the power to get certificates issued for any identity in the forest. You should treat the host as a Tier-0 asset, and protect the registration authority certificate with appropriate measures (using a Hardware Security Module is highly recommended).
- Your certification authorities should publish CRL Distribution Points (CDP) and Authority Information Access (AIA) to a HTTP address that is reachable from the destination forest, as any LDAP path will not be useable in a cross-forest setup without a trust relationship.
- If you plan to use Extended Key Usage (EKU) constraints on your certification authories, ensure that the allowed EKUs of the CA certificate contain the Object Identifier (OID) of the EKU you plan to use for the Registration Authority certificate. By default, this is "Certificate Request Agent" (_1.3.6.1.4.1.311.20.2.1_) but this can be customized in the proxy configuration.
- For the sake of Auto-Enrollment with the proxy, the certification authorities are **not** required to be populated to [NTAuthCertificates](https://www.gradenegger.eu/en/cleaning-up-the-ntauthcertificates-object/) in either forest. However, depending on your use case, it still might become necessary. If you do, be aware of the [security implications](https://www.gradenegger.eu/en/attack-vector-via-smartcard-logon-mechanism/) that come with this decision.
- For the sake of simplicity, the below description to set up the proxy uses the default Application Pool identity for the Web Services. Depending on your setup, you might want to use other measures which may complicate the matter in terms of kerberos authentication, name resolution and the like.
- You may want to further restrict or modify the content of the certificate requests on your certification authorities with the [TameMyCerts policy module for AD CS](https://github.com/Sleepw4lker/TameMyCerts).
- For high-availability, it is recommended to **not** use a load balancer but simply install a second instance of the TameMyCerts enrollment proxy.
- For the sake of security, the TameMyCerts enrollment proxy (and optionally Microsoft CEP) should be the only services installed on the web server.

### Deploying the TameMyCerts REST API in the source forest

Communications between the enrollment proxy and the certification authorities is handled via the TameMyCerts REST API. Therefore you need to deploy it in your source forest, and make it reachable from the enrollment proxy in the destination forest.

Find guidance on how to set up the REST API [here](https://github.com/Sleepw4lker/TameMyCerts.REST/blob/main/README.md).

### Configuring Group Policy in the destination forest

To reduce network load, the enrollment proxy uses the _CertificateTemplateCache_ registry key of the web server as source of information about certificate templates, which requires that Auto-Enrollment is enabled on the machine level (option "update certificates that use certificate templates"). Please ensure this is configured via group policy for the web server that will host the enrollment proxy.

### Installing IIS

For the enrollment proxy, you will need a Windows Server in the destination forest that will host the IIS web server role and the enrollment proxy service.

Deploy IIS and the required featured with the below PowerShell command:

```powershell
Install-WindowsFeature -Name Web-Server,Web-Windows-Auth,Web-Filtering,Web-IP-Security -IncludeManagementTools
```

Download and install the ASP .NET Core 8.0 [hosting bundle](https://dotnet.microsoft.com/permalink/dotnetcore-current-windows-runtime-bundle-installer).

Then ensure you have a SSL certificate installed and require SSL on the web site you plan to install the enrollment proxy onto. You probably will need to install the SSL certificate manually,

### Installing Microsoft CEP

You will only be able to use the enrollment proxy if you also have an instance of the Microsoft Certificate Enrollment Policy Web Service (CEP) that can point your clients to it. One instance of Microsoft CEP is sufficient to serve as many CES or TameMyCerts enrollment proxies as required.

You can deploy it on the same machine as the enrollment proxy, or to a distinct host. Use below PowerShell command:

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

### Configuring IIS for the enrollment proxy

Create a virtual application. Using the Default Application Pool is sufficient.

> It is advised to include the target CA name in the application name (like "TEST-CA_WSTEP"), as you will need one WSTEP endpoint per certification authority.

### Installing the enrollment proxy service

The TameMyCerts enrollment proxy will log denied requests and application errors to the Windows Event Log. Register the necessary Event Source with the below PowerShell command (as Administrator):

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

Create a certificate request in the machine context of the enrollment proxy server. The [PSCertificateEnrollment](https://github.com/Sleepw4lker/PSCertificateEnrollment) PowerShell Module might be helpful here.

Example for PSCertificateEnrollment:

```powershell
New-CertificateRequest -Subject "CN=WSTEP signing certificate" -MachineContext
```

Submit the certificate request to the certification authority in the source forest. The resulting certificate must have the "Certificate Request Agent" (_1.3.6.1.4.1.311.20.2.1_) Extended Key Usage.

Afterwards, install the certificate on the enrollment proxy server.

    certreq -accept %FILE_NAME%

Grant _Read_ permissions to "IIS_IUSRS" security group for the certificate's private key. You can do this via Microsoft Management Console (certlm.msc).

### Configuring the enrollment proxy

Edit the appsettings.json file in the application root of the enrollment proxy.

- Configure _CAName_ to the Common Name of the target certification authority (example: "TEST-CA").
- Configure the _ApiAddress_ to the base URI of the REST API server. Note that the path **must** end with a slash (example: "https://myapiendpoint.mydomain.local/tmcrest/").
- Configure the _ApiUser_ to the Domain User that will be granted enroll permissions on the certificate template in the source forest (example: "Service_WSTEP")

For the API user's password, you protect it with the machine's DPAPI key by using the provided script.

```powershell
New-ProtectedPassword.ps1
```

Now, put the output of the command into the _ApiPassword_ in the appsettings.json.

### Setting up Auto-Enrollment

> Ensure the destination forest knows and trusts the PKI of the source forest, e.g. by propagating the Root CA certificate via Group Policy.

For Auto-Enrollment to work, you must export two LDAP objects from the source forest to the destination forest:

- The _pKIEnrollmentService_ objects for each certification authority that is to be used with the enrollment proxy.
- The _pKICertificatTemplate_ objects for each certificate template that is to be used with the enrollment proxy.

> As Microsoft AD CS determines the certificate template to be used for issuance based on it's object identifier (OID), it will **not** work creating a template with the same name in the destination forest. It must use the exact same OID as in the source forest.

Export the _pKIEnrollmentService_ objects **from the source forest** with the following script that is provided with the downloadable package.

```powershell
.\Export-CertificateTemplate.ps1 -CertificationAuthority "TEST-CA" 
```

Export _pKICertificateTemplate_ objects object **from the source forest** with the same script.

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

> Note that if you edit the template in any of the two forests, its version number is increased. Ensure that the major versions of both versions of the certificate template are always identitcal, and that the minor versions are also either identical or that the minor version of the copy in the destination forest is lower as in the source forest. Otherwise, certificate requests will get denied by the certification authority.

### Configuring Group Policy

Now you can create a group policy that instructs clients to point to the CEP server (which will point them to the enrollment proxy).

The Enrollment Policy is configured in the following place:

- User Configuration \ Windows Settings \ Security Settings \ Public Key Policies \ Certificate Services Client - Certificate Enrollment Policy
- Computer Configuration \ Windows Settings \ Security Settings \ Public Key Policies \ Certificate Services Client - Certificate Enrollment Policy

Auto-Enrollment is configured in the following place:

- User Configuration \ Windows Settings \ Security Settings \ Public Key Policies \ Certificate Services Client - Auto-Enrollment
- Computer Configuration \ Windows Settings \ Security Settings \ Public Key Policies \ Certificate Services Client - Auto-Enrollment