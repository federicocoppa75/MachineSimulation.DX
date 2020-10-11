using CncViewer.Connection.ViewModels.Links;
using System;
using System.Collections.Generic;
using System.Text;

namespace CncViewer.Connection.Messages
{
    public class LoadLinksConnectionsMessage
    {
        public IEnumerable<LinkViewModel> Links { get; set; }
    }
}
