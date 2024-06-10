// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_XCEP;

/// <summary>
///     The Attributes complex type contains information about a CertificateEnrollmentPolicy object defined in section
///     3.1.4.1.3.7. It MUST be present for each CertificateEnrollmentPolicy object instance.
/// </summary>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy")]
public class AttributesType
{
    private CertificateValidityType _certificateValidityField;

    private string _commonNameField;

    private uint _enrollmentFlagsField;

    private List<ExtensionType> _extensionsField = new();

    private uint _generalFlagsField;

    private int? _hashAlgorithmOidReferenceField;

    private KeyArchivalAttributesType? _keyArchivalAttributesField;

    private EnrollmentPermissionType _permissionField;

    private uint _policySchemaField;

    private PrivateKeyAttributesType _privateKeyAttributesField;

    private uint _privateKeyFlagsField;

    private RaRequirementsType? _rARequirementsField;

    private RevisionType _revisionField;

    private uint _subjectNameFlagsField;

    private List<string>? _supersededPoliciesField;

    /// <summary>
    ///     A string value of the common name (CN) of a CertificateEnrollmentPolicy object. The xcep:commonName element MUST be
    ///     unique in the scope of a GetPoliciesResponse (section 3.1.4.1.1.2) message.
    /// </summary>
    [XmlElement(ElementName = "commonName")]
    public string CommonName
    {
        get => _commonNameField;
        set => _commonNameField = value;
    }

    /// <summary>
    ///     An integer value presenting the schema version of the corresponding CertificateEnrollmentPolicy. The policySchema
    ///     element SHOULD be an integer value of 1, 2, or 3.
    /// </summary>
    [XmlElement(ElementName = "policySchema")]
    public uint PolicySchema
    {
        get => _policySchemaField;
        set => _policySchemaField = value;
    }

    /// <summary>
    ///     An instance of a CertificateValidity object as defined in section 3.1.4.1.3.8.
    /// </summary>
    [XmlElement(ElementName = "certificateValidity")]
    public CertificateValidityType CertificateValidity
    {
        get => _certificateValidityField;
        set => _certificateValidityField = value;
    }

    /// <summary>
    ///     An instance of an EnrollmentPermission object as defined in section 3.1.4.1.3.11.
    /// </summary>
    [XmlElement(ElementName = "permission")]
    public EnrollmentPermissionType Permission
    {
        get => _permissionField;
        set => _permissionField = value;
    }

    /// <summary>
    ///     An instance of a PrivateKeyAttributes object as defined in section 3.1.4.1.3.20.
    /// </summary>
    [XmlElement(ElementName = "privateKeyAttributes")]
    public PrivateKeyAttributesType PrivateKeyAttributes
    {
        get => _privateKeyAttributesField;
        set => _privateKeyAttributesField = value;
    }

    /// <summary>
    ///     An instance of a Revision object as defined in section 3.1.4.1.3.24.
    /// </summary>
    [XmlElement(ElementName = "revision")]
    public RevisionType Revision
    {
        get => _revisionField;
        set => _revisionField = value;
    }

    /// <summary>
    ///     An instance of a SupersededPolicies object as defined in section 3.1.4.1.3.25. A value of nil indicates that the
    ///     corresponding CertificateEnrollmentPolicy object does not supersede another.
    ///     A list of superseded policies identified by the value of their commonName attribute. The list is not ordered.
    ///     The commonName is a string value representing the common name of a CertificateEnrollmentPolicy object that has been
    ///     superseded by the CertificateEnrollmentPolicy object corresponding to this SupersededPolicies object. The list of
    ///     commonName elements in the SupersededPolicies object is constructed based on the msPKI-Supersede-Templates
    ///     attribute as specified in [MS-CRTD] section 2.21. Each value is returned as a string element.
    /// </summary>
    [XmlArray(ElementName = "supersededPolicies", IsNullable = true)]
    [XmlArrayItem(ElementName = "commonName")]
    public List<string>? SupersededPolicies
    {
        get => _supersededPoliciesField;
        set => _supersededPoliciesField = value;
    }

    /// <summary>
    ///     The privateKeyFlags element is an unsigned integer that MUST be a bitwise OR of zero or more of the following
    ///     possible values. For more information about the relationship between the privateKeyFlags element and the
    ///     msPKI-Private-Key-Flag attribute, see [MS-WCCE] section 3.1.2.4.2.2.2.8.
    /// </summary>
    [XmlElement(ElementName = "privateKeyFlags")]
    public uint PrivateKeyFlags
    {
        get => _privateKeyFlagsField;
        set => _privateKeyFlagsField = value;
    }

    /// <summary>
    ///     The subjectNameFlags element is an unsigned integer that MUST be a bitwise OR of zero or more of the following
    ///     possible values.
    /// </summary>
    [XmlElement(ElementName = "subjectNameFlags")]
    public uint SubjectNameFlags
    {
        get => _subjectNameFlagsField;
        set => _subjectNameFlagsField = value;
    }

    /// <summary>
    ///     The enrollmentFlags element is an unsigned integer that MUST be a bitwise OR of zero or more of the following
    ///     values.
    /// </summary>
    [XmlElement(ElementName = "enrollmentFlags")]
    public uint EnrollmentFlags
    {
        get => _enrollmentFlagsField;
        set => _enrollmentFlagsField = value;
    }

    /// <summary>
    ///     The generalFlags element is an unsigned integer that MUST be a bitwise OR of zero or more of the following values.
    /// </summary>
    [XmlElement(ElementName = "generalFlags")]
    public uint GeneralFlags
    {
        get => _generalFlagsField;
        set => _generalFlagsField = value;
    }

    /// <summary>
    ///     An integer value that references an existing oIDReferenceID element as defined in section 3.1.4.1.3.16. The hash
    ///     algorithm is used when signing operations are performed during the certificate enrollment process. If the value of
    ///     the policySchema element for this Attributes object is 3 and the hash algorithm is defined for the policy, the
    ///     value of the hashAlgorithmOIDReference element MUST be an integer that references the oIDReferenceID of the
    ///     corresponding hash algorithm definition. If the value of the policySchema element for this Attributes object is 1
    ///     or 2, or the hash algorithm is not defined, the hashAlgorithmOIDReference element MUST be specified as nil.
    /// </summary>
    [XmlElement(ElementName = "hashAlgorithmOIDReference", IsNullable = true)]
    public int? HashAlgorithmOidReference
    {
        get => _hashAlgorithmOidReferenceField;
        set => _hashAlgorithmOidReferenceField = value;
    }

    /// <summary>
    ///     An instance of an RARequirements object as defined in section 3.1.4.1.3.21.
    /// </summary>
    [XmlElement(ElementName = "rARequirements", IsNullable = true)]
    public RaRequirementsType? RaRequirements
    {
        get => _rARequirementsField;
        set => _rARequirementsField = value;
    }

    /// <summary>
    ///     An instance of a KeyArchivalAttributes object as defined in section 3.1.4.1.3.15.
    /// </summary>
    [XmlElement(ElementName = "keyArchivalAttributes", IsNullable = true)]
    public KeyArchivalAttributesType? KeyArchivalAttributes
    {
        get => _keyArchivalAttributesField;
        set => _keyArchivalAttributesField = value;
    }

    /// <summary>
    ///     An instance of an ExtensionCollection object as defined in section 3.1.4.1.3.13.
    /// </summary>
    [XmlArray(ElementName = "extensions", IsNullable = true)]
    [XmlArrayItem("extension")]
    public List<ExtensionType> Extensions
    {
        get => _extensionsField;
        set => _extensionsField = value;
    }
}