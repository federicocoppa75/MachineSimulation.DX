using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CncViewer.Models.Connection.Links
{
    [XmlInclude(typeof(LinearLinkData))]
    [XmlInclude(typeof(TwoPosLinkData))]
    [XmlInclude(typeof(WriteTwoPosData))]
    [XmlInclude(typeof(PulseTwoPosData))]
    public class LinkData
    {
        public int LinkId { get; set; }
        public string Variable { get; set; }
        public string Descrition { get; set; }
    }
}
