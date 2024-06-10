// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_XCEP;

/// <summary>
///     The GetPolicies operation defines the client request and server response messages that are used to complete the act
///     of retrieving a certificate enrollment policy.
/// </summary>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy")]
[XmlRoot(ElementName = "GetPolicies",
    Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy",
    IsNullable = false)]
public class GetPoliciesType
{
    private ClientType _clientTypeField;

    private RequestFilterType _requestFilterTypeField;

    /// <summary>
    ///     The xcep:client element is an instance of the Client object as defined in section 3.1.4.1.3.9. The xcep:client
    ///     element contains information about the caller including the caller's preferred language, and the date and time of
    ///     last policy retrieval. If the xcep:client element is absent, is specified as nil, or has no value, the server MUST
    ///     respond with a SOAP fault.
    /// </summary>
    [XmlElement(ElementName = "client")]
    public ClientType ClientType
    {
        get => _clientTypeField;
        set => _clientTypeField = value;
    }

    /// <summary>
    ///     The xcep:requestFilter element is an instance of the RequestFilter object as defined in section 3.1.4.1.3.22. The
    ///     xcep:requestFilter element specified in the request is used to constrain the policy request to specific policies.
    ///     If the xcep:requestFilter element is empty or specified as nil, the server MUST NOT apply any filters to the
    ///     response.
    /// </summary>
    [XmlElement(ElementName = "requestFilter")]
    public RequestFilterType RequestFilterType
    {
        get => _requestFilterTypeField;
        set => _requestFilterTypeField = value;
    }

    public static GetPoliciesType FromMessage(Message message)
    {
        if (message == null)
        {
            throw new ArgumentNullException();
        }

        var serializer = new XmlSerializer(typeof(GetPoliciesType));

        using (var reader = (XmlReader)message.GetReaderAtBodyContents())
        {
            return (GetPoliciesType)serializer.Deserialize(reader);
        }
    }
}