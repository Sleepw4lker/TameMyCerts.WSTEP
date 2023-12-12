using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models
{
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

        [XmlAttribute(AttributeName = "lang", Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Lang
        {
            get => _langField;
            set => _langField = value;
        }

        [XmlText]
        public string Value
        {
            get => _valueField;
            set => _valueField = value;
        }
    }
}