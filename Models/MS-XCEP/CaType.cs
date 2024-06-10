// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_XCEP;

/// <summary>
///     The CA complex type is used to encapsulate information about a certificate authority, including one or more URIs
///     that are available for enrollment operations and permissions.
/// </summary>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy")]
public class CaType
{
    private int _cAReferenceIdField;

    private string _certificateField;

    private bool _enrollPermissionField;

    private List<CaUriType> _caUriCollectionField = new();

    /// <summary>
    ///     An instance of a CAURICollection object as defined in section 3.1.4.1.3.6, which contains the list of URI values
    ///     for a certificate authority.
    /// </summary>
    [XmlArray(ElementName = "uris")]
    [XmlArrayItem(ElementName = "cAURI")]
    public List<CaUriType> Uris
    {
        get => _caUriCollectionField;
        set => _caUriCollectionField = value;
    }

    /// <summary>
    ///     The certificate element contains the xs:base64Binary representation of the Abstract Syntax Notation One (ASN.1)
    ///     encoded certificate authority signing certificate. The value for the certificate element MUST never be an empty
    ///     string.
    /// </summary>
    [XmlElement(ElementName = "certificate")]
    public string Certificate
    {
        get => _certificateField;
        set => _certificateField = value;
    }

    /// <summary>
    ///     The enrollPermission element contains an xs:boolean value that indicates whether or not the requestor has
    ///     permission to submit enrollment requests to the server represented by the corresponding CA object. It MUST be true
    ///     or false. If the enrollPermission element is true, the requestor has enroll permissions and can submit requests. If
    ///     the enrollPermission element is false, the requestor does not have permission.
    /// </summary>
    [XmlElement(ElementName = "enrollPermission")]
    public bool EnrollPermission
    {
        get => _enrollPermissionField;
        set => _enrollPermissionField = value;
    }

    /// <summary>
    ///     Each instance of a CA object in a GetPoliciesResponse message MUST have a unique cAReferenceID. The cAReferenceID
    ///     is an unsigned integer value used as an index for referencing the corresponding CA object within the scope of a
    ///     GetPoliciesResponse message.
    /// </summary>
    [XmlElement(ElementName = "cAReferenceID")]
    public int CaReferenceId
    {
        get => _cAReferenceIdField;
        set => _cAReferenceIdField = value;
    }
}