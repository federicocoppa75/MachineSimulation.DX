

using GalaSoft.MvvmLight;
using System.Windows.Media;

namespace MachineElements.ViewModels.Lights
{
    public class AmbientLightViewModel : ViewModelBase
    {
        private Color _color;
        public Color Color 
        { 
            get => _color; 
            set => Set(ref _color, value, nameof(Color)); 
        }
    }
}
