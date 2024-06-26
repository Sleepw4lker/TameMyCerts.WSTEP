﻿// Copyright (c) Uwe Gradenegger <info@gradenegger.eu>

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using CERTENROLLLib;
using TameMyCerts.NetCore.Common.ClassExtensions;
using TameMyCerts.NetCore.Common.Enums;
using TameMyCerts.NetCore.Common.Models;
using TameMyCerts.NetCore.Common.X509;
using TameMyCerts.WSTEP.Models.MS_WSTEP;

namespace TameMyCerts.WSTEP.Connectors;

internal class Gatekeeper
{
    private readonly string _forestRootDomain = Forest.GetCurrentForest().Name;
    private readonly CObjectId _signatureHashAlgorithm = new();
    private readonly CSignerCertificate _signerCertificate = new();

    public Gatekeeper(string caName, string eku = WinCrypt.szOID_ENROLLMENT_AGENT)
    {
        _signerCertificate.Initialize(
            true,
            X509PrivateKeyVerify.VerifyNone,
            EncodingType.XCN_CRYPT_STRING_HEXRAW,
            GetSigningCertificate(caName, eku)?.Thumbprint);

        _signatureHashAlgorithm.InitializeFromAlgorithmName(
            ObjectIdGroupId.XCN_CRYPT_HASH_ALG_OID_GROUP_ID,
            ObjectIdPublicKeyFlags.XCN_CRYPT_OID_INFO_PUBKEY_ANY,
            AlgorithmFlags.AlgorithmFlagsNone,
            "SHA256");
    }

    ~Gatekeeper()
    {
        Marshal.ReleaseComObject(_signerCertificate);
        Marshal.ReleaseComObject(_signatureHashAlgorithm);
    }

    // TODO:
    // - put all error strings into resource file
    public string VerifyRequest(RequestSecurityTokenType requestSecurityToken, WindowsIdentity enrollee,
        out List<string> attributeList)
    {
        if (requestSecurityToken.RequestType != "http://docs.oasis-open.org/ws-sx/ws-trust/200512/Issue" ||
            requestSecurityToken.BinarySecurityToken.ValueType !=
            "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd#PKCS7")
        {
            throw Deny(string.Format(
                "The request type is invalid. Request type: {0}, certificate request type: {1}.",
                requestSecurityToken.RequestType, requestSecurityToken.BinarySecurityToken.ValueType));
        }

        #region Parse certificate request

        var certificateRequestPkcs10 = new CX509CertificateRequestPkcs10();

        if (!certificateRequestPkcs10.TryInitializeFromInnerRequest(
                requestSecurityToken.BinarySecurityToken.Value, CertCli.CR_IN_CMC))
        {
            Marshal.ReleaseComObject(certificateRequestPkcs10);
            throw Deny("Unable to parse the certificate request.");
        }

        #endregion

        #region Get certificate template information from request

        if (!(certificateRequestPkcs10.GetCertificateTemplateOid() is Oid templateOid))
        {
            Marshal.ReleaseComObject(certificateRequestPkcs10);
            throw Deny(
                "Unable to get certificate template object identifier from the certificate request.");
        }

        if (!(CertificateTemplate.Create(templateOid) is CertificateTemplate certificateTemplate))
        {
            Marshal.ReleaseComObject(certificateRequestPkcs10);
            throw Deny(
                $"Unable to retrieve certificate template information from registry for {templateOid.Value}.");
        }

        #endregion

        #region Ensure user has enroll permission on the requested certificate template

        if (!certificateTemplate.AllowsForEnrollment(enrollee))
        {
            Marshal.ReleaseComObject(certificateRequestPkcs10);
            throw Deny(
                $"Identity {enrollee.Name} is not permitted to enroll for the requested certificate template.",
                WinError.CERTSRV_E_TEMPLATE_DENIED);
        }

        #endregion

        #region Ensure the incoming certificate request is empty

        if (certificateRequestPkcs10.GetSubjectDistinguishedName() != string.Empty)
        {
            Marshal.ReleaseComObject(certificateRequestPkcs10);
            throw Deny("Subject Distinguished Name of the certificate request is not empty.",
                WinError.CERT_E_INVALID_NAME);
        }

        var extensionList = certificateRequestPkcs10.GetRequestExtensions();

        if (extensionList.ContainsKey(WinCrypt.szOID_SUBJECT_ALT_NAME2) ||
            extensionList.ContainsKey(WinCrypt.szOID_DS_CA_SECURITY_EXT))
        {
            Marshal.ReleaseComObject(certificateRequestPkcs10);
            throw Deny(
                "The certificate request contains a Subject Alternative Name or Security Identifier certificate extension.",
                WinError.CERT_E_INVALID_NAME);
        }

        #endregion

        #region Look up enrollee identity in Active Directory

        var searchResult = GetDirectoryEntry(enrollee.User);
        var isUser = !searchResult.Properties["objectClass"].Contains("computer");
        var identity = isUser
            ? (string)searchResult.Properties["userPrincipalName"][0]
            : (string)searchResult.Properties["dNSHostName"][0];

        #endregion

        #region Add CSR and identity to new signed CMC layer

        var result = GetSignedCertificateRequest(certificateRequestPkcs10, enrollee.User, identity, isUser);

        #endregion

        #region Build request attribute list

        attributeList = requestSecurityToken.AdditionalContext.ToList()
            .Select(item => $"wstep{item.Key}:{item.Value}").ToList();

        attributeList.Add(isUser
            ? $"CN:{(string)searchResult.Properties["cn"][0]}"
            : $"CN:{(string)searchResult.Properties["dNSHostName"][0]}");

        #endregion

        Marshal.ReleaseComObject(certificateRequestPkcs10);

        return result;
    }

