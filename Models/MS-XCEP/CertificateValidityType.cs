// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_XCEP;

/// <summary>
///     The CertificateValidity complex type contains information about the expected validity of an issued certificate, and
///     the expected period when renewal starts.
/// </summary>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy")]
public class CertificateValidityType
{
    private uint _renewalPeriodSecondsField;

    private uint _validityPeriodSecondsField;

    /// <summary>
    ///     The validityPeriodSeconds element is the recommended validity period of an issued certificate in seconds. The
    ///     validityPeriodSeconds element MUST be a positive nonzero long.
    /// </summary>
    [XmlElement(ElementName = "validityPeriodSeconds")]
    public uint ValidityPeriodSeconds
    {
        get => _validityPeriodSecondsField;
        set => _validityPeriodSecondsField = value;
    }

    /// <summary>
    ///     The renewalPeriodSeconds element is the recommended renewal period of an issued certificate. The
    ///     renewalPeriodSeconds element MUST be a positive nonzero long.
    /// </summary>
    [XmlElement(ElementName = "renewalPeriodSeconds")]
    public uint RenewalPeriodSeconds
    {
        get => _renewalPeriodSecondsField;
        set => _renewalPeriodSecondsField = value;
    }
}