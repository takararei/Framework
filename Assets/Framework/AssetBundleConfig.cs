using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
[System.Serializable]
public class AssetBundleConfig
{
    [XmlElement("ABList")]
    public List<ABBase> ABList { get; set; }
}

[System.Serializable]
public class ABBase
{
    [XmlAttribute("Path")]
    public string Path { get; set; }
    [XmlAttribute("Crc")]
    public uint Crc { get; set; }//文件标识
    [XmlAttribute("ABName")]
    public string ABName { get; set; }
    [XmlAttribute("AssetName")]
    public string AssetName { get; set; }
    [XmlAttribute("ABDependence")]
    public List<string> ABDependence { get; set; }
}