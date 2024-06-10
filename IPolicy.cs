// Copyright (c) Uwe Gradenegger <info@gradenegger.eu>

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using TameMyCerts.WSTEP.Models.MS_XCEP;

namespace TameMyCerts.WSTEP;

[ServiceContract(Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy")]
public interface IPolicy
{
    [OperationContract(
        Action = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy/IPolicy/GetPolicies",
        ReplyAction = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy/IPolicy/GetPoliciesResponse",
        Name = "GetPoliciesResponse")]
    Message GetPolicies(Message message);
}

public class Policy : IPolicy
{
    public Message GetPolicies(Message message)
    {
        // just to ensure the input is valid
        _ = GetPoliciesType.FromMessage(message);

        var response = new GetPoliciesResponseType
        {
            Response = new ResponseType
            {
                PolicyId = "{47177453-8EE0-4739-9B73-F40626A42D51}",
                CertificateEnrollmentPolicyCollection = new List<CertificateEnrollmentPolicyType>
                {
                    new()
                    {
                        PolicyOidReference = 0,
                        CaReferenceCollection = new List<int> { 0 },
                        Attributes = new AttributesType
                        {
                            CommonName = "User_Online",
                            PolicySchema = 2,
                            CertificateValidity = new CertificateValidityType
                            {
                                ValidityPeriodSeconds = 31536000,
                                RenewalPeriodSeconds = 3628800
                            },
                            Permission = new EnrollmentPermissionType
                            {
                                Enroll = true,
                                AutoEnroll = true
                            },
                            PrivateKeyAttributes = new PrivateKeyAttributesType
                            {
                                MinimalKeyLength = 2048,
                                KeySpec = 1,
                                CryptoProviders = new List<string>
                                {
                                    "Microsoft Enhanced Cryptographic Provider v1.0",
                                    "Microsoft Base Cryptographic Provider v1.0"
                                }
                            },
                            Revision = new RevisionType
                            {
                                MajorRevision = 100,
                                MinorRevision = 8
                            },
                            PrivateKeyFlags = 50528256,
                            SubjectNameFlags = 1107296256,
                            EnrollmentFlags = 0,
                            GeneralFlags = 131642,
                            Extensions = new List<ExtensionType>
                            {
                                new()
                                {
                                    OIdReference = 2,
                                    Value = "MC8GJysGAQQBgjcVCITLq2yCk8xwhamPMobR/COC8YBmgVuBmL0JhYaHHgIBZAIBCA=="
                                },
                                new()
                                {
                                    OIdReference = 3,
                                    Value = "MAoGCCsGAQUFBwMC"
                                },
                                new()
                                {
                                    OIdReference = 4,
                                    Critical = true,
                                    Value = "AwIFoA=="
                                },
                                new()
                                {
                                    OIdReference = 5,
                                    Value = "MAwwCgYIKwYBBQUHAwI="
                                }
                            }
                        }
                    }
                }
            },
            CaCollection = new List<CaType>
            {
                new()
                {
                    Uris = new List<CaUriType>
                    {
                        new()
                        {
                            ClientAuthentication = 2,
                            Uri = "https://fbweb02.forest-b.local/wstep/Service.svc/CES",
                            Priority = 1,
                        }
                    },
                    Certificate =
                        "MIIFYzCCA0ugAwIBAgIQHLPS2yhxVYFO1BnhmncQSDANBgkqhkiG9w0BAQsFADBDMRUwEwYKCZImiZPyLGQBGRYFbG9jYWwxGDAWBgoJkiaJk/IsZAEZFghmb3Jlc3QtYTEQMA4GA1UEAxMHVEVTVC1DQTAgFw0yMzExMzAwODU3MzZaGA8yMDczMTEzMDA5MDczNVowQzEVMBMGCgmSJomT8ixkARkWBWxvY2FsMRgwFgYKCZImiZPyLGQBGRYIZm9yZXN0LWExEDAOBgNVBAMTB1RFU1QtQ0EwggIiMA0GCSqGSIb3DQEBAQUAA4ICDwAwggIKAoICAQDIEqS/koEJiQvQMUbPICW0Psy/TGUpWo4uEgsnX3W6AVLgj1zU5uFqLJyrLmFfg6PZ7J3wIG3RNonnuQfc3hm8FDdqQRvlczIzedfakPB5nrmjGqhxB3GlScXId96+yZpFe+WY2wCHEhfIEp4MeJmRcVtDl4d2xlnh5tQkaD0JaMJISqUXPxjctXOBf2cMJUX/RrYA6j3lpfcvt/j7GPu5Gsb2tZPYyyFhihBM608dUAx6BcAXgVUZ7WFusNzCiE/lwpTnkXKz1ZXWPMmmNTTbIqhS2QxunMEH3TRq6dhJqJtiZuXNOUCO1pm3mxxToA96egVKPvZPKLA33K62GTz6Sh4FraLg298Seh56jL2tn+UeZw8B7Y3SPuEUVpTH1pHN7oICaC+RzzUrpjI2qs8BTgR2KDHLwTDJkVu+3edfor4QUC7p5QPe4tNNQLTva+A4O8CTCakjc/0Gx76n0g8nF5qpOOS+k2eaVaRASVuZXVcpSV6Jtpq69xDPzOQrIDXOyPdxYZD2NmyVAMRpBlrdyKQsUBdDqLpgvOgQOVT4q5ALGo1qQYpLhndkgRW/eE/DdpPHcMVUTQyugPYV8s8O1arrtEH0ESJHfBwH75TQsPAltB3bCi/USSlO3e4MoqksKk9vjSbls+JgeHnWZgOJ5JUAdqJ3vP/rbIO/Z/cx2QIDAQABo1EwTzALBgNVHQ8EBAMCAYYwDwYDVR0TAQH/BAUwAwEB/zAdBgNVHQ4EFgQURjXMNEja3k5gy+rbggjjB+AfmUIwEAYJKwYBBAGCNxUBBAMCAQAwDQYJKoZIhvcNAQELBQADggIBAMBUv/eQVJ81k/atGdq+NSwib2Zmwa/SJ4NaYvPIkKWGkr7XIIU6ANYEyC050d5sT+AWbRRe7xkMJRQc4yfDdZiTCmaPdVLd5jMGteK0g4xkhVieVcCSjNq75jXDQaoEe8v9vYcy8TVWRYEHbnxyGRejWs8RiF+bMnWzKftbcwdAaqjed+RJ2Cw+QUJtSkuz1VS5wDwE8+Uu6mzEeEGa1z4cf+qO6Y0kPIB2clPW78a+nGTe6jQzEM5+m5OxPuq4yWjXoilRrnW6iOcfbTEcJoTQvg/6/DdLAtkeKFIAtUjflo8v3cv37IsQDyEtNuLO3GcnRSTIr5FIeYq5tZ+qZU1NDvf+CkzuH0gq7EniON5KSv7vOQZ/9d1DtITJ7ubxYLbiBIOpfwEQS5985iJyUdAoGcX7D3UV59GXKH4Ec/MgQa0U7ZIxH7sYGkEZZn7DdgqPy7H400M0rNfttMdFoDH00L6KnNL9SUU2kR/I1H4x4Ees+DEpQJ3hHywwZcz+YE+olDTtf3U1umEnihDPMR7ai2oj/zHDscM/gL5LiqbUQjf3nfK1DSJR8Vs18k1ENyRyqZxyoJcHIOJVlc5d1+vxkJVxlFJwHDBUhZudVLeO0RKpZCtt2IFJDackSrkl1x1UbSerzu7h0L2jF+JHkXtJQMJxbEy3kK6sRF1IdzXC",
                    EnrollPermission = true,
                    CaReferenceId = 0
                }
            },
            OidCollection = new List<OidType>
            {
                new()
                {
                    Value = "1.3.6.1.4.1.311.21.8.9623020.4515440.11159474.13925923.6045798.219.2498185.10584990",
                    Group = 9,
                    OIdReferenceId = 0,
                    DefaultName = "User_Online"
                },
                new()
                {
                    Value = "1.3.6.1.5.5.7.3.2",
                    Group = 7,
                    OIdReferenceId = 1,
                    DefaultName = "Client Authentication"
                },
                new()
                {
                    Value = "1.3.6.1.4.1.311.21.7",
                    Group = 6,
                    OIdReferenceId = 2,
                    DefaultName = "Certificate Template Information"
                },
                new()
                {
                    Value = "2.5.29.37",
                    Group = 6,
                    OIdReferenceId = 3,
                    DefaultName = "Enhanced Key Usage"
                },
                new()
                {
                    Value = "2.5.29.15",
                    Group = 6,
                    OIdReferenceId = 4,
                    DefaultName = "Key Usage"
                },
                new()
                {
                    Value = "1.3.6.1.4.1.311.21.10",
                    Group = 6,
                    OIdReferenceId = 5,
                    DefaultName = "Application Policies"
                }
            }
        };

        return response.ToMessage();
    }
}