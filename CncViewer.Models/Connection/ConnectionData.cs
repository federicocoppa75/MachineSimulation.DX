using CncViewer.Models.Connection.Enums;
using CncViewer.Models.Connection.Links;
using System;
using System.Collections.Generic;
using System.Text;

namespace CncViewer.Models.Connection
{
    public class ConnectionData
    {
        public ChannelType ChannelType { get; set; }
        public List<LinkData> Links { get; set; } = new List<LinkData>();
    }
}
