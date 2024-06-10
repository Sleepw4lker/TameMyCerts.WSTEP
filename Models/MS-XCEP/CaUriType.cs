// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_XCEP;

/// <summary>
///     The CAURI complex type is used to define the URI for a certificate authority, which includes specifying the
///     supported authentication type, the URI, and a relative priority.
/// </summary>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy")]
public class CaUriType
{
    private int _clientAuthenticationField;

    private int _priorityField;

    private bool _renewalOnlyField;

    private string _uriField;

    /// <summary>
    ///     The clientAuthentication element is used to define the supported authentication type for the uri element of this
    ///     CAURI object. The clientAuthentication element is an unsigned integer that MUST have one of the following values:
    ///     1,2,4,8
    /// </summary>
    [XmlElement(ElementName = "clientAuthentication")]
    public int ClientAuthentication
    {
        get => _clientAuthenticationField;
        set => _clientAuthenticationField = value;
    }

    /// <summary>
    ///     The uri element is used to store a Uniform Resource Identifier (URI) entry for a CA (section 3.1.4.1.3.2) object.
    /// </summary>
    [XmlElement(ElementName = "uri")]
    public string Uri
    {
        get => _uriField;
        set => _uriField = value;
    }

    /// <summary>
    ///     The priority element is an integer value that represents the priority value for the URI. The priority element value
    ///     is used as a relative indicator against other CAURI objects. The lower the integer value, the higher the priority.
    ///     Two CAURI objects have the same priority if the integer values of each priority element are the same. A CAURI
    ///     object is considered to have a lower priority if the priority element integer value is more than the integer value
    ///     of the priority element of an alternate CAURI object.
    /// </summary>
    [XmlElement(ElementName = "priority")]
    public int Priority
    {
        get => _priorityField;
        set => _priorityField = value;
    }

    /// <summary>
    ///     The renewalOnly element is an xs:boolean value that identifies whether the corresponding CAURI object can accept
    ///     all types of requests, or only renewal requests. If the value is true, the server that is addressed by the CAURI
    ///     object only accepts renewal requests. If the value is false, other request types are supported.
    /// </summary>
    [XmlElement(ElementName = "renewalOnly")]
    public bool RenewalOnly
    {
        get => _renewalOnlyField;
        set => _renewalOnlyField = value;
    }
}