// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_WSTEP;

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace =
    "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
[XmlRoot(ElementName = "BinarySecurityToken",
    Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd",
    IsNullable = false)]
public class BinarySecurityTokenType
{
    private string _encodingTypeField;

    private string _idField;

    private string _valueField;

    private string _valueTypeField;

    [XmlAttribute(AttributeName = "ValueType")]
    public string ValueType
    {
        get => _valueTypeField;
        set => _valueTypeField = value;
    }

    [XmlAttribute(AttributeName = "EncodingType")]
    public string EncodingType
    {
        get => _encodingTypeField;
        set => _encodingTypeField = value;
    }

    [XmlAttribute(AttributeName = "Id", Form = XmlSchemaForm.Qualified,
        Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
    public string Id
    {
        get => _idField;
        set => _idField = value;
    }

    [XmlText]
    public string Value
    {
        get => _valueField;
        set => _valueField = value
            .Replace(Environment.NewLine, string.Empty)
            .Replace(" ", string.Empty);
    }
}