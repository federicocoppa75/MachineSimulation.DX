using GalaSoft.MvvmLight.CommandWpf;
using MachineElement.Model.IO;
using MachineElements.ViewModels.Messages.Tooling;
using System.Linq;
using System.Windows.Input;
using Tooling.Models.IO;
using MachineSteps.Models;
using MachineSteps.ViewModels.Messages;
using MachineElements.ViewModels.Messages.Visibility;
using MachineElements.ViewModels.Messages.Inserters;
using MachineElements.ViewModels.Messages.Links;
using System;
using System.Collections.ObjectModel;
using UpdateAvailableToolSinkListMessage = MachineElements.ViewModels.Messages.ToolChange.UpdateAvailableToolSinkListMessage;
using UpdateAvailableToolsListMessage = MachineElements.ViewModels.Messages.ToolChange.UpdateAvailableToolsListMessage;
using UpdateAvailablePanelHolderMessage = MachineElements.ViewModels.Messages.PanelHolder.UpdateAvailablePanelHolderMessage;
using LinearPositionViewModel = MachineElements.ViewModels.Links.Evo.LinearPositionViewModel;
using MachineElements.ViewModels.Messages.Inverters;
using System.Threading.Tasks;
using MachineElements.ViewModels.Messages.Generic;
using MachineElements.ViewModels.Interfaces.Enums;
using MachineElements.ViewModels.Interfaces;
using MachineElements.ViewModels.Interfaces.Messages;
using MachineElements.ViewModels.Messages.Probe;
using IStepExecutionInfoProvider = MachineElements.ViewModels.Interfaces.IStepExecutionInfoProvider;
using MachineElements.ViewModels.Interfaces.Links;
using ResetAvailableToolSinkListMessage = MachineElements.ViewModels.Messages.ToolChange.ResetAvailableToolSinkListMessage;
using ResetAvailablePanelHolderMessage = MachineElements.ViewModels.Messages.PanelHolder.ResetAvailablePanelHolderMessage;
using System.Windows.Media;
using GalaSoft.MvvmLight.Threading;
using MachineElements.ViewModels.Interfaces.Messages.Panel;

namespace MachineViewer
{
    public class MainViewModel : MachineElements.ViewModels.MainViewModel, IStepExecutionInfoProvider
    {
        private const string _defaultTitle = "Machiene viewer";
        private bool _suspendedAutoStepOverValue;
        private bool _suspendedDynamicTransition;
        private string _machProjectFile;
        private string _toolsFile;
        private string _toolingFile;
        private string _defaultStepExtension = "msteps";

        private bool _isPanelHolderVisible;
        public bool IsPanelHolderVisible
        {
            get { return _isPanelHolderVisible; }
            set
            {
                if (Set(ref _isPanelHolderVisible, value, nameof(IsPanelHolderVisible)))
                {
                    MessengerInstance.Send(new PanelHolderVisibilityChangedMessage() { Value = _isPanelHolderVisible });
                }
            }
        }

        private bool _isCollidersVisible;
        public bool IsCollidersVisible
        {
            get { return _isCollidersVisible; }
            set
            {
                if (Set(ref _isCollidersVisible, value, nameof(IsCollidersVisible)))
                {
                    MessengerInstance.Send(new CollidersVisibilityChangedMessage() { Value = _isCollidersVisible });
                }
            }
        }

        private bool _dynamicTransition;
        public bool DynamicTransition
        {
            get => _dynamicTransition;
            set
            {
                if (Set(ref _dynamicTransition, value, nameof(DynamicTransition)))
                {
                    NotifyDynamicTransitionChanged();
                    if (!_dynamicTransition) AutoStepOver = false;
                }
            }
        }

        private bool _autoStepOver;
        public bool AutoStepOver
        {
            get => _autoStepOver;
            set
            {
                if (Set(ref _autoStepOver, value, nameof(AutoStepOver)))
                {
                    if (_autoStepOver) DynamicTransition = true;
                    else MultiChannel = false;
                    MessengerInstance.Send(new AutoStepOverChangedMessage() { Value = _autoStepOver });
                }
            }
        }

