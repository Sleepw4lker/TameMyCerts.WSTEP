// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_WSTEP;

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://docs.oasis-open.org/ws-sx/ws-trust/200512")]
[XmlRoot(ElementName = "RequestedSecurityToken")]
public class RequestedSecurityTokenType
{
    private BinarySecurityTokenType _binarySecurityTokenField;

    [XmlElement(ElementName = "BinarySecurityToken",
        Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
    public BinarySecurityTokenType BinarySecurityToken
    {
        get => _binarySecurityTokenField;
        set => _binarySecurityTokenField = value;
    }
}