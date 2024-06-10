// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;

namespace TameMyCerts.WSTEP.Models.MS_WSTEP;

// You cannot use the XmlSerializer class to describe faults. The XmlSerializerFormatAttribute has no effect on fault contracts.
// https://learn.microsoft.com/en-us/dotnet/framework/wcf/feature-details/specifying-data-transfer-in-service-contracts
[Serializable]
[DesignerCategory("code")]
[DataContract(Name = "CertificateEnrollmentWSDetail",
    Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollment")]
public class CertificateEnrollmentWsDetailType
{
    private string _binaryResponseField;

    private int _errorCodeField;

    private bool _invalidRequestField;

    private int _requestIdField;

    public CertificateEnrollmentWsDetailType()
    {
    }

    public CertificateEnrollmentWsDetailType(int requestId, int errorCode, string binaryResponse)
    {
        _binaryResponseField = binaryResponse;
        _errorCodeField = errorCode;
        _requestIdField = requestId;
        _invalidRequestField = true;
    }

    [DataMember(Name = "BinaryResponse")]
    public string BinaryResponse
    {
        get => _binaryResponseField;
        set => _binaryResponseField = value;
    }

    [DataMember(Name = "ErrorCode")]
    public int ErrorCode
    {
        get => _errorCodeField;
        set => _errorCodeField = value;
    }

    [DataMember(Name = "InvalidRequest")]
    public bool InvalidRequest
    {
        get => _invalidRequestField;
        set => _invalidRequestField = value;
    }

    [DataMember(Name = "RequestID")]
    public int RequestId
    {
        get => _requestIdField;
        set => _requestIdField = value;
    }
}