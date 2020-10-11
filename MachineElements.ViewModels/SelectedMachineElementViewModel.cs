using GalaSoft.MvvmLight;
using MachineElements.ViewModels.Interfaces;
using MachineElements.ViewModels.Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Media3D;
using Material = HelixToolkit.Wpf.SharpDX.Material;


namespace MachineElements.ViewModels
{
    public class SelectedMachineElementViewModel : ViewModelBase, ISelectedMachineElementViewModel
    {
        private IMachineElementViewModel _machineElement;

        public int Id => GetDataOrDefault(-1, () => _machineElement.Id);

        public string Name
        {
            get => GetDataOrDefault("none", () => _machineElement.Name);
            set => SetData(value, (v) => _machineElement.Name = v);
        }

        [Browsable(false)]
        public Transform3D Transform
        {
            get => GetDataOrDefault(null, () => _machineElement.Transform);
            set => SetData(value, (v) => _machineElement.Transform = v);
        }

        //[Browsable(false)]
        public Material Material
        {
            get => GetDataOrDefault(null, () => _machineElement.Material);
            set => SetData(value, (v) => _machineElement.Material = v);
        }

        private string _materialName = "none";

        //[Browsable(false)]
        public string MaterialName
        {
            get => _materialName; 
            set => Set(ref _materialName,  value, nameof(MaterialName)); 
        }

        [Browsable(false)]
        public List<string> MaterialsNames { get; set; }


        public SelectedMachineElementViewModel()
        {
            MessengerInstance.Register<SelectMachineElementMessage>(this, OnSelectMachineElementMessage);

            MaterialsNames = HelixToolkit.Wpf.SharpDX.PhongMaterials.Materials.Select(m => m.Name).ToList();
        }

        private void OnSelectMachineElementMessage(SelectMachineElementMessage msg)
        {
            _machineElement = msg.Element;
            RaisePropertyChanged(nameof(Id));
            RaisePropertyChanged(nameof(Name));
            RaisePropertyChanged(nameof(Material));
            RaisePropertyChanged(nameof(MaterialName));

            //var m = HelixToolkit.Wpf.SharpDX.PhongMaterials.Red
        }

        private void SetData<T>(T value, Action<T> actionToSet)
        {
            if(_machineElement != null)
            {
                actionToSet(value);
            }
        }

        private T GetDataOrDefault<T>(T def, Func<T> getFunc) => (_machineElement != null) ? getFunc() : def;
    }

}
