using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CncViewer.ConfigEditor.Enums;
using CncViewer.Models.Connection.Links;

namespace CncViewer.ConfigEditor.ViewModels
{
    class PulseTwoPosViewModel : LinkViewModel
    {
        public new int Id
        {
            get => base.Id;
            set => base.Id = value;
        }

        public override LinkType Type => LinkType.PulseTwoPos;

        public PulseTwoPosViewModel(int id) : base(id)
        {
        }

        public override LinkData ToModel() => new PulseTwoPosData() { LinkId = Id, Variable = Variable, Descrition = Description };
    }
}
