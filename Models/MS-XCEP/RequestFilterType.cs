// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_XCEP;

/// <summary>
///     The RequestFilter complex type is provided in a request and used by the server to filter the GetPoliciesResponse to
///     contain only CertificateEnrollmentPolicy objects that satisfy the filter.
/// </summary>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy")]
[XmlRoot(ElementName = "RequestFilter", IsNullable = false)]
public class RequestFilterType
{
    private int _clientVersionField;
    
    private List<string>? _policyOiDsField;

    private int _serverVersionField;

    /// <summary>
    ///     An instance of a FilterOIDCollection object as defined in section 3.1.4.1.3.14. If the policyOIDs element is nil,
    ///     the server MUST NOT apply an OID filter to the policies returned in the GetPoliciesResponse message.
    /// </summary>
    [XmlArray(ElementName = "policyOIDs", IsNullable = true)]
    [XmlArrayItem(ElementName = "oid")]
    public List<string>? PolicyOiDs
    {
        get => _policyOiDsField;
        set => _policyOiDsField = value;
    }

    /// <summary>
    ///     The server SHOULD only return CertificateEnrollmentPolicy objects whose bitwise AND of the privateKeyFlags element
    ///     of the attributes element with 0x0F000000 is smaller than or equal to 0x0Z000000, where Z denotes the value of the
    ///     clientVersion.
    /// </summary>
    [XmlElement(ElementName = "clientVersion")]
    public int ClientVersion
    {
        get => _clientVersionField;
        set => _clientVersionField = value;
    }

    /// <summary>
    ///     The server SHOULD only return the CertificateEnrollmentPolicy objects whose bitwise AND of the privateKeyFlags
    ///     element of the attributes element with 0x000F0000 is smaller than or equal to 0x000Y0000, where Y denotes the value
    ///     of the serverVersion.
    /// </summary>
    [XmlElement(ElementName = "serverVersion")]
    public int ServerVersion
    {
        get => _serverVersionField;
        set => _serverVersionField = value;
    }
}