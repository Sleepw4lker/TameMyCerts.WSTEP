// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_XCEP;

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollmentpolicy")]
[XmlRoot(ElementName = "Response")]
public class ResponseType
{
    private List<CertificateEnrollmentPolicyType> _certificateEnrollmentPolicyCollectionField;

    private uint? _nextUpdateHoursField;

    private bool? _policiesNotChangedField;

    private string _policyFriendlyNameField = string.Empty;

    // policyId seems to represent the AD Forest GUID
    // TODO: REST API must deliver this? IMO this can be any arbitrary string as long as it is unique.
    private string _policyIdField;

    public ResponseType()
    {
    }

    public ResponseType(List<CertificateEnrollmentPolicyType> certificateEnrollmentPolicyCollectionField,
        string policyFriendlyNameField, string policyIdField)
    {
        _certificateEnrollmentPolicyCollectionField = certificateEnrollmentPolicyCollectionField;
        _policyFriendlyNameField = policyFriendlyNameField;
        _policyIdField = policyIdField;
    }

    /// <summary>
    ///     A unique identifier for the certificate enrollment policy. Two or more servers can respond with the same policyID
    ///     element in a GetPoliciesResponse message if, and only if, they are configured to return the same Response object to
    ///     the same requestor. The policyID element is not intended to be a human-readable property.
    /// </summary>
    [XmlElement(ElementName = "policyID")]
    public string PolicyId
    {
        get => _policyIdField;
        set => _policyIdField = value;
    }

    /// <summary>
    ///     A human readable friendly name for the certificate enrollment policy.
    /// </summary>
    [XmlElement(ElementName = "policyFriendlyName")]
    public string PolicyFriendlyName
    {
        get => _policyFriendlyNameField;
        set => _policyFriendlyNameField = value;
    }

    /// <summary>
    ///     An integer representing the number of hours that the server recommends the client wait before submitting another
    ///     GetPolicies message. If the nextUpdateHours element is present and not nil, the nextUpdateHours element value MUST
    ///     be a positive nonzero integer.
    /// </summary>
    [XmlElement(ElementName = "nextUpdateHours", IsNullable = true)]
    public uint? NextUpdateHours
    {
        get => _nextUpdateHoursField;
        set => _nextUpdateHoursField = value;
    }

    /// <summary>
    ///     Used to indicate to the requestor whether the policies have changed since the requestor specified lastUpdateTime in
    ///     the GetPolicies request message as described in section 3.1.4.1.3.9. If the value of the policiesNotChanged element
    ///     is true, the policy has not changed since the lastUpdateTime value in the GetPolicies message. If the
    ///     policiesNotChanged element is false or nil, the policy has changed since the requestor specified lastUpdateTime.
    /// </summary>
    [XmlElement(ElementName = "policiesNotChanged", IsNullable = true)]
    public bool? PoliciesNotChanged
    {
        get => _policiesNotChangedField;
        set => _policiesNotChangedField = value;
    }

    /// <summary>
    ///     A list of CertificateEnrollmentPolicy objects. The list is not ordered. The PolicyCollection is used to group
    ///     CertificateEnrollmentPolicy objects together.
    /// </summary>
    [XmlArray(ElementName = "policies")]
    [XmlArrayItem(ElementName = "policy")]
    public List<CertificateEnrollmentPolicyType> CertificateEnrollmentPolicyCollection
    {
        get => _certificateEnrollmentPolicyCollectionField;
        set => _certificateEnrollmentPolicyCollectionField = value;
    }
}