        private bool _multiChannel;
        public bool MultiChannel
        {
            get => _multiChannel;
            set
            {
                if (Set(ref _multiChannel, value, nameof(MultiChannel)))
                {
                    MessengerInstance.Send(new MultiChannelMessage() { Value = _multiChannel });
                }
            }
        }

        private bool _materialRemoval;
        public bool MaterialRemoval
        {
            get => _materialRemoval;
            set
            {
                if (Set(ref _materialRemoval, value, nameof(MaterialRemoval)))
                {
                    MessengerInstance.Send(new MaterialRemovalMessage() { Active = _materialRemoval });
                }
            }
        }

        private bool _isStepTimeVisible;
        public bool IsStepTimeVisible
        {
            get => _isStepTimeVisible;
            set => Set(ref _isStepTimeVisible, value, nameof(IsStepTimeVisible));
        }

        private bool _isAxesStateVisible;
        public bool IsAxesStateVisible
        {
            get => _isAxesStateVisible;
            set => Set(ref _isAxesStateVisible, value, nameof(IsAxesStateVisible));
        }

        private TimeSpan _stepTime = new TimeSpan();
        public TimeSpan StepTime
        {
            get => _stepTime;
            set => Set(ref _stepTime, value, nameof(StepTime));
        }

        private string _inverterName;
        public string InverterName
        {
            get => _inverterName;
            set => Set(ref _inverterName, value, nameof(InverterName));
        }

        private int _inverterValue;
        public int InverterValue
        {
            get => _inverterValue;
            set => Set(ref _inverterValue, value, nameof(InverterValue));
        }

        private bool _isInverterStateVisible;
        public bool IsInverterStateVisible
        {
            get => _isInverterStateVisible;
            set => Set(ref _isInverterStateVisible, value, nameof(IsInverterStateVisible));
        }

        public bool IsDefaultLights
        {
            get => GetLightType() == LightType.Default;
            set => SetLightType(LightType.Default);
        }

        public bool IsSunLight
        {
            get => GetLightType() == LightType.Sun;
            set => SetLightType(LightType.Sun);
        }

        public bool IsSpotHeadLight
        {
            get => GetLightType() == LightType.Spot;
            set => SetLightType(LightType.Spot);
        }

        public bool IsDefaultLights2
        {
            get => GetLightType() == LightType.Default2;
            set => SetLightType(LightType.Default2);
        }
        public bool IsDefaultLights3
        {
            get => GetLightType() == LightType.Default3;
            set => SetLightType(LightType.Default3);
        }

        public bool IsFPSActive
        {
            get => (MachineViewModel != null) ? MachineViewModel.ShowFrameRate : false;
            set
            {
                if (MachineViewModel != null) MachineViewModel.ShowFrameRate = value;
            }
        }

        public bool IsTriangleNumberActive
        {
            get => (MachineViewModel != null) ? MachineViewModel.ShowTriangleCountInfo : false;
            set
            {
                if (MachineViewModel != null) MachineViewModel.ShowTriangleCountInfo = value;
            }
        }

        public bool IsFrameDetailsActive
        {
            get => (MachineViewModel != null) ? MachineViewModel.ShowFrameDetails : false;
            set
            {
                if (MachineViewModel != null) MachineViewModel.ShowFrameDetails = value;
            }
        }

        public bool IsCameraInfoActive
        {
            get => (MachineViewModel != null) ? MachineViewModel.ShowCameraInfo : false;
            set
            {
                if (MachineViewModel != null) MachineViewModel.ShowCameraInfo = value;
            }
        }

        public bool IsEnabledSelectionByView
        {
            get => (MachineViewModel != null) ? MachineViewModel.EnableSelectionByView : false; 
            set 
            {
                if (MachineViewModel != null) MachineViewModel.EnableSelectionByView = value;
            }
        }

