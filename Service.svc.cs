// Copyright © Uwe Gradenegger <uwe@gradenegger.eu>

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Specialized;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web.Configuration;
using TameMyCerts.WSTEP.Connectors;
using TameMyCerts.WSTEP.Models;

namespace TameMyCerts.WSTEP
{
    public class SecurityTokenService : ISecurityTokenService
    {
        private readonly TameMyCertsRestConnector _connector;
        private readonly Gatekeeper _gatekeeper;
        private readonly Logger _logger = new Logger();

        public SecurityTokenService()
        {
            try
            {
                _connector = new TameMyCertsRestConnector(ConfigurationManager.AppSettings["CAName"],
                    ConfigurationManager.AppSettings["ApiAddress"], ConfigurationManager.AppSettings["ApiUser"],
                    GetEncryptedApiPassword(),
                    Convert.ToBoolean(ConfigurationManager.AppSettings["ApiCertificateRevocationCheck"]));
                _gatekeeper = new Gatekeeper(ConfigurationManager.AppSettings["SignerCertificateExtendedKeyUsage"]);
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                throw;
            }
        }

        public Message RequestSecurityToken(Message message)
        {
            try
            {
                var signedRequest = _gatekeeper.VerifyRequest(
                    RequestSecurityTokenType.FromMessage(message),
                    ServiceSecurityContext.Current.WindowsIdentity);

                var response = _connector.SubmitRequest(signedRequest);

                return response.ToMessage(message.Headers.MessageId);
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                throw;
            }
        }

        private static string GetEncryptedApiPassword()
        {
            if (WebConfigurationManager.GetSection("secureAppSettings") is NameValueCollection section &&
                section["ApiPassword"] != null)
            {
                return section["ApiPassword"];
            }

            throw new ArgumentException("Unable to retrieve API password from configuration.");
        }
    }
}