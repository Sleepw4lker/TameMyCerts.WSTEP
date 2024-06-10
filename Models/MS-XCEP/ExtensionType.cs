// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_XCEP;

/// <summary>
///     The Extension complex type is used to provide an X.509v3 Certificate Extension, as specified in [RFC5280] section
///     4.1.2.9.
/// </summary>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy")]
public class ExtensionType
{
    private bool _criticalField;

    private int _oIdReferenceField;

    private string _valueField;

    /// <summary>
    ///     The oIDReference element is an integer value that references an existing oIDReferenceID element of an existing
    ///     object identifier (OID) object as defined in section 3.1.4.1.3.16. The integer value MUST be a valid OID reference
    ///     ID.
    /// </summary>
    [XmlElement(ElementName = "oIDReference")]
    public int OIdReference
    {
        get => _oIdReferenceField;
        set => _oIdReferenceField = value;
    }

    /// <summary>
    ///     The critical element is used to indicate whether the Extension is critical. A value of true indicates that the
    ///     Extension is critical. A value of false indicates that the Extension is not critical.
    /// </summary>
    [XmlElement(ElementName = "critical")]
    public bool Critical
    {
        get => _criticalField;
        set => _criticalField = value;
    }

    /// <summary>
    ///     The value element contains the xs:base64Binary representation of the ASN.1 encoded value of the Extension.
    /// </summary>
    [XmlElement(ElementName = "value")]
    public string Value
    {
        get => _valueField;
        set => _valueField = value;
    }
}