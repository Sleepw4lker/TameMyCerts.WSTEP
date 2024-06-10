// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_XCEP;

/// <summary>
///     The OID complex type is used and referenced throughout the X.509 Certificate Enrollment Policy Protocol to identify
///     an object and to provide generic attributes on the object. Each OID object has a specific intended purpose, denoted
///     by the group element.
/// </summary>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy")]
public class OidType
{
    private string _defaultNameField;

    private uint _groupField;

    private int _oIdReferenceIdField;

    private string _valueField;

    /// <summary>
    ///     The object identifier value (for example, 1.2.3.4).
    /// </summary>
    [XmlElement(ElementName = "value")]
    public string Value
    {
        get => _valueField;
        set => _valueField = value;
    }

    /// <summary>
    ///     The integer value that identifies the type of object that the OID object represents. The group element MUST be one
    ///     of the following integer values: 1,2,3,4,5,6,7,8,9
    /// </summary>
    [XmlElement(ElementName = "group")]
    public uint Group
    {
        get => _groupField;
        set => _groupField = value;
    }

    /// <summary>
    ///     The integer identifier for the OID. The value of oIDReferenceID MUST be unique for each unique OID object instance
    ///     in a GetPoliciesResponse message.
    /// </summary>
    [XmlElement(ElementName = "oIDReferenceID")]
    public int OIdReferenceId
    {
        get => _oIdReferenceIdField;
        set => _oIdReferenceIdField = value;
    }

    /// <summary>
    ///     A friendly name for the OID object. The defaultName element MUST be provided in a GetPoliciesResponse message. The
    ///     defaultName is not localized and has no language specifier.
    /// </summary>
    [XmlElement(ElementName = "defaultName")]
    public string DefaultName
    {
        get => _defaultNameField;
        set => _defaultNameField = value;
    }
}