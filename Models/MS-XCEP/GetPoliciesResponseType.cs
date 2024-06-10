// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_XCEP;

/// <summary>
///     GetPoliciesResponse is a response message. It is the message sent from the server to the client containing the
///     requested certificate enrollment policy.
/// </summary>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy")]
[XmlRoot(ElementName = "GetPoliciesResponse",
    Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy",
    IsNullable = false)]
public class GetPoliciesResponseType
{
    private List<CaType> _cACollectionField = new();

    private List<OidType> _oIdCollectionField = new();

    private ResponseType _responseField;

    public GetPoliciesResponseType()
    {
    }

    public GetPoliciesResponseType(List<CaType> cACollectionField, List<OidType> oIdCollectionField,
        ResponseType responseField)
    {
        _cACollectionField = cACollectionField;
        _oIdCollectionField = oIdCollectionField;
        _responseField = responseField;
    }


    /// <summary>
    ///     The xcep:response: element is an instance of the Response object as defined in section 3.1.4.1.3.23 that contains
    ///     the certificate enrollment policies.
    /// </summary>
    [XmlElement(ElementName = "response")]
    public ResponseType Response
    {
        get => _responseField;
        set => _responseField = value;
    }

    /// <summary>
    ///     The xcep:cAs element is an instance of a CACollection object as defined in section 3.1.4.1.3.3 that contains the
    ///     issuers for the certificate enrollment policies.
    /// </summary>
    [XmlArray(ElementName = "cAs", IsNullable = false)]
    [XmlArrayItem(ElementName = "cA")]
    public List<CaType> CaCollection
    {
        get => _cACollectionField;
        set => _cACollectionField = value;
    }

    /// <summary>
    ///     The xcep:oIDs element is an instance of the OIDCollection object as defined in section 3.1.4.1.3.17 that contains
    ///     the collection of OIDs for the response.
    /// </summary>
    [XmlArray(ElementName = "oIDs")]
    [XmlArrayItem("oID", IsNullable = false)]
    public List<OidType> OidCollection
    {
        get => _oIdCollectionField;
        set => _oIdCollectionField = value;
    }

    public Message ToMessage(UniqueId? relatesTo = null)
    {
        // https://weblogs.asp.net/rternier/sending-xml-data-through-wcf-using-system-servicemodel-channels-message-without-using-datacontractserializer
        var serializer = new XmlSerializer(GetType());
        var stringWriter = new StringWriter();
        serializer.Serialize(stringWriter, this);

        var reader = XmlReader.Create(new StringReader(stringWriter.ToString()));

        var message = Message.CreateMessage(MessageVersion.Soap12WSAddressing10,
            "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy/IPolicy/GetPoliciesResponse",
            reader);

        if (relatesTo != null)
        {
            message.Headers.RelatesTo = relatesTo;
        }

        return message;
    }
}