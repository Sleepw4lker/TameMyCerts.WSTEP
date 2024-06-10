// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_XCEP;

/// <summary>
///     The CertificateEnrollmentPolicy complex type is used to encapsulate a certificate enrollment policy object and its
///     set of issuers. Each instance of a CertificateEnrollmentPolicy object is uniquely identified by its
///     policyOIDReference.
/// </summary>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy")]
public class CertificateEnrollmentPolicyType
{
    private AttributesType _attributesField;

    private List<int> _cAReferenceCollectionField = new();

    private int _policyOidReferenceField;

    /// <summary>
    ///     A policyOIDReference element is an integer value that references the oIDReferenceID element of an existing OID
    ///     object as defined in section 3.1.4.1.3.16. A policyOIDReference element MUST be present in the set of
    ///     oIDReferenceID element values in the corresponding GetPoliciesResponse message.
    /// </summary>
    [XmlElement(ElementName = "policyOIDReference")]
    public int PolicyOidReference
    {
        get => _policyOidReferenceField;
        set => _policyOidReferenceField = value;
    }

    /// <summary>
    ///     A cAs element is used to represent an instance of a CAReferenceCollection object as defined in section 3.1.4.1.3.4,
    ///     which is used to reference the issuers for this CertificateEnrollmentPolicy object.
    ///     The CAReferenceCollection complex type is used to group one or more cAReference elements together.
    ///     An integer value reference of an existing cAReferenceID element in a CA (section 3.1.4.1.3.2) object. The integer
    ///     value of each cAReference element MUST be unique for each CAReferenceCollection object and MUST reference a
    ///     cAReferenceID element defined in the corresponding GetPoliciesResponse message.
    /// </summary>
    [XmlArray(ElementName = "cAs")]
    [XmlArrayItem(ElementName = "cAReference")]
    public List<int> CaReferenceCollection
    {
        get => _cAReferenceCollectionField;
        set => _cAReferenceCollectionField = value;
    }

    /// <summary>
    ///     A attributes element is used to represent an instance of an Attributes object as defined in section 3.1.4.1.3.1.
    /// </summary>
    [XmlElement(ElementName = "attributes")]
    public AttributesType Attributes
    {
        get => _attributesField;
        set => _attributesField = value;
    }
}