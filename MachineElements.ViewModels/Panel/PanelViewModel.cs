using HelixToolkit.Wpf.SharpDX;
using MachineElements.ViewModels.Interfaces.Messages.Panel;
using MachineElements.ViewModels.Interfaces.Panel;
using System.Windows.Media.Media3D;

namespace MachineElements.ViewModels.Panel
{
    public class PanelViewModel : MachineElementViewModel, IPanelViewModel
    {
        //public PanelData PanelData { get; set; }
        private Rect3D _bound;

        public double SizeX { get; set; }
        public double SizeY { get; set; }
        public double SizeZ { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double CenterZ { get; set; }

        public Rect3D Bound => _bound;

        public PanelViewModel() : base()
        {
            MessengerInstance.Register<InjectMessage>(this, OnInjectMessage);
            MessengerInstance.Register<InsertMessage>(this, OnInsertMessage);
        }

        private void OnInjectMessage(InjectMessage msg)
        {
            var ie = msg.InjectElement;

            ie.Parent = this;
            Children.Add(ie);
        }

        private void OnInsertMessage(InsertMessage msg)
        {
            var ie = msg.InsertElement;

            ie.Parent = this;
            Children.Add(ie);
        }

        public void Initialize()
        {
            //throw new System.NotImplementedException();
            _bound = new Rect3D(CenterX - SizeX / 2.0, CenterY - SizeY / 2.0, CenterZ - SizeZ / 2.0, SizeX, SizeY, SizeZ);

            var builder = new MeshBuilder();
            builder.AddBox(new SharpDX.Vector3((float)CenterX, (float)CenterY, (float)CenterZ), SizeX, SizeY, SizeZ);

            Geometry = builder.ToMesh();
            Material = PhongMaterials.Orange;
        }
    }
}
