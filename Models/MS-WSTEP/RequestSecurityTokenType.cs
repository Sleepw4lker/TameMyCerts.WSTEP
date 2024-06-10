// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_WSTEP;

/// <summary>
///     The wst:RequestSecurityTokenType complex type contains the elements for the security token request in the
///     RequestSecurityTokenMsg message. It is the client-provided object for a certificate enrollment request.
///     wst:RequestSecurityTokenType is defined in the WS-Trust [WSTrust1.3] XML schema definition (XSD).
/// </summary>
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

    /// <summary>
    ///     For the X.509v3 enrollment extension to WS-Trust, the wst:tokentype element MUST be
    ///     http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3.
    /// </summary>
    [XmlElement(ElementName = "TokenType")]
    public string TokenType
    {
        get => _tokenTypeField;
        set => _tokenTypeField = value;
    }

    /// <summary>
    ///     An instance of a wst:RequestTypeOpenEnum object as defined in [WSTrust1.3] XML schema definition (XSD).
    /// </summary>
    [XmlElement(ElementName = "RequestType")]
    public string RequestType
    {
        get => _requestTypeField;
        set => _requestTypeField = value;
    }

    /// <summary>
    ///     Provides the DER ASN.1 representation of the certificate request. The type of token is defined by the wst:TokenType
    ///     element. For the X.509v3 enrollment extension the wst:TokenType MUST be specified as in section 3.1.4.1.2.8. The
    ///     certificate request follows the formatting from [MS-WCCE] section 2.2.2.6. The EncodingType attribute of the
    ///     wsse:BinarySecurityToken element MUST be set to base64Binary.
    /// </summary>
    [XmlElement(ElementName = "BinarySecurityToken",
        Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
    public BinarySecurityTokenType BinarySecurityToken
    {
        get => _binarySecurityTokenField;
        set => _binarySecurityTokenField = value;
    }

    /// <summary>
    ///     The auth:AdditionalContext element is used to provide extra information in a wst:RequestSecurityToken message. It
    ///     is an optional element, and SHOULD be omitted if there is no extra information to be passed.
    /// </summary>
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