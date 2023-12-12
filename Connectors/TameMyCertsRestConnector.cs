﻿// Copyright © Uwe Gradenegger <uwe@gradenegger.eu>

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
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TameMyCerts.WSTEP.Models;

namespace TameMyCerts.WSTEP.Connectors
{
    internal class TameMyCertsRestConnector
    {
        private readonly string _caName;

        /// <summary>
        ///     HttpClient is intended to be instantiated once and reused throughout the life of an application.
        /// </summary>
        /// <ref>https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=netframework-4.7.2</ref>
        private readonly HttpClient _httpClient;

        public TameMyCertsRestConnector(string caName, string baseAddress, string userName, string password,
            bool checkCertificateRevocationList = true)
        {
            _caName = caName;

            var handler = new HttpClientHandler
            {
                CheckCertificateRevocationList = checkCertificateRevocationList
            };

            // TODO: Can we abstract the base address to include the app-specific subdirectory?
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseAddress)
            };

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}")));
        }

        private async Task<SubmissionResponse> SubmitRequestAsync(string certificateRequest)
        {
            var request = new CertificateRequest
            {
                Request = certificateRequest
            };

            var response = await _httpClient.PostAsJsonAsync($"v1/certificates/{_caName}", request);

            // TODO: Handle the case when the API returns a HTTP failure

            return await response.Content.ReadAsAsync<SubmissionResponse>();
        }

        public RequestSecurityTokenResponseCollectionType SubmitRequest(string certificateRequest)
        {
            // TODO: Is this the best approach?
            // https://stackoverflow.com/questions/40324300/calling-async-methods-from-non-async-code
            return ProcessResponse(SubmitRequestAsync(certificateRequest).GetAwaiter().GetResult());
        }

        private static RequestSecurityTokenResponseCollectionType ProcessResponse(SubmissionResponse response)
        {
            switch (response.Disposition)
            {
                case "issued":

                    return new RequestSecurityTokenResponseCollectionType(response.Disposition,
                        response.RequestId, response.Certificate, response.BinaryResponse);

                case "denied":

                    throw new FaultException<CertificateEnrollmentWsDetailType>(
                        new CertificateEnrollmentWsDetailType(response.RequestId, response.Status.StatusCode,
                            response.BinaryResponse),
                        new FaultReason(new FaultReasonText("Denied by Policy Module")),
                        new FaultCode("Receiver", "http://www.w3.org/2003/05/soap-envelope"),
                        "http://schemas.microsoft.com/windows/pki/2009/01/enrollment/RequestSecurityTokenCertificateEnrollmentWSDetailFault");

                default:

                    // We do not support and other state
                    return new RequestSecurityTokenResponseCollectionType(response.Disposition,
                        response.RequestId, string.Empty, response.BinaryResponse);
            }
        }

        internal class CertificateRequest
        {
            public string Request { get; set; }
            public List<string> RequestAttributes { get; set; } = new List<string>();
        }

        internal class SubmissionResponse
        {
            public int RequestId { get; set; }

            public string Disposition { get; set; }


            public Status Status { get; set; }

            public string Certificate { get; set; }
            public string CertificateChain { get; set; }
            public string BinaryResponse { get; set; }
        }

        internal class Status
        {
            public int StatusCode { get; set; }

            public string Description { get; set; }
        }
    }
}