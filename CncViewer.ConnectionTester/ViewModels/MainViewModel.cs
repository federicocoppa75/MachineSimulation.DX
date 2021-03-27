using CncViewer.Connection.Messages;
using CncViewer.Connection.ViewModels.Links;
using CncViewer.Models.Connection;
using CncViewer.Models.Connection.Enums;
using CncViewer.Models.Connection.Links;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CncViewer.ConnectionTester.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        //public ObservableCollection<LinkViewModel> Links { get; private set; } = new ObservableCollection<LinkViewModel>();

        //private LinkViewModel _selectedLink;

        public ChannelType ChannelType { get; set; }

        private ICommand _fileOpenCommand;
        public ICommand FileOpenCommand { get { return _fileOpenCommand ?? (_fileOpenCommand = new RelayCommand(() => FileOpenCommandImplementation())); } }

        private ICommand _startReadingCommand;
        public ICommand StartReadingCommand { get { return _startReadingCommand ?? (_startReadingCommand = new RelayCommand(() => StartReadingCommandImplementation())); } }

        private ICommand _stopReadingCommand;
        public ICommand StopReadingCommand { get { return _stopReadingCommand ?? (_stopReadingCommand = new RelayCommand(() => StopReadingCommandImplementation())); } }

        //public LinkViewModel SelectedLink
        //{
        //    get => _selectedLink; 
        //    set => Set(ref _selectedLink, value, nameof(SelectedLink)); 
        //}


        public MainViewModel()
        {

        }

        private void FileOpenCommandImplementation()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog() { DefaultExt = "cnclink", AddExtension = true, Filter = "Cnc connection|*.cnclink" };
            var b = dlg.ShowDialog();

            if (b.HasValue && b.Value)
            {
                OpenCncConnectionData(dlg.FileName);
            }
        }

        private void OpenCncConnectionData(string fileName)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ConnectionData));

            using (var reader = new System.IO.StreamReader(fileName))
            {
                var cd = (ConnectionData)serializer.Deserialize(reader);
                var list = cd.Links.Select(o => ToViewModel(o)).ToList();

                MessengerInstance.Send(new LoadLinksConnectionsMessage() { Links = list, ChannelType= cd.ChannelType });
                ChannelType = cd.ChannelType;
            }
        }

        private LinkViewModel ToViewModel(LinkData model)
        {
            if (model is LinearLinkData lld)
            {
                return new LinearLinkViewModel(lld.LinkId, lld.Variable) { Description = model.Descrition };
            }
            else if (model is TwoPosLinkData tpld)
            {
                return new TwoPosLinkViewModel(tpld.LinkId, tpld.Variable) { Description = model.Descrition, Inverted = tpld.Inverted };
            }
            else if(model is WriteTwoPosData wtpld)
            {
                return new WriteTwoPosLinkViewModel(wtpld.LinkId, wtpld.Variable) { Description = model.Descrition };
            }
            else if(model is PulseTwoPosData ptpld)
            {
                return new PulseTwoPosLinkViewModel(ptpld.LinkId, ptpld.Variable) { Description = model.Descrition };
            }
            else
            {
                throw new ArgumentException("Unhandled link data type!");
            }
        }

        private void StartReadingCommandImplementation() => MessengerInstance.Send(new StartReadingMessage() { ChennelType = ChannelType });


        private void StopReadingCommandImplementation() => MessengerInstance.Send(new StopReadingMessage());
    }
}