        public bool IsEnabledAddProbePoint
        {
            get => (MachineViewModel != null) ? MachineViewModel.AddProbePoint : false;
            set
            {
                if (MachineViewModel != null) MachineViewModel.AddProbePoint = value;
            }
        }

        public Color BackgroundStartColor
        {
            get => (MachineViewModel != null) ? MachineViewModel.BackgroundStartColor : Colors.Gray;
            set
            {
                if (MachineViewModel != null) MachineViewModel.BackgroundStartColor = value;
            }
        }

        public Color BackgroundStopColor
        {
            get => (MachineViewModel != null) ? MachineViewModel.BackgroundStopColor : Colors.Gray;
            set
            {
                if (MachineViewModel != null) MachineViewModel.BackgroundStopColor = value;
            }
        }


        private IMachineViewModel _machineViewModel;

        public IMachineViewModel MachineViewModel
        {
            get 
            { 
                if(_machineViewModel == null)
                {
                    MessengerInstance.Send(new GetMachineViewModel() { SetAction = (o) => _machineViewModel = o });
                }

                return _machineViewModel; 
            }
        }

        private bool _loadLastEnvironmentAtStartup;
        public bool LoadLastEnvironmentAtStartup
        {
            get => _loadLastEnvironmentAtStartup;
            set => Set(ref _loadLastEnvironmentAtStartup, value, nameof(LoadLastEnvironmentAtStartup));
        }

        private string _lastEnvironmentAtStartup;

        public string LastEnvironmentAtStartup
        {
            get { return _lastEnvironmentAtStartup; }
            set { _lastEnvironmentAtStartup = value; }
        }

        private string _title;

        public string Title
        {
            get => _title;
            set => Set(ref _title, value, nameof(Title));
        }



        //public ObservableCollection<LinearPositionViewModel> LinearPositionLinks { get; set; } = new ObservableCollection<LinearPositionViewModel>();
        public ObservableCollection<IUpdatableValueLink<double>> LinearPositionLinks { get; set; } = new ObservableCollection<IUpdatableValueLink<double>>();

        private ICommand _fileOpenCommand;
        public ICommand FileOpenCommand { get { return _fileOpenCommand ?? (_fileOpenCommand = new RelayCommand(() => FileOpenCommandImplementation())); } }

        private ICommand _fileOpenArchiveCommand;
        public ICommand FileOpenArchiveCommand { get { return _fileOpenArchiveCommand ?? (_fileOpenArchiveCommand = new RelayCommand(() => FileOpenArchiveCommandImplementation())); } }

        private ICommand _fileOpenEnvironmentCommand;
        public ICommand FileOpenEnvironmentCommand { get { return _fileOpenEnvironmentCommand ?? (_fileOpenEnvironmentCommand = new RelayCommand(() => FileOpenEnvironmentCommandImplementation())); } }

        private ICommand _fileSaveEnvironmentCommand;
        public ICommand FileSaveEnvironmentCommand { get { return _fileSaveEnvironmentCommand ?? (_fileSaveEnvironmentCommand = new RelayCommand(() => FileSaveEnvironmentCommandImplementation())); } }

        private ICommand _unloadContentCommand;
        public ICommand UnloadContentCommand { get { return _unloadContentCommand ?? (_unloadContentCommand = new RelayCommand(() => UnloadContentCommandImplementation(), () => Machines.Count > 0)); } }

        private ICommand _toolingLoadCommand;
        public ICommand ToolingLoadCommand { get { return _toolingLoadCommand ?? (_toolingLoadCommand = new RelayCommand(() => ToolingLoadCommandImplementation())); } }

        private ICommand _toolingUnloadCommand;
        public ICommand ToolingUnloadCommand { get { return _toolingUnloadCommand ?? (_toolingUnloadCommand = new RelayCommand(() => ToolingUnloadCommandImplementation())); } }

        private ICommand _addPointDistanceCommand;
        public ICommand AddPointDistanceCommand { get { return _addPointDistanceCommand ?? (_addPointDistanceCommand = new RelayCommand(() => AddPointDistanceCommandImplementation(), CanExecuteAddPointDistanceCommand)); } }

