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

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using TameMyCerts.NetCore.Common.Enums;
using TameMyCerts.WSTEP.Models;

namespace TameMyCerts.WSTEP.Connectors;

internal class TameMyCertsRestConnector
{
    private readonly string _caName;
    private readonly HttpClient _httpClient;
    private readonly string _myHostName = Dns.GetHostEntry(string.Empty).HostName;

    public TameMyCertsRestConnector(string caName, string baseAddress, string userName, string password,
        bool checkCertificateRevocationList = true)
    {
        _caName = caName;

        var handler = new HttpClientHandler
        {
            CheckCertificateRevocationList = checkCertificateRevocationList
        };

        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseAddress.EndsWith('/') ? baseAddress : $"{baseAddress}/")
        };

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}")));
    }

    private async Task<SubmissionResponse> SubmitRequestAsync(string certificateRequest, List<string> attributeList)
    {
        attributeList.Add($"wstepproxy:{_myHostName}");

        var request = new CertificateRequest
        {
            Request = certificateRequest,
            RequestAttributes = attributeList
        };

        var response = await _httpClient.PostAsJsonAsync($"v1/certificates/{_caName}", request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Got HTTP Status Code {(int)response.StatusCode} ({response.StatusCode}) from {_httpClient.BaseAddress}.");
        }

        return await response.Content.ReadAsAsync<SubmissionResponse>();
    }

    public RequestSecurityTokenResponseCollectionType SubmitRequest(string certificateRequest,
        List<string> attributeList)
    {
        // TODO: Is this the best approach?
        // https://stackoverflow.com/questions/40324300/calling-async-methods-from-non-async-code
        return ProcessResponse(SubmitRequestAsync(certificateRequest, attributeList).GetAwaiter().GetResult());
    }

    private static RequestSecurityTokenResponseCollectionType ProcessResponse(SubmissionResponse response)
    {
        switch (response.Disposition.ToLower())
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
                throw new FaultException<CertificateEnrollmentWsDetailType>(
                    new CertificateEnrollmentWsDetailType(response.RequestId, WinError.NTE_FAIL, string.Empty),
                    new FaultReason(new FaultReasonText($"{response.Disposition.ToLower()} result is not supported.")),
                    new FaultCode("Receiver", "http://www.w3.org/2003/05/soap-envelope"),
                    "http://schemas.microsoft.com/windows/pki/2009/01/enrollment/RequestSecurityTokenCertificateEnrollmentWSDetailFault");
        }
    }

    internal class CertificateRequest
    {
        public string Request { get; set; }
        public List<string> RequestAttributes { get; set; } = new();
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