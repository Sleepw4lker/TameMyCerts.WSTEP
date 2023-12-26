using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models;

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://docs.oasis-open.org/ws-sx/ws-trust/200512")]
[XmlRoot(ElementName = "RequestSecurityToken",
    Namespace = "http://docs.oasis-open.org/ws-sx/ws-trust/200512",
    IsNullable = false)]
public class RequestSecurityTokenType
{
    private AdditionalContextType _additionalContextField;

    private BinarySecurityTokenType _binarySecurityTokenField;

    private string _preferredLanguageField;

    private string _requestTypeField;

    private string _tokenTypeField;

    [XmlElement(ElementName = "TokenType")]
    public string TokenType
    {
        get => _tokenTypeField;
        set => _tokenTypeField = value;
    }

    [XmlElement(ElementName = "RequestType")]
    public string RequestType
    {
        get => _requestTypeField;
        set => _requestTypeField = value;
    }

    [XmlElement(ElementName = "BinarySecurityToken",
        Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
    public BinarySecurityTokenType BinarySecurityToken
    {
        get => _binarySecurityTokenField;
        set => _binarySecurityTokenField = value;
    }

    [XmlElement(ElementName = "AdditionalContext",
        Namespace = "http://schemas.xmlsoap.org/ws/2006/12/authorization")]
    public AdditionalContextType AdditionalContext
    {
        get => _additionalContextField;
        set => _additionalContextField = value;
    }

    [XmlAttribute(AttributeName = "PreferredLanguage")]
    public string PreferredLanguage
    {
        get => _preferredLanguageField;
        set => _preferredLanguageField = value;
    }

    public static RequestSecurityTokenType FromMessage(Message message)
    {
        if (message == null)
        {
            throw new ArgumentNullException();
        }

        var serializer = new XmlSerializer(typeof(RequestSecurityTokenType));

        using (var reader = (XmlReader)message.GetReaderAtBodyContents())
        {
            return (RequestSecurityTokenType)serializer.Deserialize(reader);
        }
    }
}