        private ICommand _removeProbeDCommand;
        public ICommand RemoveProbeCommand { get { return _removeProbeDCommand ?? (_removeProbeDCommand = new RelayCommand(() => RemoveProbeCommandImplementation(), CanExecuteRemoveProbeCommand)); } }

        private ICommand _loadStepsCommand;
        public ICommand LoadStepsCommand { get { return _loadStepsCommand ?? (_loadStepsCommand = new RelayCommand(() => LoadStepsCommandImplementation())); } }

        private ICommand _unloadStepsCommand;
        public ICommand UnloadStepsCommand { get { return _unloadStepsCommand ?? (_unloadStepsCommand = new RelayCommand(() => UnloadStepsCommandImplementation())); } }

        private ICommand _exportPanelCommand;
        public ICommand ExportPanelCommand { get { return _exportPanelCommand ?? (_exportPanelCommand = new RelayCommand(() => ExportPanelCommandImplementation(), () => PanelPresenceConfirm())); } }

        public MainViewModel() : base()
        {
            Title = _defaultTitle;
            MessengerInstance.Register<SuspendPlaybackSettingsMessage>(this, OnSuspendPlaybackSettingsMessage);
            MessengerInstance.Register<ResumePlaybackSettingsMessage>(this, OnResumePlaybackSettingsMessage);
            MessengerInstance.Register<UpdateStepTimeMessage>(this, OnUpdateStepTimeMessage);
            MessengerInstance.Register<TurnOffInverterMessage>(this, OnTurnOffInverterMessage);
            MessengerInstance.Register<TurnOnInverterMessage>(this, OnTurnOnInverterMessage);
            MessengerInstance.Register<UpdateLinkViewModelsListMessage>(this, OnUpdateLinkViewModelsListMessage);
            MessengerInstance.Register<UpdateInverterMessage>(this, OnUpdateInverterMessage);
        }

        public void OpenLastEnvironment()
        {
            if(LoadLastEnvironmentAtStartup && !string.IsNullOrEmpty(LastEnvironmentAtStartup))
            {
                var fi = new System.IO.FileInfo(LastEnvironmentAtStartup);

                if(fi.Exists)
                {
                    if (MachineLoader.ImportEnvironment(LastEnvironmentAtStartup, out string machProjectFile, out string toolsFile, out string toolingFile))
                    {
                        LoadMachineFromFile(machProjectFile);
                        LoadTooling(toolingFile);
                        //ResetCamera();
                    }
                }
            }
        }

        private void FileOpenCommandImplementation()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog() { DefaultExt = "xml", AddExtension = true, Filter = "Machine struct |*.xml" };
            var b = dlg.ShowDialog();

            if (b.HasValue && b.Value)
            {
                LoadMachineFromFile(dlg.FileName);
            }
        }

        private void LoadMachineFromFile(string fileName)
        {
            var vm = MachineLoader.LoadMachineFromFile(fileName);

            Machines.Add(vm);
            NotifyMachineChanged();
            UpdateToolchangeView();
            UpdatePanelHoldersView();
            UpdateInjectorsView();
            HookLinkForManageInserters();

            _machProjectFile = fileName;

            // in questo modo comunico a tutti i linkappena caricati
            // lo stato attuale del tipo di transizione
            NotifyDynamicTransitionChanged();
        }

        private void FileOpenArchiveCommandImplementation()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog() { DefaultExt = "mcfgx", AddExtension = true, Filter = "Machine configuration|*.mcfgx" };
            var b = dlg.ShowDialog();

