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

using System.Security.Cryptography;
using System.Text;
using TameMyCerts.WSTEP.Connectors;
using TameMyCerts.WSTEP.Models;

namespace TameMyCerts.WSTEP;

[ServiceContract(Namespace = "http://docs.oasis-open.org/ws-sx/ws-trust/200512")]
public interface ISecurityTokenService
{
    [OperationContract(
        Action = "http://schemas.microsoft.com/windows/pki/2009/01/enrollment/RST/wstep",
        ReplyAction = "http://schemas.microsoft.com/windows/pki/2009/01/enrollment/RSTRC/wstep",
        Name = "RequestSecurityTokenType")]
    Message RequestSecurityToken(Message message);
}

public class SecurityTokenService : ISecurityTokenService
{
    private readonly IConfiguration _configuration;
    private readonly TameMyCertsRestConnector _connector;

    private readonly Gatekeeper _gatekeeper;

    public SecurityTokenService(IConfiguration configuration)
    {
        _configuration = configuration;

        _connector = new TameMyCertsRestConnector(
            _configuration["CAName"],
            _configuration["ApiAddress"],
            _configuration["ApiUser"],
            GetApiPassword(_configuration["ApiPassword"]),
            Convert.ToBoolean(_configuration["ApiCertificateRevocationCheck"]));

        _gatekeeper = new Gatekeeper(
            _configuration["CAName"],
            _configuration["SignerCertificateExtendedKeyUsage"]);
    }

    public Message RequestSecurityToken(Message message)
    {
        var signedRequest = _gatekeeper.VerifyRequest(
            RequestSecurityTokenType.FromMessage(message),
            ServiceSecurityContext.Current.WindowsIdentity, out var attributeList);

        var response = _connector.SubmitRequest(signedRequest, attributeList);

        return response.ToMessage(message.Headers.MessageId);
    }

    private static string GetApiPassword(string encryptedPassword)
    {
        ArgumentException.ThrowIfNullOrEmpty(encryptedPassword);

        var encryptedBytes = Convert.FromBase64String(encryptedPassword);
        var decryptedBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.LocalMachine);

        return Encoding.UTF8.GetString(decryptedBytes);
    }
}