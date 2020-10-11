using CncViewer.ConfigEditor.Enums;
using CncViewer.Models.Connection.Links;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CncViewer.ConfigEditor.ViewModels
{
    abstract class LinkViewModel : ViewModelBase
    {
        private int _id;
        public int Id
        {
            get => _id;
            protected set => Set(ref _id, value, nameof(Id));
        }

        abstract public LinkType Type { get;  }

        private string _variable;
        public string Variable
        {
            get => _variable;
            set => Set(ref _variable, value, nameof(Variable));
        }

        private string _description;

        public string Description
        {
            get => _description; 
            set => Set(ref _description, value, nameof(Description));
        }


        public LinkViewModel(int id) : base()
        {
            Id = id;
        }

        public abstract LinkData ToModel();
    }
}
