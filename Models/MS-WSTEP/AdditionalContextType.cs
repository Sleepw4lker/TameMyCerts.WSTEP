﻿// Documentation is covered by Microsoft copyrights.

using System.ComponentModel;
using System.Xml.Serialization;

namespace TameMyCerts.WSTEP.Models.MS_WSTEP;

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

    public List<KeyValuePair<string, string>> ToList()
    {
        return ContextItem.Select(item => new KeyValuePair<string, string>(item.Name, item.Value)).ToList();
    }
}