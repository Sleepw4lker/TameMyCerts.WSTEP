using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/ws/2006/12/authorization")]
    [XmlRoot(ElementName = "AdditionalContext", 
        Namespace = "http://schemas.xmlsoap.org/ws/2006/12/authorization",
        IsNullable = false)]
    public class AdditionalContextType
    {
        private ContextItemType[] _contextItemField;

        [XmlElement(ElementName = "ContextItem")]
        public ContextItemType[] ContextItem
        {
            get => _contextItemField;
            set => _contextItemField = value;
        }
    }
}