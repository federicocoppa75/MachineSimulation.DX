using CncViewer.Connection.Messages;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CncViewer.Connection.ViewModels.Links
{
    public class LinksConnectionsViewModel : ViewModelBase
    {
        public ObservableCollection<LinkViewModel> Links { get; private set; } = new ObservableCollection<LinkViewModel>();

        private LinkViewModel _selectedLink;
        public LinkViewModel SelectedLink
        {
            get => _selectedLink;
            set => Set(ref _selectedLink, value, nameof(SelectedLink));
        }

        public LinksConnectionsViewModel()
        {
            MessengerInstance.Register<LoadLinksConnectionsMessage>(this, OnLoadLinksConnectionsMessage);
        }

        private void OnLoadLinksConnectionsMessage(LoadLinksConnectionsMessage msg)
        {
            Links.Clear();

            foreach (var item in msg.Links)
            {
                Links.Add(item);
            }
        }
    }
}
