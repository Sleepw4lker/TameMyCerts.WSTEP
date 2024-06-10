// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_WSTEP;

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://docs.oasis-open.org/ws-sx/ws-trust/200512")]
[XmlRoot(ElementName = "RequestSecurityTokenResponse")]
public class RequestSecurityTokenResponseType
{
    private BinarySecurityTokenType _binarySecurityTokenField;

    private DispositionMessageType _dispositionMessageField;

    private RequestedSecurityTokenType _requestedSecurityTokenField;

    private int _requestIdField;

    private string _tokenTypeField;

    [XmlElement(ElementName = "TokenType")]
    public string TokenType
    {
        get => _tokenTypeField;
        set => _tokenTypeField = value;
    }

    [XmlElement(ElementName = "DispositionMessage",
        Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollment")]
    public DispositionMessageType DispositionMessage
    {
        get => _dispositionMessageField;
        set => _dispositionMessageField = value;
    }

    [XmlElement(ElementName = "BinarySecurityToken", Namespace =
        "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
    public BinarySecurityTokenType BinarySecurityToken
    {
        get => _binarySecurityTokenField;
        set => _binarySecurityTokenField = value;
    }

    [XmlElement(ElementName = "RequestedSecurityToken")]
    public RequestedSecurityTokenType RequestedSecurityToken
    {
        get => _requestedSecurityTokenField;
        set => _requestedSecurityTokenField = value;
    }

    [XmlElement(ElementName = "RequestID",
        Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollment")]
    public int RequestId
    {
        get => _requestIdField;
        set => _requestIdField = value;
    }
}