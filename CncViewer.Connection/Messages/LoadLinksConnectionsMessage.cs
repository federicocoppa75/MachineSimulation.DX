using CncViewer.Connection.ViewModels.Links;
using CncViewer.Models.Connection.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CncViewer.Connection.Messages
{
    public class LoadLinksConnectionsMessage
    {
        public ChannelType ChannelType { get; set; }
        public IEnumerable<LinkViewModel> Links { get; set; }
    }
}