    private string GetSignedCertificateRequest(IX509CertificateRequest request, SecurityIdentifier sid,
        string identity, bool isUser)
    {
        #region Build SAN extension

        var sanItem = new CAlternativeName();
        sanItem.InitializeFromString(
            isUser
                ? AlternativeNameType.XCN_CERT_ALT_NAME_USER_PRINCIPLE_NAME
                : AlternativeNameType.XCN_CERT_ALT_NAME_DNS_NAME,
            identity);

        var sanCollection = new CAlternativeNames { sanItem };
        var sanExtension = new CX509ExtensionAlternativeNames();
        sanExtension.InitializeEncode(sanCollection);

        #endregion

        #region Build SID extension

        var sidExtension = new CX509Extension();
        var sidExtensionOid = new CObjectId();
        sidExtensionOid.InitializeFromValue(WinCrypt.szOID_DS_CA_SECURITY_EXT);
        sidExtension.Initialize(sidExtensionOid, EncodingType.XCN_CRYPT_STRING_BASE64,
            Convert.ToBase64String(new X509CertificateExtensionSecurityIdentifier(sid).RawData));

        #endregion

        #region Build signed CMC layer

        var cmcLayer = new CX509CertificateRequestCmc();
        cmcLayer.InitializeFromInnerRequest(request);
        cmcLayer.HashAlgorithm = _signatureHashAlgorithm;
        cmcLayer.SignerCertificate = _signerCertificate;
        cmcLayer.X509Extensions.Add(sanExtension as CX509Extension);
        cmcLayer.X509Extensions.Add(sidExtension);
        cmcLayer.Encode();

        #endregion

        var result = cmcLayer.get_RawData();

        Marshal.ReleaseComObject(sanItem);
        Marshal.ReleaseComObject(sanCollection);
        Marshal.ReleaseComObject(sanExtension);
        Marshal.ReleaseComObject(sidExtension);
        Marshal.ReleaseComObject(sidExtensionOid);
        Marshal.ReleaseComObject(cmcLayer);
        Marshal.ReleaseComObject(request);

        return result;
    }

    private SearchResult GetDirectoryEntry(IdentityReference sid)
    {
        var searcher = new DirectorySearcher
        {
            SearchRoot = new DirectoryEntry($"GC://{_forestRootDomain}"),
            Filter = $"(objectSid={sid.Value})",
            PageSize = 1,
            PropertiesToLoad = { "objectClass", "cn", "userPrincipalName", "dNSHostName" },
            ClientTimeout = new TimeSpan(0, 0, 15)
        };

        var result = searcher.FindOne() ?? throw new ActiveDirectoryObjectNotFoundException(
            string.Format("Account {0} was not found in Active Directory.", sid.Value));

        var key = result.Properties["objectClass"].Contains("computer") ? "dNSHostName" : "userPrincipalName";

        if (!result.Properties.Contains(key))
        {
            throw Deny(string.Format("Account {0} ({1}) does not have property \"{2}\" populated.",
                (string)result.Properties["cn"][0], sid.Value, key), WinError.CERT_E_INVALID_NAME);
        }

        return result;
    }

    private static X509Certificate2 GetSigningCertificate(string caName, string eku)
    {
        var store = new X509Store("MY", StoreLocation.LocalMachine);

        store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

        return store.Certificates
            .Find(X509FindType.FindByIssuerName, caName, true)
            .Find(X509FindType.FindByApplicationPolicy, eku, false)
            .Find(X509FindType.FindByKeyUsage, 0x80, false)
            .Where(certificate => certificate.HasPrivateKey)
            .OrderByDescending(certificate => certificate.NotAfter)
            .FirstOrDefault();
    }

    private static FaultException Deny(string reasonText, int reasonCode = WinError.NTE_FAIL)
    {
        return new FaultException<CertificateEnrollmentWsDetailType>(
            new CertificateEnrollmentWsDetailType(0, reasonCode, string.Empty),
            new FaultReason(new FaultReasonText(reasonText)),
            new FaultCode("Receiver", "http://www.w3.org/2003/05/soap-envelope"),
            "http://schemas.microsoft.com/windows/pki/2009/01/enrollment/RequestSecurityTokenCertificateEnrollmentWSDetailFault");
    }
}