            if (b.HasValue && b.Value)
            {
                LoadMachineFromArchive(dlg.FileName);
            }
        }

        private void LoadMachineFromArchive(string machineFile)
        {
            var vm = MachineLoader.LoadMachineFromArchive(machineFile);

            Machines.Add(vm);
            NotifyMachineChanged();
            UpdateToolchangeView();
            UpdatePanelHoldersView();
            UpdateInjectorsView();
            HookLinkForManageInserters();
        }

        private void FileOpenEnvironmentCommandImplementation()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog() { DefaultExt = "env", AddExtension = true, Filter = "Environmet|*.env" };
            var b = dlg.ShowDialog();

            if (b.HasValue && b.Value)
            {
                if (MachineLoader.ImportEnvironment(dlg.FileName, out string machProjectFile, out string toolsFile, out string toolingFile))
                {
                    LoadMachineFromFile(machProjectFile);
                    LoadTooling(toolingFile);
                    //ResetCamera();

                    LastEnvironmentAtStartup = dlg.FileName;
                }
            }
        }

        private void FileSaveEnvironmentCommandImplementation()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.DefaultExt = "env";
            dlg.AddExtension = true;
            dlg.Filter = "Environment|*.env";

            var b = dlg.ShowDialog();

            if (b.HasValue && b.Value)
            {
                MachineLoader.ExportEnvironment(dlg.FileName, _machProjectFile, _toolsFile, _toolingFile);
            }
        }

        private void UnloadContentCommandImplementation()
        {
            Machines.Clear();
            NotifyMachineChanged();
            ResetToolchangeView();
            ResetPanelHoldersView();
            ResetInjectorsView();
        }

        private void ToolingLoadCommandImplementation()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog() { DefaultExt = "tooling", AddExtension = true, Filter = "Tooling |*.tooling" };
            var b = dlg.ShowDialog();

            if (b.HasValue && b.Value)
            {
                LoadTooling(dlg.FileName);
            }
        }

        private void LoadTooling(string fileName)
        {
            if (ToolingLoader.LoadTooling(fileName, out Tooling.Models.Tooling tooling, out Tools.Models.ToolSet toolSet))
            {
                _toolingFile = fileName;
                _toolsFile = tooling.ToolsFile;

                var td = toolSet.Tools.ToDictionary((t) => t.Name);

                foreach (var item in tooling.Units)
                {
                    if (td.TryGetValue(item.ToolName, out Tools.Models.Tool tool))
                    {
                        MessengerInstance.Send(new LoadToolMessage() { ToolHolderId = item.ToolHolderId, Tool = tool });
                    }
                }

                MessengerInstance.Send(new UpdateAvailableToolsListMessage());
            }
        }

        private void ToolingUnloadCommandImplementation()
        {
            MessengerInstance.Send(new MachineElements.ViewModels.Messages.ToolChange.UnloadAllToolsMessage());
            MessengerInstance.Send(new UnloadToolMessage());
        }

        private void UpdateToolchangeView() => MessengerInstance.Send(new UpdateAvailableToolSinkListMessage());

        private void UpdatePanelHoldersView() => MessengerInstance.Send(new UpdateAvailablePanelHolderMessage());

        private void UpdateInjectorsView() => MessengerInstance.Send(new UpdateAvailableInjectorsMessage());

        private void HookLinkForManageInserters() => MessengerInstance.Send(new HookLinkForManageInsertersMessage());

        private void ResetToolchangeView() => MessengerInstance.Send(new ResetAvailableToolSinkListMessage());

        private void ResetPanelHoldersView() => MessengerInstance.Send(new ResetAvailablePanelHolderMessage());

        private void ResetInjectorsView() => MessengerInstance.Send(new ResetAvailableInjectorsMessage());

        private bool CanExecuteAddPointDistanceCommand()
        {
            bool result = false;
            MessengerInstance.Send(new CanExecuteAddPointDistanceMessage() { SetValue = (b) => result = b });
            return result;
        }

        private void AddPointDistanceCommandImplementation() => MessengerInstance.Send(new AddPointDistanceProbeMessage());

        private bool CanExecuteRemoveProbeCommand()
        {
            bool result = false;
            MessengerInstance.Send(new CanExecuteRemoveProbeMessage() { SetValue = (b) => result = b });
            return result;
        }

        private void RemoveProbeCommandImplementation() => MessengerInstance.Send(new RemoveSelectedProbeMessage());

        private void LoadStepsCommandImplementation()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog() { DefaultExt = _defaultStepExtension, AddExtension = true, Filter = "Machine steps |*.msteps|Cnc iso file |*.iso|Cnc iso file |*.i" };
            var b = dlg.ShowDialog();

            if (b.HasValue && b.Value)
            {
                var extension = System.IO.Path.GetExtension(dlg.FileName).Replace(".", "");

                if (string.Compare(extension, "msteps", true) == 0)
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(MachineStepsDocument));

                    using (var reader = new System.IO.StreamReader(dlg.FileName))
                    {
                        var doc = (MachineStepsDocument)serializer.Deserialize(reader);

                        if (doc != null)
                        {
                            ShowMacineStepsDocument(doc);
                            Title = $"{_defaultTitle} - {dlg.FileName}";
                        }
                    }
                }
                else if ((string.Compare(extension, "iso", true) == 0) || (string.Compare(extension, "i", true) == 0))
                {
                    var doc = IsoStepsLoader.LoadAndParse(dlg.FileName, true, GetLinkLimits, () => (LinearPositionLinks != null) ? LinearPositionLinks.Count : 0);

                    if (doc != null)
                    {
                        ShowMacineStepsDocument(doc);
                        Title = $"{_defaultTitle} - {dlg.FileName}";
                    }
                }

                _defaultStepExtension = extension;
            }
        }

        private void ShowMacineStepsDocument(MachineStepsDocument doc)
        {
            MessengerInstance.Send(new LoadStepsMessage() { Steps = doc.Steps });
            if (doc.Steps.Count > 0)
            {
                if ((MachineViewModel.StepExecutionInfoProvider == null) || (!ReferenceEquals(this, MachineViewModel.StepExecutionInfoProvider))) MachineViewModel.StepExecutionInfoProvider = this;
                IsStepTimeVisible = true;
                IsAxesStateVisible = true;
            }
        }

        private void UnloadStepsCommandImplementation()
        {
            var dynamicTransition = DynamicTransition;
            var autoStepOver = AutoStepOver;
            DynamicTransition = false;
            AutoStepOver = false;

            MessengerInstance.Send(new UnloadStepsMessage());
            IsStepTimeVisible = false;
            IsAxesStateVisible = false;

            DynamicTransition = dynamicTransition;
            AutoStepOver = autoStepOver;

            Title = _defaultTitle;
        }

        private void ExportPanelCommandImplementation()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.DefaultExt = "stl";
            dlg.AddExtension = true;
            dlg.Filter = "STL File format |*.stl";

            var b = dlg.ShowDialog();

            if (b.HasValue && b.Value)
            {
                MessengerInstance.Send(new PanelExportRequestMessage() { FileName = dlg.FileName });
            }
        }

        private bool PanelPresenceConfirm()
        {
            bool result = false;

            MessengerInstance.Send(new PanelPresenceRequestMessage() { Confirm = () => result = true });

            return result;
        }

        private void NotifyDynamicTransitionChanged()
        {
            //MessengerInstance.Send(new DynamicTransitionChangedMessage() { Value = _dynamicTransition });
            MessengerInstance.Send(new EnableGradualTransitionMessage() { Value = _dynamicTransition });
            MessengerInstance.Send(new EnablePneumaticTransactionMessage() { Value = _dynamicTransition });
        }

        private void OnSuspendPlaybackSettingsMessage(SuspendPlaybackSettingsMessage obj)
        {
            _suspendedAutoStepOverValue = AutoStepOver;
            _suspendedDynamicTransition = DynamicTransition;
            AutoStepOver = false;
            DynamicTransition = false;
        }

        private void OnResumePlaybackSettingsMessage(ResumePlaybackSettingsMessage obj)
        {
            DynamicTransition = _suspendedDynamicTransition;
            AutoStepOver = _suspendedAutoStepOverValue;
        }

        private void OnUpdateStepTimeMessage(UpdateStepTimeMessage msg) => StepTime = TimeSpan.FromSeconds(msg.Time);

        private void OnTurnOffInverterMessage(TurnOffInverterMessage obj)
        {
            InverterName = string.Empty;
            IsInverterStateVisible = false;
            NotifyBack(obj.BackNotifyId, obj.Duration);
        }

        private void OnTurnOnInverterMessage(TurnOnInverterMessage msg)
        {
            if (!IsInverterStateVisible) IsInverterStateVisible = true;

            int invId = 0;

            if (msg.Order == 0)
            {
                switch (msg.Head)
                {
                    case 1:
                    case 2:
                    case 12:
                    case 21:
                        invId = 2;
                        break;
                    default:
                        throw new ArgumentException("Invalid head id!!!");
                }
            }
            else if (msg.Head == 1)
            {
                switch (msg.Order)
                {
                    case 1:
                        invId = 1;
                        break;
                    case 2:
                        invId = 3;
                        break;
                    default:
                        throw new ArgumentException("Invalid order id!!!");
                }
            }
            else
            {
                throw new InvalidOperationException("Invalid inverter id!!!");
            }

            InverterName = $"S{invId}";
            InverterValue = msg.RotationSpeed;
            NotifyBack(msg.BackNotifyId, msg.Duration);
        }

        private void OnUpdateInverterMessage(UpdateInverterMessage msg)
        {
            InverterValue = msg.RotationSpeed;
            NotifyBack(msg.BackNotifyId, msg.Duration);
        }

        private void NotifyBack(int bakNotifyId, double duration)
        {
            if (bakNotifyId > 0)
            {
                if (DynamicTransition)
                {
                    Task.Delay(TimeSpan.FromSeconds(duration))
                        .ContinueWith((t) =>
                        {
                            MessengerInstance.Send(new BackNotificationMessage() { DestinationId = bakNotifyId });
                        });
                }
                else
                {
                    MessengerInstance.Send(new BackNotificationMessage() { DestinationId = bakNotifyId });
                }
            }
        }

        private void OnUpdateLinkViewModelsListMessage(UpdateLinkViewModelsListMessage msg)
        {
            var list = msg.LinkViewModels.Where((lnk) => lnk is LinearPositionViewModel)
                                         .Cast<LinearPositionViewModel>()
                                         .OrderBy((lpvm) => AxOrder(lpvm.Id))       // sarebbe da fare nella vista ma non avevo voglia!
                                         .ToList();

            LinearPositionLinks.Clear();

            for (int i = 0; i < list.Count; i++) LinearPositionLinks.Add(list[i] as IUpdatableValueLink<double>);
        }

        private int AxOrder(int axId)
        {
            int result = -1;

            switch (axId)
            {
                case 1: result = 1; break;
                case 2: result = 2; break;
                case 101: result = 3; break;
                case 201: result = 4; break;
                case 102: result = 5; break;
                case 202: result = 6; break;
                case 112: result = 7; break;
                case 212: result = 8; break;
                default: break;
            }

            return result;
        }

        private LightType GetLightType() => (MachineViewModel != null) ? MachineViewModel.LightType : LightType.Default;

        private void SetLightType(LightType lightType)
        {
            if((MachineViewModel != null) && (MachineViewModel.LightType != lightType))
            {
                MachineViewModel.LightType = lightType;
                RaisePropertyChanged(nameof(IsDefaultLights));
                RaisePropertyChanged(nameof(IsDefaultLights2));
                RaisePropertyChanged(nameof(IsDefaultLights3));
                RaisePropertyChanged(nameof(IsSunLight));
                RaisePropertyChanged(nameof(IsSpotHeadLight));
            }
        }

        private Tuple<double, double> GetLinkLimits(int linkId)
        {
            double min = 0.0, max = 0.0;

            MessengerInstance.Send(new ReadLinkLimitsMessage()
            {
                LinkId = linkId,
                SetLimits = (a, b) =>
                {
                    min = a;
                    max = b;
                }
            });

            return new Tuple<double, double>(min, max);
        }
    }
}
