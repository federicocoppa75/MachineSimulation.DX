using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CncViewer.ConfigEditor.Enums;
using CncViewer.Models.Connection.Links;

namespace CncViewer.ConfigEditor.ViewModels
{
    class LinearLinkViewModel : LinkViewModel
    {
        public override LinkType Type => LinkType.Linear;

        public LinearLinkViewModel(int id) : base(id)
        {

        }

        public override LinkData ToModel() => new LinearLinkData() { LinkId = Id, Variable = Variable, Descrition = Description };
    }
}
