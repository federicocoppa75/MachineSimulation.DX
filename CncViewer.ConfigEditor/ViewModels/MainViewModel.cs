using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Links = MachineModels.Models.Links;
using LinkType = MachineModels.Enums.LinkType;
using System.Collections.ObjectModel;
using MachineElement = MachineModels.Models.MachineElement;
using CncViewer.Models.Connection;
using CncViewer.Models.Connection.Links;
using CncLinkType = CncViewer.ConfigEditor.Enums.LinkType;
using CncViewer.Models.Connection.Enums;

namespace CncViewer.ConfigEditor.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        public ObservableCollection<LinkViewModel> Links { get; private set; } = new ObservableCollection<LinkViewModel>();

        private LinkViewModel _selectedLink;
        public LinkViewModel SelectedLink
        {
            get => _selectedLink;
            set => Set(ref _selectedLink, value, nameof(SelectedLink));
        }

        private ChannelType _channelType;

        public ChannelType ChannelType
        {
            get => _channelType; 
            set => Set(ref _channelType, value, nameof(ChannelType));
        }


        private ICommand _fileOpenCommand;
        public ICommand FileOpenCommand { get { return _fileOpenCommand ?? (_fileOpenCommand = new RelayCommand(() => FileOpenCommandImplementation())); } }

        private ICommand _fileSaveCommand;
        public ICommand FileSaveCommand { get { return _fileSaveCommand ?? (_fileSaveCommand = new RelayCommand(() => FileSaveCommandImplementation())); } }

        private ICommand _fileOpenConfigurationCommand;
        public ICommand FileOpenConfigurationCommand { get { return _fileOpenConfigurationCommand ?? (_fileOpenConfigurationCommand = new RelayCommand(() => FileOpenConfigurationCommandImplementation())); } }

        private ICommand _fileOpenArchiveCommand;
        public ICommand FileOpenArchiveCommand { get { return _fileOpenArchiveCommand ?? (_fileOpenArchiveCommand = new RelayCommand(() => FileOpenArchiveCommandImplementation())); } }

        private ICommand _fileOpenEnvironmentCommand;
        public ICommand FileOpenEnvironmentCommand { get { return _fileOpenEnvironmentCommand ?? (_fileOpenEnvironmentCommand = new RelayCommand(() => FileOpenEnvironmentCommandImplementation())); } }

        private ICommand _unloadContentCommand;
        public ICommand UnloadContentCommand { get { return _unloadContentCommand ?? (_unloadContentCommand = new RelayCommand(() => UnloadContentCommandImplementation())); } }

        private ICommand _addLinkCommand;
        public ICommand AddLinkCommand { get { return _addLinkCommand ?? (_addLinkCommand = new RelayCommand<CncLinkType>((type) => AddLinkCommandImplementation(type))); } }

        public MainViewModel() : base()
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

        private void FileSaveCommandImplementation()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.DefaultExt = "cnclink";
            dlg.AddExtension = true;
            dlg.Filter = "Cnc connection|*.cnclink";

            var b = dlg.ShowDialog();

            if (b.HasValue && b.Value)
            {
                SaveCncConnectionData(dlg.FileName);
            }
        }

        private void FileOpenConfigurationCommandImplementation()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog() { DefaultExt = "xml", AddExtension = true, Filter = "Machine struct |*.xml" };
            var b = dlg.ShowDialog();

            if (b.HasValue && b.Value)
            {
                LoadMachineFromFile(dlg.FileName);
            }
        }

        private void FileOpenArchiveCommandImplementation()
        {
            throw new NotImplementedException();
        }

        private void FileOpenEnvironmentCommandImplementation()
        {
            throw new NotImplementedException();
        }

        private void UnloadContentCommandImplementation()
        {
            throw new NotImplementedException();
        }

        private void AddLinkCommandImplementation(CncLinkType linkType)
        {
            switch (linkType)
            {
                case CncLinkType.Linear:
                    break;
                case CncLinkType.TwoPos:
                    break;
                case CncLinkType.WriteTwoPos:
                    Links.Add(new WriteTwoPosViewModel(-1));
                    break;
                case CncLinkType.PulseTwoPos:
                    Links.Add(new PulseTwoPosViewModel(-1));
                    break;
                default:
                    break;
            }
        }

        private void LoadMachineFromFile(string machProjectFile)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(MachineElement));

            using (var reader = new System.IO.StreamReader(machProjectFile))
            {
                var me = (MachineElement)serializer.Deserialize(reader);

                var links = new List<Tuple<LinkType, Links.Link>>();

                BrowseModesForLinks(me, (type, link) => links.Add(new Tuple<LinkType, Links.Link>(type, link)));

                Links.Clear();

                foreach (var item in links)
                {
                    int id;

                    switch (item.Item1)
                    {
                        case LinkType.LinearPosition:
                            id = (item.Item2 as Links.LinearPosition).Id;
                            Links.Add(new LinearLinkViewModel(id));
                            break;
                        case LinkType.LinearPneumatic:
                        case LinkType.RotaryPneumatic:
                            id = (item.Item2 as Links.TwoPositionsLink).Id;
                            Links.Add(new TwoPosLinkViewModel(id));
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void BrowseModesForLinks(MachineElement element, Action<LinkType, Links.Link> addLink)
        {
            if(element.LinkToParentType != LinkType.Static)
            {
                addLink(element.LinkToParentType, element.LinkToParentData);
            }

            element.Children.ForEach((e) => BrowseModesForLinks(e, addLink));
        }
        private void SaveCncConnectionData(string fileName)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ConnectionData));

            using (var writer = new System.IO.StreamWriter(fileName))
            {
                var links = Links.Select((o) => o.ToModel()).ToList();
                var cd = new ConnectionData() { Links = links , ChannelType = ChannelType};

                serializer.Serialize(writer, cd);
            }
        }

        private void OpenCncConnectionData(string fileName)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ConnectionData));

            using (var reader = new System.IO.StreamReader(fileName))
            {
                var cd = (ConnectionData)serializer.Deserialize(reader);

                Links.Clear();

                foreach (var item in cd.Links)
                {
                    Links.Add(ToViewModel(item));
                }

                ChannelType = cd.ChannelType;
            }
        }

        private LinkViewModel ToViewModel(LinkData model)
        {
            if(model is LinearLinkData lld)
            {
                return new LinearLinkViewModel(lld.LinkId) { Variable = lld.Variable, Description = lld.Descrition };
            }
            else if(model is TwoPosLinkData tpld)
            {
                return new TwoPosLinkViewModel(tpld.LinkId) { Variable = tpld.Variable, Description = tpld.Descrition, Inverted = tpld.Inverted };
            }
            else
            {
                throw new ArgumentException("Unhandled link data type!");
            }
        }
    }
}
