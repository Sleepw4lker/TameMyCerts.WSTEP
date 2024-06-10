// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_XCEP;

/// <summary>
///     If additional registration authority (RA) key(s) are required in signing enrollment requests for this policy, these
///     keys are defined in an RARequirements object.
/// </summary>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy")]
public class RaRequirementsType
{
    private List<int>? _rAekUsField;
    
    private List<int>? _rAPoliciesField;

    private uint _rASignaturesField;

    /// <summary>
    ///     Defines an integer indicating the number of additional signatures required. The rASignatures element MUST be an
    ///     integer greater than or equal to 0.
    /// </summary>
    [XmlElement(ElementName = "rASignatures")]
    public uint RaSignatures
    {
        get => _rASignaturesField;
        set => _rASignaturesField = value;
    }

    /// <summary>
    ///     An instance of an OIDReferenceCollection object as defined in section 3.1.4.1.3.18. The rAEKUs element defines the
    ///     required values in the extended key usage (EKU) extension of the RA certificate.
    /// </summary>
    [XmlArray(ElementName = "rAEKUs", IsNullable = true)]
    [XmlArrayItem(ElementName = "oIDReference")]
    public List<int>? RAekUs
    {
        get => _rAekUsField;
        set => _rAekUsField = value;
    }

    /// <summary>
    ///     An instance of an OIDReferenceCollection object defined in section 3.1.4.1.3.18. The rAPolicies element defines the
    ///     required values in the policy extension of the RA certificate.
    /// </summary>
    [XmlArray(ElementName = "rAPolicies", IsNullable = true)]
    [XmlArrayItem(ElementName = "oIDReference")]
    public List<int>? RaPolicies
    {
        get => _rAPoliciesField;
        set => _rAPoliciesField = value;
    }
}