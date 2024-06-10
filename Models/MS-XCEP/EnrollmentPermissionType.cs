// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_XCEP;

/// <summary>
///     The EnrollmentPermission complex type is used to convey the permissions for the associated parent object.
/// </summary>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy")]
public class EnrollmentPermissionType
{
    private bool _autoEnrollField;

    private bool _enrollField;

    /// <summary>
    ///     The xs:boolean enroll element is used to indicate whether the requestor has permission to enroll. If the enroll
    ///     element is true, the requestor has permission to enroll. If the enroll element is false, the requestor does not
    ///     have permission to enroll.
    /// </summary>
    [XmlElement(ElementName = "enroll")]
    public bool Enroll
    {
        get => _enrollField;
        set => _enrollField = value;
    }

    /// <summary>
    ///     The xs:boolean autoEnroll element is used to indicate whether the requestor has permission to automatically enroll.
    ///     If the autoEnroll element is true, the requestor has permission to automatically enroll. If the autoEnroll element
    ///     is false, the requestor does not have permission to automatically enroll.
    /// </summary>
    [XmlElement(ElementName = "autoEnroll")]
    public bool AutoEnroll
    {
        get => _autoEnrollField;
        set => _autoEnrollField = value;
    }
}