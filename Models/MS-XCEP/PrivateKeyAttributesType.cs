// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_XCEP;

/// <summary>
///     The PrivateKeyAttributes complex type contains the attributes for the private key that will be associated with any
///     certificate request for the corresponding CertificateEnrollmentPolicy object.
/// </summary>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy")]
public class PrivateKeyAttributesType
{
    private int? _algorithmOidReferenceField;

    private List<string> _cryptoProvidersField = new();

    private uint _keySpecField;

    private uint? _keyUsagePropertyField;

    private ushort _minimalKeyLengthField;

    private string? _permissionsField;

    /// <summary>
    ///     An integer specifying the minimum key length in bits for the private key. The value of the minimalKeyLength element
    ///     MUST be a positive nonzero number.
    /// </summary>
    [XmlElement(ElementName = "minimalKeyLength")]
    public ushort MinimalKeyLength
    {
        get => _minimalKeyLengthField;
        set => _minimalKeyLengthField = value;
    }

    /// <summary>
    ///     This element has identical semantics for the pKIDefaultKeySpec attribute specified in [MS-WCCE] section
    ///     3.1.2.4.2.2.1.5.
    /// </summary>
    [XmlElement(ElementName = "keySpec")]
    public uint KeySpec
    {
        get => _keySpecField;
        set => _keySpecField = value;
    }

    /// <summary>
    ///     This element has identical semantics to the pKIKeyUsage attribute specified in [MS-WCCE] section 3.1.2.4.2.2.1.3.
    /// </summary>
    [XmlElement(ElementName = "keyUsageProperty", IsNullable = true)]
    public uint? KeyUsageProperty
    {
        get => _keyUsagePropertyField;
        set => _keyUsagePropertyField = value;
    }

    /// <summary>
    ///     Used to specify a Security Descriptor Definition Language (SDDL) representation of the permissions when a private
    ///     key is created.
    /// </summary>
    [XmlElement(ElementName = "permissions", IsNullable = true)]
    public string? Permissions
    {
        get => _permissionsField;
        set => _permissionsField = value;
    }

    /// <summary>
    ///     An integer reference to an oIDReferenceID element of an existing OID (section 3.1.4.1.3.16) object in a
    ///     GetPoliciesResponse message. The OID object that is referenced corresponds to the asymmetric algorithm of the
    ///     private key.
    /// </summary>
    [XmlElement(ElementName = "algorithmOIDReference", IsNullable = true)]
    public int? AlgorithmOidReference
    {
        get => _algorithmOidReferenceField;
        set => _algorithmOidReferenceField = value;
    }

    /// <summary>
    ///     An instance of the CryptoProviders object as specified in section 3.1.4.1.3.10. If there are no cryptographic
    ///     providers to be specified, the cryptoProviders element MUST be nil.
    /// </summary>
    [XmlArray(ElementName = "cryptoProviders")]
    [XmlArrayItem(typeof(string), ElementName = "provider", IsNullable = true)]
    public List<string> CryptoProviders
    {
        get => _cryptoProvidersField;
        set => _cryptoProvidersField = value;
    }
}