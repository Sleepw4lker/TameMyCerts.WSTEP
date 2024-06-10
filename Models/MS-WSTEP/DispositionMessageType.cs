// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_WSTEP;

/// <summary>
///     The DispositionMessageType is an extension to the string type that allows an attribute definition of the language
///     for the string. The DispositionMessageType is used to provide additional information about the server processing.
/// </summary>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollment")]
[XmlRoot(ElementName = "DispositionMessage",
    Namespace = "http://schemas.microsoft.com/windows/pki/2009/01/enrollment",
    IsNullable = false)]
public class DispositionMessageType
{
    private string _langField;

    private string _valueField;

    /// <summary>
    ///     The language reference xml:lang, indicating the natural or formal language the string element content is written
    ///     in.
    /// </summary>
    [XmlAttribute(AttributeName = "lang", Form = XmlSchemaForm.Qualified,
        Namespace = "http://www.w3.org/XML/1998/namespace")]
    public string Lang
    {
        get => _langField;
        set => _langField = value;
    }

    /// <summary>
    ///     The string element contains the literal string disposition message returned from the server. The string element
    ///     contains an xml:lang attribute that defines the language for the string. The language SHOULD be provided for each
    ///     string element instance.
    /// </summary>
    [XmlText]
    public string Value
    {
        get => _valueField;
        set => _valueField = value;
    }
}