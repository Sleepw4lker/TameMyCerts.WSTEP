// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_XCEP;

/// <summary>
///     The Revision complex type identifies the version information of a CertificateEnrollmentPolicy object.
/// </summary>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy")]
public class RevisionType
{
    private uint _majorRevisionField;

    private uint _minorRevisionField;

    /// <summary>
    ///     The major version number of the corresponding CertificateEnrollmentPolicy object. The majorRevision element MUST be
    ///     a positive nonzero integer. The majorRevision element will be populated from the revision attribute as specified in
    ///     [MS-CRTD] section 2.6.
    /// </summary>
    [XmlElement(ElementName = "majorRevision")]
    public uint MajorRevision
    {
        get => _majorRevisionField;
        set => _majorRevisionField = value;
    }

    /// <summary>
    ///     The minor version number of the corresponding CertificateEnrollmentPolicy object. The minorRevision element MUST be
    ///     an integer greater than or equal to 0. The minorRevision element will be populated from the
    ///     msPKI-Template-Minor-Revision attribute as specified in [MS-CRTD] section 2.17.
    /// </summary>
    [XmlElement(ElementName = "minorRevision")]
    public uint MinorRevision
    {
        get => _minorRevisionField;
        set => _minorRevisionField = value;
    }
}