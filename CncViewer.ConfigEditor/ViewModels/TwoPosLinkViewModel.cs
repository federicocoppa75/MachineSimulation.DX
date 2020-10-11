using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CncViewer.ConfigEditor.Enums;
using CncViewer.Models.Connection.Links;

namespace CncViewer.ConfigEditor.ViewModels
{
    class TwoPosLinkViewModel : LinkViewModel
    {
        public override LinkType Type => LinkType.TwoPos;

        public TwoPosLinkViewModel(int id) : base(id)
        {

        }

        public override LinkData ToModel() => new TwoPosLinkData() { LinkId = Id, Variable = Variable, Descrition = Description };
    }
}
