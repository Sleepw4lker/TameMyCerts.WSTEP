// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_WSTEP;

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/ws/2006/12/authorization")]
public class ContextItemType
{
    private string _nameField;

    private string _valueField;

    public string Value
    {
        get => _valueField;
        set => _valueField = value;
    }

    [XmlAttribute(AttributeName = "Name")]
    public string Name
    {
        get => _nameField;
        set => _nameField = value;
    }
}