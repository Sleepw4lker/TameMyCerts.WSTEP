// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_XCEP;

/// <summary>
///     The KeyArchivalAttributes complex type contains the required attributes that MUST be used on the client prior to
///     sending the client private key to the server for archival.
/// </summary>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy")]
public class KeyArchivalAttributesType
{
    private uint _symmetricAlgorithmKeyLengthField;
    private int _symmetricAlgorithmOidReferenceField;

    /// <summary>
    ///     A reference to an oIDReferenceID element of an existing OID object as defined in section 3.1.4.1.3.16. The
    ///     referenced OID object identifies the expected symmetric key algorithm used when encrypting a private key during key
    ///     exchange requests. The value MUST correspond to an existing oIDReferenceID in the GetPoliciesResponse (section
    ///     3.1.4.1.1.2) message.
    /// </summary>
    [XmlElement(ElementName = "symmetricAlgorithmOIDReference")]
    public int SymmetricAlgorithmOidReference
    {
        get => _symmetricAlgorithmOidReferenceField;
        set => _symmetricAlgorithmOidReferenceField = value;
    }

    /// <summary>
    ///     An integer value representing the expected bit length of a symmetric key used when encrypting a private key during
    ///     key exchange requests. The symmetricAlgorithmKeyLength element MUST be a positive nonzero integer value.
    /// </summary>
    [XmlElement(ElementName = "symmetricAlgorithmKeyLength")]
    public uint SymmetricAlgorithmKeyLength
    {
        get => _symmetricAlgorithmKeyLengthField;
        set => _symmetricAlgorithmKeyLengthField = value;
    }
}