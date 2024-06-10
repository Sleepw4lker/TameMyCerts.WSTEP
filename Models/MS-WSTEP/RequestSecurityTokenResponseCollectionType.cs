// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_WSTEP;

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://docs.oasis-open.org/ws-sx/ws-trust/200512")]
[XmlRoot(ElementName = "RequestSecurityTokenResponseCollection",
    Namespace = "http://docs.oasis-open.org/ws-sx/ws-trust/200512",
    IsNullable = false)]
public class RequestSecurityTokenResponseCollectionType
{
    private RequestSecurityTokenResponseType _requestSecurityTokenResponseField;

    public RequestSecurityTokenResponseCollectionType()
    {
    }

    public RequestSecurityTokenResponseCollectionType(string dispositionMessage, int requestId, string certificate,
        string fullResponse)
    {
        RequestSecurityTokenResponse = new RequestSecurityTokenResponseType
        {
            TokenType = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3",
            DispositionMessage = new DispositionMessageType
            {
                Lang = "en-US",
                Value = dispositionMessage
            },
            BinarySecurityToken = new BinarySecurityTokenType
            {
                ValueType =
                    "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd#PKCS7",
                EncodingType =
                    "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd#base64binary",
                Value = fullResponse
            },
            RequestedSecurityToken = new RequestedSecurityTokenType
            {
                BinarySecurityToken = new BinarySecurityTokenType
                {
                    ValueType =
                        "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3",
                    EncodingType =
                        "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd#base64binary",
                    Value = certificate
                }
            },
            RequestId = requestId
        };
    }

    public RequestSecurityTokenResponseCollectionType(int requestId, string message)
    {
        RequestSecurityTokenResponse = new RequestSecurityTokenResponseType
        {
            TokenType = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3",
            DispositionMessage = new DispositionMessageType
            {
                Lang = "en-US",
                Value = message
            },
            RequestId = requestId
        };
    }

    public RequestSecurityTokenResponseType RequestSecurityTokenResponse
    {
        get => _requestSecurityTokenResponseField;
        set => _requestSecurityTokenResponseField = value;
    }

    public Message ToMessage(UniqueId relatesTo = null)
    {
        // https://weblogs.asp.net/rternier/sending-xml-data-through-wcf-using-system-servicemodel-channels-message-without-using-datacontractserializer
        var serializer = new XmlSerializer(GetType());
        var stringWriter = new StringWriter();
        serializer.Serialize(stringWriter, this);

        var reader = XmlReader.Create(new StringReader(stringWriter.ToString()));

        var message = Message.CreateMessage(MessageVersion.Soap12WSAddressing10,
            "http://schemas.microsoft.com/windows/pki/2009/01/enrollment/RSTRC/wstep",
            reader);

        if (relatesTo != null)
        {
            message.Headers.RelatesTo = relatesTo;
        }

        return message;
    }
}