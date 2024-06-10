// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_XCEP;

/// <summary>
///     The Client complex type contains information about the client's current state and preferences.
/// </summary>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy")]
[XmlRoot(ElementName = "client",
    Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy",
    IsNullable = false)]
public class ClientType
{
    private DateTime? _lastUpdateField;

    private string _preferredLanguageField;

    /// <summary>
    ///     The lastUpdate element is an xs:dateTime value that represents the last time that the client retrieved policies
    ///     from a policy server with this Response object's policyServerID, as defined in section 3.1.4.1.3.23. The lastUpdate
    ///     element is provided as GMT. If the client does not include the lastUpdate element or the element is nil, the server
    ///     MUST consider the value to be older than the initialized value of the LastUpdateTime data element. The server
    ///     SHOULD provide a full GetPoliciesResponse message if the client's lastUpdate time is older than the time of the
    ///     server's last update to the policy. If the lastUpdate time provided by the client is equal to or newer than the
    ///     lastUpdate time of the server, the server SHOULD respond with a GetPoliciesResponse message in which the oIDs and
    ///     cAs elements are set to nil, as defined in 3.1.4.1.1.2, and the policiesNotChanged element in the Response object
    ///     is set to true.
    /// </summary>
    [XmlElement(ElementName = "lastUpdate", IsNullable = true)]
    public DateTime? LastUpdate
    {
        get => _lastUpdateField;
        set => _lastUpdateField = value;
    }

    /// <summary>
    ///     The preferredLanguage element is an xs:language value that indicates the preferred caller language. The
    ///     GetPoliciesResponse message SHOULD be returned in the preferred client language. If the preferredLanguage is not
    ///     present in the SupportedLanguages data element, the GetPoliciesResponse message MUST return using the language
    ///     specified by the DefaultLanguage data element.
    /// </summary>
    [XmlElement(ElementName = "preferredLanguage")]
    public string PreferredLanguage
    {
        get => _preferredLanguageField;
        set => _preferredLanguageField = value;
    }
}