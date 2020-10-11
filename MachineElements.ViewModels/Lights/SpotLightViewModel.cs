using GalaSoft.MvvmLight;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MachineElements.ViewModels.Lights
{
    public class SpotLightViewModel : ViewModelBase
    {
        private Color _color;
        public Color Color
        {
            get => _color;
            set => Set(ref _color, value, nameof(Color));
        }

        //public Vector3D Direction { get; set; }
        //public Point3D Position { get; set; }
    }
}
